using Xunit;
using ShakeMe.Core.Services;
using ShakeMe.Core.Dtos;
using System;
using System.Threading.Tasks;

namespace ShakeMe.Tests.Services
{
    public class AuthServiceTests
    {
        private readonly AuthService _authService;

        public AuthServiceTests()
        {
            _authService = new AuthService();
        }

        [Fact]
        public async Task RegisterAsync_ShouldCreateUser()
        {
            var dto = new RegisterDto
            {
                FirstName = "Jean",
                LastName = "Dupont",
                Email = "jean@example.com",
                Pseudo = "JeanDu94",
                DateOfBirth = new DateTime(1998, 5, 12),
                Password = "Password123!"
            };

            var result = await _authService.RegisterAsync(dto);

            Assert.Equal(dto.Email, result.Email);
            Assert.Equal(dto.Pseudo, result.Pseudo);
            Assert.NotEqual(Guid.Empty, result.Id);
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowIfEmailAlreadyExists()
        {
            var dto1 = new RegisterDto
            {
                FirstName = "Alice",
                LastName = "Smith",
                Email = "alice@example.com",
                Pseudo = "Alicia",
                DateOfBirth = new DateTime(2000, 1, 1),
                Password = "SecurePass1!"
            };

            var dto2 = new RegisterDto
            {
                FirstName = "Alice2",
                LastName = "Smith2",
                Email = "alice@example.com", // même email
                Pseudo = "Alicia2",
                DateOfBirth = new DateTime(1999, 1, 1),
                Password = "SecurePass2!"
            };

            await _authService.RegisterAsync(dto1);
            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(dto2));
        }


        [Fact]
        public async Task RegisterAsync_ShouldThrowIfUnder13()
        {
            var dto = new RegisterDto
            {
                FirstName = "Young",
                LastName = "Kid",
                Email = "young@example.com",
                Pseudo = "Youngster",
                DateOfBirth = DateTime.Today.AddYears(-10),
                Password = "StrongPass1!"
            };

            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(dto));
        }

        [Fact]
        public async Task RegisterAsync_ShouldThrowIfPasswordWeak()
        {
            var dto = new RegisterDto
            {
                FirstName = "Weak",
                LastName = "Password",
                Email = "weak@example.com",
                Pseudo = "Weakling",
                DateOfBirth = new DateTime(2000, 1, 1),
                Password = "123"
            };

            await Assert.ThrowsAsync<Exception>(() => _authService.RegisterAsync(dto));
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceedWithEmail()
        {
            var dto = new RegisterDto
            {
                FirstName = "Bob",
                LastName = "Doe",
                Email = "bob@example.com",
                Pseudo = "Bobby",
                DateOfBirth = new DateTime(1990, 1, 1),
                Password = "StrongPass1!"
            };

            await _authService.RegisterAsync(dto);

            var login = new LoginDto
            {
                Identifier = "bob@example.com",
                Password = "StrongPass1!"
            };

            var result = await _authService.LoginAsync(login);

            Assert.Equal("Bobby", result.Pseudo);
            Assert.Equal("bob@example.com", result.Email);
        }

        [Fact]
        public async Task LoginAsync_ShouldSucceedWithPseudo()
        {
            var dto = new RegisterDto
            {
                FirstName = "Charlie",
                LastName = "Zed",
                Email = "charlie@abc.com",
                Pseudo = "ZedCh",
                DateOfBirth = new DateTime(1995, 6, 15),
                Password = "StrongPass2!"
            };

            await _authService.RegisterAsync(dto);

            var login = new LoginDto
            {
                Identifier = "ZedCh",
                Password = "StrongPass2!"
            };

            var result = await _authService.LoginAsync(login);

            Assert.Equal("charlie@abc.com", result.Email);
        }


        [Fact]
        public async Task LoginAsync_ShouldFailWithWrongPassword()
        {
            var dto = new RegisterDto
            {
                FirstName = "David",
                LastName = "M",
                Email = "david@mail.com",
                Pseudo = "Dav",
                DateOfBirth = new DateTime(2000, 1, 1),
                Password = "StrongPass3!"
            };

            await _authService.RegisterAsync(dto);

            var login = new LoginDto
            {
                Identifier = "david@mail.com",
                Password = "wrongpass"
            };

            await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(login));
        }


        [Fact]
        public async Task LoginAsync_ShouldFailIfUserNotFound()
        {
            var login = new LoginDto
            {
                Identifier = "ghost@nope.com",
                Password = "anyStrongPass1!"
            };

            await Assert.ThrowsAsync<Exception>(() => _authService.LoginAsync(login));
        }
    }
}