using AutoMapper;
using FluentAssertions;
using Lumiere.Application.DTOs.Request.Auth;
using Lumiere.Application.Interfaces.Repositoies;
using Lumiere.Application.Interfaces.Services;
using Lumiere.Application.Services;
using Lumiere.Domain.Entities;
using Lumiere.Tests.Unit.Helpers;
using Moq;

namespace Lumiere.Tests.Unit.Services;

public class AuthServiceTests
{
    private readonly Mock<IUnitOfWork> _uowMock;
    private readonly Mock<IUserRepository> _userRepoMock;
    private readonly Mock<IRoleRepository> _roleRepoMock;
    private readonly Mock<ITokenService> _tokenMock;
    private readonly IMapper _mapper;
    private readonly AuthService _sut;

    public AuthServiceTests()
    {
        _uowMock = new Mock<IUnitOfWork>();
        _userRepoMock = new Mock<IUserRepository>();
        _roleRepoMock = new Mock<IRoleRepository>();
        _tokenMock = new Mock<ITokenService>();
        _mapper = MapperFactory.Create();

        _uowMock.Setup(u => u.Users).Returns(_userRepoMock.Object);
        _uowMock.Setup(u => u.Roles).Returns(_roleRepoMock.Object);

        // Token service always returns a fake token
        _tokenMock.Setup(t => t.GenerateToken(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>()))
                  .Returns("fake-jwt-token");
        _tokenMock.Setup(t => t.GetExpiration()).Returns(DateTime.UtcNow.AddHours(1));

        _sut = new AuthService(_uowMock.Object, _tokenMock.Object, _mapper);
    }

    // LoginAsync

    [Fact]
    public async Task LoginAsync_WithValidCredentials_ReturnsTokenAndUser()
    {
        // Arrange — create a real BCrypt hash so the verify passes
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("password123");
        var user = new User { Id = 1, Email = "test@test.com", PasswordHash = passwordHash, IsDeleted = false };
        var userWithRole = new User { Id = 1, Email = "test@test.com", PasswordHash = passwordHash, Role = new Role { Name = "User" } };

        _userRepoMock.Setup(r => r.GetByEmailAsync("test@test.com", default)).ReturnsAsync(user);
        _userRepoMock.Setup(r => r.GetWithRoleAsync(1, default)).ReturnsAsync(userWithRole);

        // Act
        var result = await _sut.LoginAsync(new LoginRequest { Email = "test@test.com", Password = "password123" });

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Token.Should().Be("fake-jwt-token");
        result.Data.User.Should().NotBeNull();
    }

    [Fact]
    public async Task LoginAsync_WithWrongPassword_ReturnsFail()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("correctPassword");
        var user = new User { Id = 1, Email = "test@test.com", PasswordHash = passwordHash, IsDeleted = false };
        _userRepoMock.Setup(r => r.GetByEmailAsync("test@test.com", default)).ReturnsAsync(user);

        // Act
        var result = await _sut.LoginAsync(new LoginRequest { Email = "test@test.com", Password = "wrongPassword" });

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("Invalid email or password");
    }

    [Fact]
    public async Task LoginAsync_WithNonExistentEmail_ReturnsFail()
    {
        // Arrange
        _userRepoMock.Setup(r => r.GetByEmailAsync("nobody@test.com", default)).ReturnsAsync((User?)null);

        // Act
        var result = await _sut.LoginAsync(new LoginRequest { Email = "nobody@test.com", Password = "any" });

        // Assert
        result.Success.Should().BeFalse();
    }

    // RegisterAsync

    [Fact]
    public async Task RegisterAsync_WithNewEmail_CreatesUserAndReturnsToken()
    {
        // Arrange
        var request = new RegisterRequest { Name = "John", Email = "new@test.com", Password = "pass123", Country = "IN" };
        var userRole = new Role { Id = 2, Name = "User" };
        var savedUser = new User { Id = 5, Email = "new@test.com", Role = userRole };

        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
                     .ReturnsAsync(false); // email not taken

        _roleRepoMock.Setup(r => r.GetByNameAsync("User", default)).ReturnsAsync(userRole);
        _uowMock.Setup(u => u.SaveChangesAsync(default)).ReturnsAsync(1);
        _userRepoMock.Setup(r => r.GetWithRoleAsync(It.IsAny<int>(), default)).ReturnsAsync(savedUser);

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeTrue();
        result.Data!.Token.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public async Task RegisterAsync_WithExistingEmail_ReturnsFail()
    {
        // Arrange
        var request = new RegisterRequest { Email = "existing@test.com", Password = "pass123" };

        _userRepoMock.Setup(r => r.ExistsAsync(It.IsAny<System.Linq.Expressions.Expression<Func<User, bool>>>(), default))
                     .ReturnsAsync(true); // email IS taken

        // Act
        var result = await _sut.RegisterAsync(request);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Contain("already registered");
        _uowMock.Verify(u => u.SaveChangesAsync(default), Times.Never);
    }
}
