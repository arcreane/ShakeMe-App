using ShakeMe.Core.Dtos;

namespace ShakeMe.Core.Services;

public interface IUserService
{
    Task<UserDto?> GetProfileAsync();
    Task<UserDto?> UpdateProfileAsync(UserDto dto);
}