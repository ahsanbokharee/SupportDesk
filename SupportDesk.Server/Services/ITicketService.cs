
using SupportDesk.Server.Dtos;

namespace SupportDesk.Server.Services;

public interface ITicketService
{
    Task<PagedResult<TicketDto>> GetAllAsync(TicketQuery query);
    Task<TicketDto?> GetByIdAsync(int id);
    Task<TicketDto> CreateAsync(CreateTicketDto dto);
    Task<TicketDto?> UpdateAsync(int id, UpdateTicketDto dto);
    Task<string?> ValidateCategoryAsync(int? categoryId, int? ticketId = null);
}
