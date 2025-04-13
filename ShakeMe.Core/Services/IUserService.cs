using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;

namespace ShakeMe.Core.Services;

public interface IUserService
{
    Task<UserModel> CreateUser(CreateUserDto dto);
    UserModel? UpdateUser(Guid id, UpdateUserDto dto);
    Task<UserModel?> AuthenticateUserAsync(string identifier, string password);

    
    UserModel? GetUserById(Guid id);
    IEnumerable<UserModel> GetAllUsers();
    bool DeleteUser(Guid id);
}
