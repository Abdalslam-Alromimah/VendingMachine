// ChangeCalculatorServiceTests.cs
using FluentAssertions;
using VendingMachine.Application.Services;
using Xunit;

namespace VendingMachine.Tests.Services;

public class ChangeCalculatorServiceTests
{
    private readonly ChangeCalculatorService _service;

    public ChangeCalculatorServiceTests()
    {
        _service = new ChangeCalculatorService();
    }

    [Theory]
    [InlineData(0, new int[] { })]
    [InlineData(5, new int[] { 5 })]
    [InlineData(165, new int[] { 100, 50, 10, 5 })]
    [InlineData(275, new int[] { 100, 100, 50, 20, 5 })]
    public void CalculateChange_ValidAmount_ReturnsCorrectCoins(int amount, int[] expectedCoins)
    {
        // Act
        var result = _service.CalculateChange(amount);

        // Assert
        var totalValue = result.Sum(kvp => kvp.Key * kvp.Value);
        totalValue.Should().Be(amount);

        var resultCoins = new List<int>();
        foreach (var kvp in result.OrderByDescending(x => x.Key))
        {
            for (int i = 0; i < kvp.Value; i++)
            {
                resultCoins.Add(kvp.Key);
            }
        }

        resultCoins.Should().BeEquivalentTo(expectedCoins);
    }
}