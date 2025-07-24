// IVendingMachineService.cs
using VendingMachine.Domain.DTOs;

namespace VendingMachine.Application.Services;

public interface IVendingMachineService
{
    Task<BuyResponseDto> BuyProductAsync(string userId, int productId, int amount);
}