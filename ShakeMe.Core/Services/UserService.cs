using ShakeMe.Core.Dtos;
using ShakeMe.Core.Http;

namespace ShakeMe.Core.Services;

public class UserService : IUserService
{
    private readonly IApiClient _apiClient;

    public UserService(IApiClient apiClient)
    {
        Console.WriteLine("🛠️ Constructeur UserService appelé");
        _apiClient = apiClient;
    }

    public async Task<UserDto?> GetProfileAsync()
    {
        return await _apiClient.GetAsync<UserDto>("/api/users/me");
    }

    public async Task<UserDto?> UpdateProfileAsync(UserDto dto)
    {
        return await _apiClient.PutAsync<UserDto, UserDto>("/api/users/me", dto);
    }
}