// VendingMachineCommands.cs
using MediatR;
using VendingMachine.Domain.DTOs;

namespace VendingMachine.Application.Commands;

public record DepositCommand(string UserId, DepositDto Deposit) : IRequest<DepositResponseDto>;
public record BuyCommand(string UserId, BuyRequestDto BuyRequest) : IRequest<BuyResponseDto>;
public record ResetDepositCommand(string UserId) : IRequest;