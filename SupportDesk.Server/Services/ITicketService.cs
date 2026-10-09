
using SupportDesk.Server.Dtos;
using SupportDesk.Server.Models;

namespace SupportDesk.Server.Services;

public interface ITicketService
{
    Task<List<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(int id);
    Task<Ticket> CreateAsync(CreateTicketDto dto);
    Task<Ticket?> UpdateAsync(int id, UpdateTicketDto dto);
}
