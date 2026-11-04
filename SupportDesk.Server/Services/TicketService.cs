using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SupportDesk.Server.Data;
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Models;

namespace SupportDesk.Server.Services;

public class TicketService : ITicketService
{
    private readonly AppDbContext _db;

    public TicketService(AppDbContext db)
    {
        _db = db;
    }
    // One place that says how a Ticket (+ its Category) becomes a TicketDto.
    // Expression that helps EF translate it into the SQL SELECT (with a JOIN to Categories).
    private static readonly Expression<Func<Ticket, TicketDto>> ToDto = t => new TicketDto
    {
        Id = t.Id,
        TicketNumber = t.TicketNumber,
        Subject = t.Subject,
        Description = t.Description,
        Status = t.Status,
        Priority = t.Priority,
        CreatedAt = t.CreatedAt,
        CategoryId = t.CategoryId,
        CategoryName = t.Category.Name,
        CategoryColor = t.Category.Color,
        PreviousNumbers = t.Aliases
            .OrderBy(a => a.CreatedAt)
            .Select(a => a.TicketNumber)
            .ToList(),
    };


    public async Task<PagedResult<TicketDto>> GetAllAsync(TicketQuery query)
    {
        IQueryable<Ticket> tickets = _db.Tickets;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            tickets = tickets.Where(t =>
                t.Subject.Contains(search)
                || t.TicketNumber.Contains(search)
                || t.Aliases.Any(a => a.TicketNumber.Contains(search)));
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            tickets = tickets.Where(t => t.Status == query.Status);
        }

        if (!string.IsNullOrEmpty(query.Priority))
        {
            tickets = tickets.Where(t => t.Priority == query.Priority);
        }
        if (query.CategoryId is int categoryId)
        {
            tickets = tickets.Where(t => t.CategoryId == categoryId);
        }

        var totalCount = await tickets.CountAsync();

        var descending = query.SortDir != "asc";

        var sorted = query.SortBy switch
        {
            "priority" => SortTickets(tickets,
                t => t.Priority == "High" ? 3 : t.Priority == "Medium" ? 2 : 1,
                descending),
            "status" => SortTickets(tickets,
                t => t.Status == "Open" ? 1
                   : t.Status == "In Progress" ? 2
                   : t.Status == "Resolved" ? 3 : 4,
                descending),
            "subject" => SortTickets(tickets, t => t.Subject, descending),
            _ => SortTickets(tickets, t => t.CreatedAt, descending),
        };

        var items = await sorted
            .ThenByDescending(t => t.Id)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .Select(ToDto)
            .ToListAsync();

        return new PagedResult<TicketDto>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<TicketDto?> GetByIdAsync(int id)
    {
        return await _db.Tickets
                .Where(t => t.Id == id)
                .Select(ToDto)
                .FirstOrDefaultAsync();
    }

    public async Task<TicketDto> CreateAsync(CreateTicketDto dto)
    {
        var category = dto.CategoryId is int id
                ? await _db.Categories.SingleAsync(c => c.Id == id)
                : await _db.Categories.SingleAsync(c => c.IsDefault);

        await using var transaction = await _db.Database.BeginTransactionAsync();

        // Take the next number for this category in ONE statement. The UPDATE locks the
        // category's row until the transaction ends, so a second request creating a ticket
        // in the same category waits here; two tickets can never get the same number.
        //var numbers = await TakeNextNumberAsync(category.Id);
        var number = await TakeNextNumberAsync(category.Id);

        var ticket = new Ticket
        {
            Subject = dto.Subject,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = "Open",
            CreatedAt = DateTime.UtcNow,
            CategoryId = category.Id,
            Number = number,
            TicketNumber = $"{category.Prefix}-{number}",
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();
        await transaction.CommitAsync();   // only now are the number AND the ticket saved, together

        return (await GetByIdAsync(ticket.Id))!;
    }

    public async Task<TicketDto?> UpdateAsync(int id, UpdateTicketDto dto)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return null;
        }

        await using var transaction = await _db.Database.BeginTransactionAsync();

        if (ticket.CategoryId != dto.CategoryId)
        {
            var newCategory = await _db.Categories.SingleAsync(c => c.Id == dto.CategoryId);

            // Has this ticket been in the target category before? Then it already owns a key there.
            var keyFromEarlierVisit = await _db.TicketAliases
                .SingleOrDefaultAsync(a => a.TicketId == ticket.Id && a.CategoryId == newCategory.Id);

            int number;
            if (keyFromEarlierVisit is not null)
            {
                // Returning: restore the old key (BILL-3 comes back) and stop treating it as an alias.
                number = keyFromEarlierVisit.Number;
                _db.TicketAliases.Remove(keyFromEarlierVisit);
            }
            else
            {
                // First visit to this category: take its next number.
                number = await TakeNextNumberAsync(newCategory.Id);
            }

            // The key it is leaving becomes an alias, so it still finds the ticket.
            _db.TicketAliases.Add(new TicketAlias
            {
                TicketId = ticket.Id,
                CategoryId = ticket.CategoryId,
                Number = ticket.Number,
                TicketNumber = ticket.TicketNumber,
                CreatedAt = DateTime.UtcNow,
            });

            ticket.CategoryId = newCategory.Id;
            ticket.Number = number;
            ticket.TicketNumber = $"{newCategory.Prefix}-{number}";
        }
        ticket.Subject = dto.Subject;
        ticket.Description = dto.Description;
        ticket.Priority = dto.Priority;
        ticket.Status = dto.Status;

        await _db.SaveChangesAsync();
        await transaction.CommitAsync();

        return await GetByIdAsync(id);
    }

    private static IOrderedQueryable<Ticket> SortTickets<TKey>(
        IQueryable<Ticket> tickets,
        Expression<Func<Ticket, TKey>> key,
        bool descending)
    {
        return descending ? tickets.OrderByDescending(key) : tickets.OrderBy(key);
    }
    public async Task<string?> ValidateCategoryAsync(int? categoryId, int? ticketId = null)
    {
        if (categoryId is null)
        {
            return null;                       // create without a category → default is used
        }

        var category = await _db.Categories.FindAsync(categoryId.Value);
        if (category is null)
        {
            return "That category does not exist.";
        }

        if (category.IsActive)
        {
            return null;
        }

        // An inactive category is only allowed if the ticket is already in it
        // (editing a ticket's other fields must keep working after its category is deactivated).
        var unchanged = ticketId is not null
            && await _db.Tickets.AnyAsync(t => t.Id == ticketId && t.CategoryId == categoryId);

        return unchanged ? null : "That category is no longer active.";
    }
    // Takes the next number in a category. MUST run inside a transaction: the UPDATE locks the
    // category row until commit/rollback, so concurrent callers queue and never share a number.
    private async Task<int> TakeNextNumberAsync(int categoryId)
    {
        var numbers = await _db.Database
            .SqlQuery<int>($"UPDATE Categories SET LastNumber = LastNumber + 1 OUTPUT inserted.LastNumber AS [Value] WHERE Id = {categoryId}")
            .ToListAsync();
        return numbers.Single();
    }
}