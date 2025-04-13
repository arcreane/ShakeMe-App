using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;
using ShakeMe.Core.Services;
using BCrypt.Net;

namespace ShakeMe.Core.Services;

public class AuthService : IAuthService
{
    private readonly List<UserModel> _users = new(); // Simule une base de données en mémoire

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
            Id = Guid.NewGuid(),
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
            Id = user.Id,
            Email = user.Email,
            Pseudo = user.Pseudo
        };
    }

    public async Task<AuthenticatedUserDto> LoginAsync(LoginDto dto)
    {
        // Cherche par email ou pseudo
        var user = _users.FirstOrDefault(u =>
            u.Email.Equals(dto.Identifier, StringComparison.OrdinalIgnoreCase) ||
            u.Pseudo.Equals(dto.Identifier, StringComparison.OrdinalIgnoreCase));

        if (user is null)
            throw new Exception("Utilisateur non trouvé.");

        // Vérifie le mot de passe
        // if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
        //     throw new Exception("Mot de passe incorrect.");
        if (user.PasswordHash != dto.Password)
            throw new Exception("Mot de passe incorrect.");

        user.LastActive = DateTime.UtcNow;

        return new AuthenticatedUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Pseudo = user.Pseudo
        };
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

}