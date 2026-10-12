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

    public async Task<PagedResult<Ticket>> GetAllAsync(TicketQuery query)
    {
        IQueryable<Ticket> tickets = _db.Tickets;

        if (!string.IsNullOrWhiteSpace(query.Search))
        {
            var search = query.Search.Trim();
            tickets = tickets.Where(t =>
                t.Subject.Contains(search) || t.TicketNumber.Contains(search));
        }

        if (!string.IsNullOrEmpty(query.Status))
        {
            tickets = tickets.Where(t => t.Status == query.Status);
        }

        if (!string.IsNullOrEmpty(query.Priority))
        {
            tickets = tickets.Where(t => t.Priority == query.Priority);
        }

        var totalCount = await tickets.CountAsync();

        var items = await tickets
            .OrderByDescending(t => t.CreatedAt)
            .Skip((query.Page - 1) * query.PageSize)
            .Take(query.PageSize)
            .ToListAsync();

        return new PagedResult<Ticket>
        {
            Items = items,
            Page = query.Page,
            PageSize = query.PageSize,
            TotalCount = totalCount
        };
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _db.Tickets.FindAsync(id);
    }

    public async Task<Ticket> CreateAsync(CreateTicketDto dto)
    {
        var ticket = new Ticket
        {
            Subject = dto.Subject,
            Description = dto.Description,
            Priority = dto.Priority,
            Status = "Open",
            CreatedAt = DateTime.UtcNow
        };

        _db.Tickets.Add(ticket);
        await _db.SaveChangesAsync();

        ticket.TicketNumber = $"TCK-{1000 + ticket.Id}";
        await _db.SaveChangesAsync();

        return ticket;
    }

    public async Task<Ticket?> UpdateAsync(int id, UpdateTicketDto dto)
    {
        var ticket = await _db.Tickets.FindAsync(id);
        if (ticket == null)
        {
            return null;
        }

        ticket.Subject = dto.Subject;
        ticket.Description = dto.Description;
        ticket.Priority = dto.Priority;
        ticket.Status = dto.Status;

        await _db.SaveChangesAsync();
        return ticket;
    }
}