using ShakeMe.Core.Dtos;

namespace ShakeMe.Core.Services;

public interface IAuthService
{
    Task<AuthenticatedUserDto> RegisterAsync(RegisterDto dto);
    Task<AuthenticatedUserDto?> LoginAsync(string identifier, string password);
}