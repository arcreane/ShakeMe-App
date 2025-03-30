using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;

namespace ShakeMe.Core.Services;

public interface IUserService
{
    User CreateUser(CreateUserDto dto);
    User? GetUserById(Guid id);
    IEnumerable<User> GetAllUsers();
    bool DeleteUser(Guid id);
}
