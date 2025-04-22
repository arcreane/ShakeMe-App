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

    public async Task<AuthenticatedUserDto> RegisterAsync(RegisterDto dto)
    {
        if (CalculateAge(dto.DateOfBirth) < 13)
            throw new Exception("Vous devez avoir au moins 13 ans pour vous inscrire.");

        if (!IsPasswordStrong(dto.Password))
            throw new Exception("Le mot de passe doit faire au moins 12 caractères et contenir une majuscule, une minuscule, un chiffre et un caractère spécial.");

        if (_users.Any(u => u.Email == dto.Email || u.Pseudo == dto.Pseudo))
            throw new Exception("Email ou pseudo déjà utilisé.");

        var passwordHash = dto.Password;

        var user = new UserModel
        {
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            Pseudo = dto.Pseudo,
            PasswordHash = passwordHash,
            DateOfBirth = dto.DateOfBirth,
            LastActive = DateTime.UtcNow
        };

        _users.Add(user);

        return new AuthenticatedUserDto
        {
            Token = "token-plus-tard",
            User = MapToUserDto(user)
        };
    }

    public async Task<AuthenticatedUserDto?> LoginAsync(string identifier, string password)
    {
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
            return null;

        var result = JsonSerializer.Deserialize<AuthenticatedUserDto>(raw, _jsonOptions);
        return result;
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
