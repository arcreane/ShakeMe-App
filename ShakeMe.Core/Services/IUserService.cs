using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;

namespace ShakeMe.Core.Services;

public interface IUserService
{
    Task<UserModel> CreateUser(CreateUserDto dto);
    UserModel? UpdateUser(int id, UpdateUserDto dto);
    Task<UserModel?> AuthenticateUserAsync(string identifier, string password);

    
    UserModel? GetUserById(int id);
    IEnumerable<UserModel> GetAllUsers();
    bool DeleteUser(int id);
}
