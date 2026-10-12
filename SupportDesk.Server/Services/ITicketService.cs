
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Models;

namespace SupportDesk.Server.Services;

public interface ITicketService
{
    Task<PagedResult<Ticket>> GetAllAsync(TicketQuery query);
    Task<Ticket?> GetByIdAsync(int id);
    Task<Ticket> CreateAsync(CreateTicketDto dto);
    Task<Ticket?> UpdateAsync(int id, UpdateTicketDto dto);
}
