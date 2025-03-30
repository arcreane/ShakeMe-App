using ShakeMe.Core.Models;
using ShakeMe.Core.Dtos;

namespace ShakeMe.Core.Services;

public class UserService : IUserService
{
    private readonly List<User> _users = new();

    public User CreateUser(CreateUserDto dto)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            IsAnonymous = dto.IsAnonymous,
            Pseudo = string.IsNullOrWhiteSpace(dto.Pseudo)
                ? GeneratePseudo(dto.FirstName, dto.LastName)
                : dto.Pseudo,
            AvatarUrl = dto.AvatarUrl,
            LastActive = DateTime.UtcNow
        };

        _users.Add(user);
        return user;
    }
    
    public User? UpdateUser(Guid id, UpdateUserDto dto)
    {
        var user = GetUserById(id);
        if (user == null) return null;

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        user.Email = dto.Email ?? user.Email;
        user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
        user.IsAnonymous = dto.IsAnonymous ?? user.IsAnonymous;
        user.Pseudo = dto.Pseudo ?? user.Pseudo;
        user.AvatarUrl = dto.AvatarUrl ?? user.AvatarUrl;
        user.LastActive = DateTime.UtcNow;

        return user;
    }

    public User? GetUserById(Guid id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public IEnumerable<User> GetAllUsers()
    {
        return _users;
    }

    public bool DeleteUser(Guid id)
    {
        var user = GetUserById(id);
        if (user == null) return false;

        return _users.Remove(user);
    }

    private string GeneratePseudo(string firstName, string lastName)
    {
        return $"{firstName.ToLowerInvariant()}_{lastName.ToLowerInvariant()}_{Guid.NewGuid().ToString()[..4]}";
    }
}