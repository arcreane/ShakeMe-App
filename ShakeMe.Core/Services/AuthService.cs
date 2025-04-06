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
        // Vérifie si l'email ou le pseudo existe déjà
        if (_users.Any(u => u.Email == dto.Email || u.Pseudo == dto.Pseudo))
            throw new Exception("Email ou pseudo déjà utilisé.");

        // Hash du mot de passe
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password);

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
        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new Exception("Mot de passe incorrect.");

        user.LastActive = DateTime.UtcNow;

        return new AuthenticatedUserDto
        {
            Id = user.Id,
            Email = user.Email,
            Pseudo = user.Pseudo
        };
    }
}