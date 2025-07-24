// Product.cs
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VendingMachine.Domain.Entities;

public class Product
{
    [Key]
    public int Id { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "Product name cannot exceed 100 characters")]
    public string ProductName { get; set; } = string.Empty;

    [Range(0, int.MaxValue, ErrorMessage = "Amount available must be non-negative")]
    public int AmountAvailable { get; set; }

    [Range(5, int.MaxValue, ErrorMessage = "Cost must be at least 5 cents")]
    public int Cost { get; set; }

    [Required]
    public string SellerId { get; set; } = string.Empty;

    [ForeignKey("SellerId")]
    public virtual ApplicationUser Seller { get; set; } = null!;
}