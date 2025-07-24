// AuthServiceTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Moq;
using VendingMachine.Application.Services;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using Xunit;

namespace VendingMachine.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<IConfiguration> _configurationMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);

        _configurationMock = new Mock<IConfiguration>();
        var jwtSection = new Mock<IConfigurationSection>();

        _configurationMock.Setup(x => x.GetSection("Jwt")).Returns(jwtSection.Object);
        jwtSection.Setup(x => x["Key"]).Returns("VendingMachineSecretKey2024!@#$%^&*()_+");
        jwtSection.Setup(x => x["Issuer"]).Returns("VendingMachineAPI");
        jwtSection.Setup(x => x["Audience"]).Returns("VendingMachineUsers");
        jwtSection.Setup(x => x["ExpireMinutes"]).Returns("60");

        _authService = new AuthService(_userManagerMock.Object, _configurationMock.Object);
    }

    [Fact]
    public async Task LoginAsync_ValidCredentials_ReturnsToken()
    {
        // Arrange
        var user = new ApplicationUser { Id = "123", UserName = "testuser", Deposit = 100 };
        var loginDto = new UserLoginDto { Username = "testuser", Password = "password" };

        _userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
            .ReturnsAsync(user);
        _userManagerMock.Setup(x => x.CheckPasswordAsync(user, "password"))
            .ReturnsAsync(true);
        _userManagerMock.Setup(x => x.GetRolesAsync(user))
            .ReturnsAsync(new List<string> { "buyer" });

        // Act
        var result = await _authService.LoginAsync(loginDto);

        // Assert
        result.Should().NotBeNull();
        result.Token.Should().NotBeNullOrEmpty();
        result.User.Username.Should().Be("testuser");
        result.User.Role.Should().Be("buyer");
    }

    [Fact]
    public async Task LoginAsync_InvalidCredentials_ThrowsUnauthorizedException()
    {
        // Arrange
        var loginDto = new UserLoginDto { Username = "testuser", Password = "wrongpassword" };

        _userManagerMock.Setup(x => x.FindByNameAsync("testuser"))
            .ReturnsAsync((ApplicationUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(loginDto));
    }
}
