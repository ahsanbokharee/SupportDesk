using SupportDesk.Server.Dtos;

namespace SupportDesk.Server.Services;

public interface IAuthService
{
    Task<LoginResponseDto?> LoginAsync(LoginRequestDto dto);
}