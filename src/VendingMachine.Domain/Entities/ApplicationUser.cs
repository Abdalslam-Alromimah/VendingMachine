// ApplicationUser.cs
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace VendingMachine.Domain.Entities;

public class ApplicationUser : IdentityUser
{
    [Range(0, int.MaxValue, ErrorMessage = "Deposit must be non-negative")]
    public int Deposit { get; set; } = 0;

    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
