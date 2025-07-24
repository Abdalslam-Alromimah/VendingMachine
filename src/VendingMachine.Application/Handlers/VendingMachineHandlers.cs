// VendingMachineHandlers.cs
using MediatR;
using Microsoft.Extensions.Logging;
using VendingMachine.Application.Commands;
using VendingMachine.Application.Services;
using VendingMachine.Domain.DTOs;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;
using VendingMachine.Domain.ValueObjects;

namespace VendingMachine.Application.Handlers;

public class DepositHandler : IRequestHandler<DepositCommand, DepositResponseDto>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<DepositHandler> _logger;

    public DepositHandler(IUserRepository userRepository, ILogger<DepositHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<DepositResponseDto> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing deposit for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        // Validate coin denominations
        foreach (var coin in request.Deposit.Coins)
        {
            if (!Coin.IsValidDenomination(coin.Key))
            {
                throw new InvalidCoinException(coin.Key);
            }

            if (coin.Value <= 0)
            {
                throw new DomainException($"Coin count must be positive for denomination {coin.Key}");
            }
        }

        // Calculate deposit amount
        var depositAmount = Coin.CalculateTotal(request.Deposit.Coins);
        var newDeposit = user.Deposit + depositAmount;

        await _userRepository.UpdateDepositAsync(request.UserId, newDeposit);

        _logger.LogInformation("Deposit successful for user: {UserId}. Amount: {Amount}, New total: {NewTotal}",
            request.UserId, depositAmount, newDeposit);

        return new DepositResponseDto
        {
            NewDeposit = newDeposit,
            Message = $"Successfully deposited {depositAmount} cents. New balance: {newDeposit} cents."
        };
    }
}

public class BuyHandler : IRequestHandler<BuyCommand, BuyResponseDto>
{
    private readonly IVendingMachineService _vendingMachineService;
    private readonly ILogger<BuyHandler> _logger;

    public BuyHandler(IVendingMachineService vendingMachineService, ILogger<BuyHandler> logger)
    {
        _vendingMachineService = vendingMachineService;
        _logger = logger;
    }

    public async Task<BuyResponseDto> Handle(BuyCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing purchase for user: {UserId}, Product: {ProductId}, Amount: {Amount}",
            request.UserId, request.BuyRequest.ProductId, request.BuyRequest.Amount);

        var result = await _vendingMachineService.BuyProductAsync(
            request.UserId,
            request.BuyRequest.ProductId,
            request.BuyRequest.Amount);

        _logger.LogInformation("Purchase successful for user: {UserId}. Total spent: {TotalSpent}",
            request.UserId, result.TotalSpent);

        return result;
    }
}

public class ResetDepositHandler : IRequestHandler<ResetDepositCommand>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<ResetDepositHandler> _logger;

    public ResetDepositHandler(IUserRepository userRepository, ILogger<ResetDepositHandler> logger)
    {
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task Handle(ResetDepositCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Resetting deposit for user: {UserId}", request.UserId);

        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            throw new UserNotFoundException(request.UserId);
        }

        await _userRepository.UpdateDepositAsync(request.UserId, 0);

        _logger.LogInformation("Deposit reset successful for user: {UserId}", request.UserId);
    }
}