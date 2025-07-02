using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;
using ShakeMe.Core.Services;
using Xunit;

namespace ShakeMe.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public async Task CreateUser_ShouldAddUserToList()
    {
        var service = new UserService();
        var dto = new CreateUserDto
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            Pseudo = "pseudo"
        };

        var result = await service.CreateUser(dto);

        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);
        Assert.Contains(result, service.GetAllUsers());
    }


    [Fact]
    public async Task GetUserById_WithValidId_ReturnsUser()
    {
        var service = new UserService();
        var dto = new CreateUserDto
        {
            FirstName = "Bob",
            LastName = "Jones",
            Email = "bob@example.com",
            DateOfBirth = new DateTime(1995, 5, 15),
            Pseudo = "bobby"
        };

        var user = await service.CreateUser(dto);

        var result = service.GetUserById(user.Id);

        Assert.NotNull(result);
        Assert.Equal("bob@example.com", result?.Email);
    }

    [Fact]
    public async Task DeleteUser_RemovesUserAndReturnsTrue()
    {
        var service = new UserService();
        var dto = new CreateUserDto
        {
            FirstName = "Charlie",
            LastName = "Brown",
            Email = "charlie@example.com",
            DateOfBirth = new DateTime(1990, 10, 10),
            Pseudo = "charlie"
        };

        var user = await service.CreateUser(dto);

        var result = service.DeleteUser(user.Id);

        Assert.True(result);
        Assert.Null(service.GetUserById(user.Id));
    }

    
    [Fact]
    public async Task UpdateUser_WithValidId_UpdatesFields()
    {
        var service = new UserService();
        var user = await service.CreateUser(new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            Pseudo = "johnny"
        });

        var updateDto = new UpdateUserDto
        {
            FirstName = "Jonathan",
            Email = "jonathan@example.com"
        };

        var updatedUser = service.UpdateUser(user.Id, updateDto);

        Assert.NotNull(updatedUser);
        Assert.Equal("Jonathan", updatedUser.FirstName);
        Assert.Equal("Doe", updatedUser.LastName);
        Assert.Equal("jonathan@example.com", updatedUser.Email);
    }


    [Fact]
    public void UpdateUser_WithInvalidId_ReturnsNull()
    {
        var service = new UserService();

        var updateDto = new UpdateUserDto { FirstName = "Ghost" };
        var result = service.UpdateUser(Guid.NewGuid(), updateDto);

        Assert.Null(result);
    }


    [Fact]
    public async Task UpdateUser_OnlyPartialFields_UpdatesCorrectly()
    {
        var service = new UserService();
        var user = await service.CreateUser(new CreateUserDto
        {
            FirstName = "Sophie",
            LastName = "Dupont",
            Email = "sophie@example.com",
            DateOfBirth = new DateTime(1992, 3, 4),
            Pseudo = "soso"
        });

        var updateDto = new UpdateUserDto { FirstName = "Sofia" };

        var updatedUser = service.UpdateUser(user.Id, updateDto);

        Assert.NotNull(updatedUser);
        Assert.Equal("Sofia", updatedUser.FirstName);
        Assert.Equal("Dupont", updatedUser.LastName);
    }

    [Fact]
    public async Task UpdateUser_UpdatesLastActive()
    {
        var service = new UserService();
        var user = await service.CreateUser(new CreateUserDto
        {
            FirstName = "Tom",
            LastName = "Hardy",
            Email = "tom@example.com",
            DateOfBirth = new DateTime(1985, 10, 1),
            Pseudo = "tommy"
        });

        var before = user.LastActive;

        Thread.Sleep(1000); // Laisse le temps au système de changer l'heure

        var updatedUser = service.UpdateUser(user.Id, new UpdateUserDto { Pseudo = "tom_h" });

        Assert.True(updatedUser.LastActive > before);
    }
}