// VendingMachineDbContext.cs
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using VendingMachine.Domain.Entities;

namespace VendingMachine.Infrastructure.Data;

public class VendingMachineDbContext : IdentityDbContext<ApplicationUser>
{
    public VendingMachineDbContext(DbContextOptions<VendingMachineDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        // Configure Product entity
        builder.Entity<Product>(entity =>
        {
            entity.HasKey(p => p.Id);

            entity.Property(p => p.ProductName)
                  .IsRequired()
                  .HasMaxLength(100);

            entity.Property(p => p.Cost)
                  .IsRequired();

            entity.Property(p => p.AmountAvailable)
                  .IsRequired();

            entity.Property(p => p.SellerId)
                  .IsRequired();

            entity.HasOne(p => p.Seller)
                  .WithMany(u => u.Products)
                  .HasForeignKey(p => p.SellerId)
                  .OnDelete(DeleteBehavior.Cascade);

            entity.HasIndex(p => p.ProductName);
            entity.HasIndex(p => p.SellerId);
        });

        // Configure ApplicationUser entity
        builder.Entity<ApplicationUser>(entity =>
        {
            entity.Property(u => u.Deposit)
                  .HasDefaultValue(0);
        });

        // Seed roles
        builder.Entity<Microsoft.AspNetCore.Identity.IdentityRole>().HasData(
            new Microsoft.AspNetCore.Identity.IdentityRole
            {
                Id = "1",
                Name = "buyer",
                NormalizedName = "BUYER"
            },
            new Microsoft.AspNetCore.Identity.IdentityRole
            {
                Id = "2",
                Name = "seller",
                NormalizedName = "SELLER"
            }
        );
    }
}