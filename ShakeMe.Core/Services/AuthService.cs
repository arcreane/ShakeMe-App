using System.Net.Http.Json;
using System.Text.Json;
using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;
using ShakeMe.Core.Services;
using BCrypt.Net;

namespace ShakeMe.Core.Services;

public class AuthService : IAuthService
{
    private readonly List<UserModel> _users = new(); // Simule une base de données en mémoire

    private readonly HttpClient _httpClient;

    public AuthService()
    {
        _httpClient = new HttpClient
        {
            BaseAddress = new Uri("http://10.0.2.2:3000")
        };
    }

    public async Task<AuthenticatedUserDto> RegisterAsync(RegisterDto dto)
    {
        // Vérification âge minimum
        if (CalculateAge(dto.DateOfBirth) < 13)
            throw new Exception("Vous devez avoir au moins 13 ans pour vous inscrire.");

        // Vérification mot de passe conforme ANSSI
        if (!IsPasswordStrong(dto.Password))
            throw new Exception("Le mot de passe doit faire au moins 12 caractères et contenir une majuscule, une minuscule, un chiffre et un caractère spécial.");

        // Vérifie si l'email ou le pseudo existe déjà
        if (_users.Any(u => u.Email == dto.Email || u.Pseudo == dto.Pseudo))
            throw new Exception("Email ou pseudo déjà utilisé.");

        // var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);
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
        var response = await _httpClient.PostAsJsonAsync("/auth/login", new
        {
            identifier,
            password
        });

        var raw = await response.Content.ReadAsStringAsync();
        Console.WriteLine($"📨 Réponse brute JSON : {raw}");

        if (!response.IsSuccessStatusCode)
            return null;

        // test direct avec JsonSerializer
        var result = JsonSerializer.Deserialize<AuthenticatedUserDto>(raw);
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