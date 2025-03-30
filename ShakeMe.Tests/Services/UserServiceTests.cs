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
}