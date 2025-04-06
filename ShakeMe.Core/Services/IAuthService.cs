using ShakeMe.Core.Dtos;

namespace ShakeMe.Core.Services;

public interface IAuthService
{
    Task<AuthenticatedUserDto> RegisterAsync(RegisterDto dto);
    Task<AuthenticatedUserDto> LoginAsync(LoginDto dto);
}