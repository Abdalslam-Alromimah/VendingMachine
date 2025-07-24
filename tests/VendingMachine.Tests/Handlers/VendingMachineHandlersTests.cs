// VendingMachineHandlersTests.cs
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Handlers;
using VendingMachine.Application.Services;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;
using Xunit;

namespace VendingMachine.Tests.Handlers;

public class VendingMachineHandlersTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<DepositHandler>> _loggerMock;

    public VendingMachineHandlersTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<DepositHandler>>();
    }

    [Fact]
    public async Task DepositHandler_ValidCoins_UpdatesUserDeposit()
    {
        // Arrange
        var handler = new DepositHandler(_userRepositoryMock.Object, _loggerMock.Object);
        var user = new ApplicationUser { Id = "user1", Deposit = 100 };
        var command = new DepositCommand("user1", new DepositDto
        {
            Coins = new Dictionary<int, int> { { 100, 1 }, { 50, 2 } }
        });

        _userRepositoryMock.Setup(x => x.GetByIdAsync("user1")).ReturnsAsync(user);
        _userRepositoryMock.Setup(x => x.UpdateDepositAsync("user1", 300)).Returns(Task.CompletedTask);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.NewDeposit.Should().Be(300);
        _userRepositoryMock.Verify(x => x.UpdateDepositAsync("user1", 300), Times.Once);
    }

    [Fact]
    public async Task DepositHandler_InvalidCoin_ThrowsInvalidCoinException()
    {
        // Arrange
        var handler = new DepositHandler(_userRepositoryMock.Object, _loggerMock.Object);
        var user = new ApplicationUser { Id = "user1", Deposit = 100 };
        var command = new DepositCommand("user1", new DepositDto
        {
            Coins = new Dictionary<int, int> { { 15, 1 } } // Invalid denomination
        });

        _userRepositoryMock.Setup(x => x.GetByIdAsync("user1")).ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<InvalidCoinException>(() => handler.Handle(command, CancellationToken.None));
    }
}

