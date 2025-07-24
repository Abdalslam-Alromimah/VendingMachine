// IChangeCalculatorService.cs
using VendingMachine.Domain.ValueObjects;

namespace VendingMachine.Application.Services;

public interface IChangeCalculatorService
{
    Dictionary<int, int> CalculateChange(int amount);
}

