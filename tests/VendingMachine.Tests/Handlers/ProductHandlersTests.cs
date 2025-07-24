// ProductHandlersTests.cs
using FluentAssertions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Moq;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Handlers;
using VendingMachine.Application.Queries;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;
using Xunit;

namespace VendingMachine.Tests.Handlers;

public class ProductHandlersTests
{
    private readonly Mock<IProductRepository> _productRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<ILogger<CreateProductHandler>> _loggerMock;

    public ProductHandlersTests()
    {
        _productRepositoryMock = new Mock<IProductRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _loggerMock = new Mock<ILogger<CreateProductHandler>>();
    }

    [Fact]
    public async Task CreateProductHandler_ValidProduct_ReturnsProductDto()
    {
        // Arrange
        var handler = new CreateProductHandler(_productRepositoryMock.Object, _userRepositoryMock.Object, _loggerMock.Object);
        var seller = new ApplicationUser { Id = "seller1", UserName = "seller1" };
        var product = new Product
        {
            Id = 1,
            ProductName = "Test Product",
            AmountAvailable = 10,
            Cost = 100,
            SellerId = "seller1"
        };

        var command = new CreateProductCommand(
            new ProductCreateDto
            {
                ProductName = "Test Product",
                AmountAvailable = 10,
                Cost = 100
            },
            "seller1"
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync("seller1")).ReturnsAsync(seller);
        _productRepositoryMock.Setup(x => x.CreateAsync(It.IsAny<Product>())).ReturnsAsync(product);

        // Act
        var result = await handler.Handle(command, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.ProductName.Should().Be("Test Product");
        result.Cost.Should().Be(100);
        result.SellerId.Should().Be("seller1");
    }

    [Fact]
    public async Task CreateProductHandler_InvalidSeller_ThrowsUserNotFoundException()
    {
        // Arrange
        var handler = new CreateProductHandler(_productRepositoryMock.Object, _userRepositoryMock.Object, _loggerMock.Object);
        var command = new CreateProductCommand(
            new ProductCreateDto
            {
                ProductName = "Test Product",
                AmountAvailable = 10,
                Cost = 100
            },
            "invalid-seller"
        );

        _userRepositoryMock.Setup(x => x.GetByIdAsync("invalid-seller")).ReturnsAsync((ApplicationUser?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UserNotFoundException>(() => handler.Handle(command, CancellationToken.None));
    }
}

