// SeedData.cs
using Microsoft.AspNetCore.Identity;
using VendingMachine.Domain.Entities;
using VendingMachine.Infrastructure.Data;

namespace VendingMachine.Infrastructure.Data;

public static class SeedData
{
    public static async Task InitializeAsync(
        VendingMachineDbContext context,
        UserManager<ApplicationUser> userManager,
        RoleManager<IdentityRole> roleManager)
    {
        // Seed roles
        await SeedRolesAsync(roleManager);

        // Seed users
        await SeedUsersAsync(userManager);

        // Seed products
        await SeedProductsAsync(context, userManager);
    }

    private static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager)
    {
        string[] roles = { "buyer", "seller" };

        foreach (var role in roles)
        {
            if (!await roleManager.RoleExistsAsync(role))
            {
                await roleManager.CreateAsync(new IdentityRole(role));
            }
        }
    }

    private static async Task SeedUsersAsync(UserManager<ApplicationUser> userManager)
    {
        // Seed seller
        if (await userManager.FindByNameAsync("seller1") == null)
        {
            var seller = new ApplicationUser
            {
                UserName = "seller1",
                Email = "seller1@example.com",
                Deposit = 0
            };

            await userManager.CreateAsync(seller, "Seller123!");
            await userManager.AddToRoleAsync(seller, "seller");
        }

        // Seed buyer
        if (await userManager.FindByNameAsync("buyer1") == null)
        {
            var buyer = new ApplicationUser
            {
                UserName = "buyer1",
                Email = "buyer1@example.com",
                Deposit = 200 // Start with some deposit for testing
            };

            await userManager.CreateAsync(buyer, "Buyer123!");
            await userManager.AddToRoleAsync(buyer, "buyer");
        }
    }

    private static async Task SeedProductsAsync(
        VendingMachineDbContext context,
        UserManager<ApplicationUser> userManager)
    {
        if (!context.Products.Any())
        {
            var seller = await userManager.FindByNameAsync("seller1");
            if (seller != null)
            {
                var products = new[]
                {
                    new Product
                    {
                        ProductName = "Coca Cola",
                        AmountAvailable = 10,
                        Cost = 100,
                        SellerId = seller.Id
                    },
                    new Product
                    {
                        ProductName = "Pepsi",
                        AmountAvailable = 15,
                        Cost = 95,
                        SellerId = seller.Id
                    },
                    new Product
                    {
                        ProductName = "Water",
                        AmountAvailable = 20,
                        Cost = 50,
                        SellerId = seller.Id
                    },
                    new Product
                    {
                        ProductName = "Snickers",
                        AmountAvailable = 8,
                        Cost = 120,
                        SellerId = seller.Id
                    },
                    new Product
                    {
                        ProductName = "Chips",
                        AmountAvailable = 12,
                        Cost = 75,
                        SellerId = seller.Id
                    }
                };

                context.Products.AddRange(products);
                await context.SaveChangesAsync();
            }
        }
    }
}