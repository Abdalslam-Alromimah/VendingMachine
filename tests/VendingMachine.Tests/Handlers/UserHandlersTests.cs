// UserHandlersTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Handlers;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;
using Xunit;

namespace VendingMachine.Tests.Handlers;

public class UserHandlersTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<UserManager<ApplicationUser>> _userManagerMock;
    private readonly Mock<ILogger<CreateUserHandler>> _loggerMock;

    public UserHandlersTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        var store = new Mock<IUserStore<ApplicationUser>>();
        _userManagerMock = new Mock<UserManager<ApplicationUser>>(
            store.Object, null, null, null, null, null, null, null, null);
        _loggerMock = new Mock<ILogger<CreateUserHandler>>();
    }

    [Fact]
    public async Task CreateUserHandler_ValidUser_ReturnsUserDto()
    {
        // Arrange
        var handler = new CreateUserHandler(_userRepositoryMock.Object, _userManagerMock.Object, _loggerMock.Object);
        var user = new ApplicationUser { Id = "user1", UserName = "testuser", Deposit = 0 };
        var command = new CreateUserCommand(new UserRegistrationDto
        {
            Username = "testuser",
            Password = "Test123!",
            Role = "buyer"
        });

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync("testuser")).ReturnsAsync((ApplicationUser?)null);
        _userRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<ApplicationUser>(), "Test123!")).ReturnsAsync(user);
        _userManagerMock.Setup(x => x.AddToRoleAsync(user, "buyer")).ReturnsAsync(IdentityResult.Success);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Username.Should().Be("testuser");
        result.Role.Should().Be("buyer");
    }

    [Fact]
    public async Task CreateUserHandler_DuplicateUsername_ThrowsDuplicateUsernameException()
    {
        // Arrange
        var handler = new CreateUserHandler(_userRepositoryMock.Object, _userManagerMock.Object, _loggerMock.Object);
        var existingUser = new ApplicationUser { Id = "existing", UserName = "testuser" };
        var command = new CreateUserCommand(new UserRegistrationDto
        {
            Username = "testuser",
            Password = "Test123!",
            Role = "buyer"
        });

        _userRepositoryMock.Setup(x => x.GetByUsernameAsync("testuser")).ReturnsAsync(existingUser);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateUsernameException>(() => handler.Handle(command, CancellationToken.None));
    }
}