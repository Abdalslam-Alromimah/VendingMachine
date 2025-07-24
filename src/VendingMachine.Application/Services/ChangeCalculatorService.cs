// ChangeCalculatorService.cs
using VendingMachine.Domain.ValueObjects;

namespace VendingMachine.Application.Services;

public class ChangeCalculatorService : IChangeCalculatorService
{
    public Dictionary<int, int> CalculateChange(int amount)
    {
        return Coin.CalculateChange(amount);
    }
}