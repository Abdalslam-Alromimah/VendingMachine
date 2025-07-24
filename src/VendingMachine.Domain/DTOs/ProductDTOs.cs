// ProductDTOs.cs
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Domain.DTOs;

public class ProductCreateDto
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string ProductName { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Amount available must be non-negative")]
    public int AmountAvailable { get; set; }

    [Range(5, int.MaxValue, ErrorMessage = "Cost must be at least 5 cents")]
    public int Cost { get; set; }
}

public class ProductUpdateDto
{
    [StringLength(100, MinimumLength = 1)]
    public string? ProductName { get; set; }

    [Range(0, int.MaxValue, ErrorMessage = "Amount available must be non-negative")]
    public int? AmountAvailable { get; set; }

    [Range(5, int.MaxValue, ErrorMessage = "Cost must be at least 5 cents")]
    public int? Cost { get; set; }
}

public class ProductDto
{
    public int Id { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int AmountAvailable { get; set; }
    public int Cost { get; set; }
    public string SellerId { get; set; } = string.Empty;
    public string SellerUsername { get; set; } = string.Empty;
}