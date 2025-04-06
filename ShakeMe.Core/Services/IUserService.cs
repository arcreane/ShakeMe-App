using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;

namespace ShakeMe.Core.Services;

public interface IUserService
{
    UserModel CreateUser(CreateUserDto dto);
    UserModel? UpdateUser(Guid id, UpdateUserDto dto);
    UserModel? GetUserById(Guid id);
    IEnumerable<UserModel> GetAllUsers();
    bool DeleteUser(Guid id);
}
