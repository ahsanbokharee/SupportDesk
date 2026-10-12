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

    private static IOrderedQueryable<Ticket> SortTickets<TKey>(
        IQueryable<Ticket> tickets,
        Expression<Func<Ticket, TKey>> key,
        bool descending)
    {
        return descending ? tickets.OrderByDescending(key) : tickets.OrderBy(key);
    }
}