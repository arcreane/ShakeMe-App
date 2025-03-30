using ShakeMe.Core.Dtos;
using ShakeMe.Core.Models;
using ShakeMe.Core.Services;
using Xunit;

namespace ShakeMe.Tests.Services;

public class UserServiceTests
{
    [Fact]
    public void CreateUser_ShouldAddUserToList()
    {
        // Arrange
        var service = new UserService();
        var dto = new CreateUserDto
        {
            FirstName = "Alice",
            LastName = "Smith",
            Email = "alice@example.com",
            DateOfBirth = new DateTime(2000, 1, 1),
            IsAnonymous = false,
            Pseudo = "pseudo",
            AvatarUrl = "http://example.com/avatar.png"
        };

        // Act
        var result = service.CreateUser(dto);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(dto.Email, result.Email);
        Assert.Contains(result, service.GetAllUsers());
    }

    [Fact]
    public void GetUserById_WithValidId_ReturnsUser()
    {
        var service = new UserService();
        var dto = new CreateUserDto
        {
            FirstName = "Bob",
            LastName = "Jones",
            Email = "bob@example.com",
            DateOfBirth = new DateTime(1995, 5, 15),
            IsAnonymous = true,
            Pseudo = "bobby"
        };

        var user = service.CreateUser(dto);

        var result = service.GetUserById(user.Id);

        Assert.NotNull(result);
        Assert.Equal("bob@example.com", result?.Email);
    }

    [Fact]
    public void DeleteUser_RemovesUserAndReturnsTrue()
    {
        var service = new UserService();
        var dto = new CreateUserDto
        {
            FirstName = "Charlie",
            LastName = "Brown",
            Email = "charlie@example.com",
            DateOfBirth = new DateTime(1990, 10, 10),
            IsAnonymous = false,
            Pseudo = "charlie"
        };

        var user = service.CreateUser(dto);

        var result = service.DeleteUser(user.Id);

        Assert.True(result);
        Assert.Null(service.GetUserById(user.Id));
    }
    
    [Fact]
    public void UpdateUser_WithValidId_UpdatesFields()
    {
        var service = new UserService();
        var user = service.CreateUser(new CreateUserDto
        {
            FirstName = "John",
            LastName = "Doe",
            Email = "john@example.com",
            DateOfBirth = new DateTime(1990, 1, 1),
            IsAnonymous = false,
            Pseudo = "johnny",
            AvatarUrl = "avatar1.png"
        });

        var updateDto = new UpdateUserDto
        {
            FirstName = "Jonathan",
            Email = "jonathan@example.com",
            AvatarUrl = "avatar2.png"
        };

        var updatedUser = service.UpdateUser(user.Id, updateDto);

        Assert.NotNull(updatedUser);
        Assert.Equal("Jonathan", updatedUser.FirstName);
        Assert.Equal("Doe", updatedUser.LastName);
        Assert.Equal("jonathan@example.com", updatedUser.Email);
        Assert.Equal("avatar2.png", updatedUser.AvatarUrl);
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
    public void UpdateUser_OnlyPartialFields_UpdatesCorrectly()
    {
        var service = new UserService();
        var user = service.CreateUser(new CreateUserDto
        {
            FirstName = "Sophie",
            LastName = "Dupont",
            Email = "sophie@example.com",
            DateOfBirth = new DateTime(1992, 3, 4),
            IsAnonymous = false,
            Pseudo = "soso",
            AvatarUrl = "old.png"
        });

        var updateDto = new UpdateUserDto { AvatarUrl = "new.png" };

        var updatedUser = service.UpdateUser(user.Id, updateDto);

        Assert.NotNull(updatedUser);
        Assert.Equal("Sophie", updatedUser.FirstName);
        Assert.Equal("new.png", updatedUser.AvatarUrl);
    }

    [Fact]
    public void UpdateUser_UpdatesLastActive()
    {
        var service = new UserService();
        var user = service.CreateUser(new CreateUserDto
        {
            FirstName = "Tom",
            LastName = "Hardy",
            Email = "tom@example.com",
            DateOfBirth = new DateTime(1985, 10, 1),
            IsAnonymous = false,
            Pseudo = "tommy",
            AvatarUrl = "old.png"
        });

        var before = user.LastActive;

        Thread.Sleep(1000);

        var updatedUser = service.UpdateUser(user.Id, new UpdateUserDto { Pseudo = "tom_h" });

        Assert.True(updatedUser.LastActive > before);
    }

}