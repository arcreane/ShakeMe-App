using System.Net.Http.Headers;
using System.Text.Json;
using Microsoft.Maui.Storage;
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
        await Task.Delay(100); // debug uniquement

        await _apiClient.GetAsync<UserDto>("/api/users/profile");

        var httpClient = new HttpClient();
        var token = await SecureStorage.GetAsync("auth_token");
        httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var response = await httpClient.GetAsync("http://10.0.2.2:3000/api/users/profile");
        var raw = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"📩 Réponse brute profil : {raw}");

        if (!response.IsSuccessStatusCode)
        {
            Console.WriteLine($"❌ Erreur GetProfileAsync : {response.StatusCode}");
            return null;
        }

        return JsonSerializer.Deserialize<UserDto>(raw, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
    }


    public async Task<UserDto?> UpdateProfileAsync(UserDto dto)
    {
        var response = await _apiClient.PutAsync<UserDto, UserResponseDto>("/api/users/me", dto);
        return response?.User;
    }

}