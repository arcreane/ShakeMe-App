using ShakeMe.Core.Models;
using ShakeMe.Core.Dtos;

namespace ShakeMe.Core.Services;

public class UserService : IUserService
{
    private readonly List<UserModel> _users = new();

    public UserModel CreateUser(CreateUserDto dto)
    {
        var user = new UserModel
        {
            Id = Guid.NewGuid(),
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            Email = dto.Email,
            DateOfBirth = dto.DateOfBirth,
            Pseudo = string.IsNullOrWhiteSpace(dto.Pseudo)
                ? GeneratePseudo(dto.FirstName, dto.LastName)
                : dto.Pseudo,
            LastActive = DateTime.UtcNow
        };

        _users.Add(user);
        return user;
    }
    
    public UserModel? UpdateUser(Guid id, UpdateUserDto dto)
    {
        var user = GetUserById(id);
        if (user == null) return null;

        user.FirstName = dto.FirstName ?? user.FirstName;
        user.LastName = dto.LastName ?? user.LastName;
        user.Email = dto.Email ?? user.Email;
        user.DateOfBirth = dto.DateOfBirth ?? user.DateOfBirth;
        user.Pseudo = dto.Pseudo ?? user.Pseudo;
        user.LastActive = DateTime.UtcNow;

        return user;
    }

    public UserModel? GetUserById(Guid id)
    {
        return _users.FirstOrDefault(u => u.Id == id);
    }

    public IEnumerable<UserModel> GetAllUsers()
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