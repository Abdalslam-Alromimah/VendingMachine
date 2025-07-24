// Coin.cs
namespace VendingMachine.Domain.ValueObjects;

public static class Coin
{
    public static readonly int[] ValidDenominations = { 5, 10, 20, 50, 100 };

    public static bool IsValidDenomination(int value)
    {
        return ValidDenominations.Contains(value);
    }

    public static Dictionary<int, int> CalculateChange(int amount)
    {
        var change = new Dictionary<int, int>();
        var denominations = ValidDenominations.OrderByDescending(x => x).ToArray();

        foreach (var denomination in denominations)
        {
            if (amount >= denomination)
            {
                var count = amount / denomination;
                change[denomination] = count;
                amount %= denomination;
            }
        }

        return change;
    }

    public static int CalculateTotal(Dictionary<int, int> coins)
    {
        return coins.Sum(kvp => kvp.Key * kvp.Value);
    }
}