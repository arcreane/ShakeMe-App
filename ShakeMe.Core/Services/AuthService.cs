using System.Text;
using System.Text.Json;
using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;
using ShakeMe.Core.Services;

namespace ShakeMe.Core.Services;

public class AuthService : IAuthService
{
    private readonly List<UserModel> _users = new(); // Simule une base de données en mémoire
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:3000")
        };

        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<AuthenticatedUserDto?> RegisterAsync(RegisterDto dto)
    {
        var camelCaseOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        var content = new StringContent(
            JsonSerializer.Serialize(dto, camelCaseOptions),
            Encoding.UTF8,
            "application/json"
        );


        var response = await _httpClient.PostAsync("/auth/register", content);
        var raw = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"📩 Réponse brute REGISTER : {raw}");

        if (!response.IsSuccessStatusCode)
            return null;

        return JsonSerializer.Deserialize<AuthenticatedUserDto>(raw, _jsonOptions);
    }

    public async Task<AuthenticatedUserDto?> LoginAsync(string identifier, string password)
    {
        try
        {
            Console.WriteLine("🔑 Tentative de login...");
            var body = new
            {
                identifier,
                password
            };

            var jsonContent = new StringContent(
                JsonSerializer.Serialize(body),
                Encoding.UTF8,
                "application/json"
            );

            var response = await _httpClient.PostAsync("/auth/login", jsonContent);
            var raw = await response.Content.ReadAsStringAsync();

            Console.WriteLine($"📨 Réponse brute JSON : {raw}");

            if (!response.IsSuccessStatusCode)
            {
                Console.WriteLine($"❌ Login échoué : {response.StatusCode}");
                return null;
            }

            var result = JsonSerializer.Deserialize<AuthenticatedUserDto>(raw, _jsonOptions);
            return result;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Exception lors du Login : {ex.Message}");
            return null;
        }
    }

    private int CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;
        return age;
    }

    private bool IsPasswordStrong(string password)
    {
        if (password.Length < 12) return false;
        if (!password.Any(char.IsUpper)) return false;
        if (!password.Any(char.IsLower)) return false;
        if (!password.Any(char.IsDigit)) return false;
        if (!password.Any(c => !char.IsLetterOrDigit(c))) return false;

        return true;
    }

    private UserDto MapToUserDto(UserModel user) => new()
    {
        Id = user.Id,
        Email = user.Email,
        Pseudo = user.Pseudo,
        FirstName = user.FirstName,
        LastName = user.LastName
    };
}
