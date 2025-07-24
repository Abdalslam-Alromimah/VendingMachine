

// VendingMachineDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Domain.DTOs;

public class DepositDto
{
    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "At least one coin must be provided")]
    public Dictionary<int, int> Coins { get; set; } = new();
}

public class BuyRequestDto
{
    [Range(1, int.MaxValue, ErrorMessage = "Product ID must be positive")]
    public int ProductId { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Amount must be at least 1")]
    public int Amount { get; set; }
}

public class BuyResponseDto
{
    public int TotalSpent { get; set; }
    public List<ProductDto> ProductsPurchased { get; set; } = new();
    public Dictionary<int, int> Change { get; set; } = new();
}

public class DepositResponseDto
{
    public int NewDeposit { get; set; }
    public string Message { get; set; } = string.Empty;
}