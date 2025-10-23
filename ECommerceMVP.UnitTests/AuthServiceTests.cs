using ECommerceMVP.Application.DTOs;
using ECommerceMVP.Application.Services;
using ECommerceMVP.Domain.Entities;
using ECommerceMVP.Domain.Repositories;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace ECommerceMVP.UnitTests;

public class AuthServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _configurationMock = new Mock<IConfiguration>();

        // Setup JWT configuration with proper mocking
        _configurationMock.Setup(x => x["Jwt:Key"]).Returns("YourSuperSecretKeyHereThatIsAtLeast32CharactersLong!");
        _configurationMock.Setup(x => x["Jwt:Issuer"]).Returns("TestIssuer");
        _configurationMock.Setup(x => x["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_userRepositoryMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_ValidRequest_ShouldCreateUserAndReturnAuthResponse()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "testuser",
            Email = "test@example.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User"
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameOrEmailAsync(request.Username, request.Email))
            .ReturnsAsync((User?)null);

        User? capturedUser = null;
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<User>()))
            .Callback<User>(u => {
                capturedUser = u;
                // Simulate MongoDB setting the ID
                if (string.IsNullOrEmpty(u.Id))
                {
                    u.Id = "507f1f77bcf86cd799439011";
                }
            })
            .Returns(Task.CompletedTask);

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.User.Username.Should().Be(request.Username);
        result.User.Email.Should().Be(request.Email);
        result.User.FirstName.Should().Be(request.FirstName);
        result.User.LastName.Should().Be(request.LastName);
        result.Token.Should().NotBeNullOrEmpty();

        capturedUser.Should().NotBeNull();
        capturedUser!.Username.Should().Be(request.Username);
        capturedUser.Email.Should().Be(request.Email);
        capturedUser.FirstName.Should().Be(request.FirstName);
        capturedUser.LastName.Should().Be(request.LastName);
        capturedUser.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, TimeSpan.FromSeconds(1));
    }

    [Fact]
    public async Task RegisterAsync_UsernameExists_ShouldThrowException()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Username = "existinguser",
            Email = "new@example.com",
            Password = "password123",
            FirstName = "Test",
            LastName = "User"
        };

        var existingUser = new User { Username = "existinguser", Email = "old@example.com" };
        _userRepositoryMock.Setup(x => x.GetByUsernameOrEmailAsync(request.Username, request.Email))
            .ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ShouldReturnAuthResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "password123"
        };

        var user = new User
        {
            Id = "507f1f77bcf86cd799439011",
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123"),
            FirstName = "Test",
            LastName = "User"
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        result.Should().NotBeNull();
        result.User.UserId.Should().Be(user.Id);
        result.User.Username.Should().Be(user.Username);
        result.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task LoginAsync_InvalidUsername_ShouldThrowException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "nonexistent",
            Password = "password123"
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.LoginAsync(request));
    }

    [Fact]
    public async Task LoginAsync_InvalidPassword_ShouldThrowException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Username = "testuser",
            Password = "wrongpassword"
        };

        var user = new User
        {
            Username = "testuser",
            Email = "test@example.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("password123")
        };

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync(request.Username))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(() => _authService.LoginAsync(request));
    }
}