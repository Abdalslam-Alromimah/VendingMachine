// VendingMachineService.cs
using Microsoft.Extensions.Logging;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;

namespace VendingMachine.Application.Services;

public class VendingMachineService : IVendingMachineService
{
    private readonly IUserRepository _userRepository;
    private readonly IProductRepository _productRepository;
    private readonly IChangeCalculatorService _changeCalculatorService;
    private readonly ILogger<VendingMachineService> _logger;

    public VendingMachineService(
        IUserRepository userRepository,
        IProductRepository productRepository,
        IChangeCalculatorService changeCalculatorService,
        ILogger<VendingMachineService> logger)
    {
        _userRepository = userRepository;
        _productRepository = productRepository;
        _changeCalculatorService = changeCalculatorService;
        _logger = logger;
    }

    public async Task<BuyResponseDto> BuyProductAsync(string userId, int productId, int amount)
    {
        // Get user
        var user = await _userRepository.GetByIdAsync(userId);
        if (user == null)
        {
            throw new UserNotFoundException(userId);
        }

        // Get product
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            throw new ProductNotFoundException(productId);
        }

        // Check availability
        if (product.AmountAvailable < amount)
        {
            throw new InsufficientStockException(product.ProductName, amount, product.AmountAvailable);
        }

        // Calculate total cost
        var totalCost = product.Cost * amount;

        // Check user has enough funds
        if (user.Deposit < totalCost)
        {
            throw new InsufficientFundsException(totalCost, user.Deposit);
        }

        // Calculate change
        var changeAmount = user.Deposit - totalCost;
        var change = _changeCalculatorService.CalculateChange(changeAmount);

        // Update product stock
        product.AmountAvailable -= amount;
        await _productRepository.UpdateAsync(product);

        // Update user deposit (reset to 0 after purchase)
        await _userRepository.UpdateDepositAsync(userId, 0);

        var productsPurchased = new List<ProductDto>();
        for (int i = 0; i < amount; i++)
        {
            productsPurchased.Add(new ProductDto
            {
                Id = product.Id,
                ProductName = product.ProductName,
                AmountAvailable = product.AmountAvailable,
                Cost = product.Cost,
                SellerId = product.SellerId,
                SellerUsername = product.Seller?.UserName ?? "Unknown"
            });
        }

        _logger.LogInformation("Purchase completed. User: {UserId}, Product: {ProductId}, Amount: {Amount}, Total Cost: {TotalCost}",
            userId, productId, amount, totalCost);

        return new BuyResponseDto
        {
            TotalSpent = totalCost,
            ProductsPurchased = productsPurchased,
            Change = change
        };
    }
}