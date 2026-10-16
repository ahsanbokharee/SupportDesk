using SupportDesk.Server.Dtos;

namespace SupportDesk.Server.Services;

public interface IUserService
{
    Task<List<UserDto>> GetAllAsync();
    Task<UserDto?> GetByIdAsync(int id);
    Task<bool> EmailExistsAsync(string email, int? exceptUserId = null);
    Task<UserDto> CreateAsync(CreateUserDto dto);
    Task<UserDto?> UpdateAsync(int id, UpdateUserDto dto);
}