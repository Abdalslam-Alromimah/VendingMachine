// ProductRepository.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;
using VendingMachine.Infrastructure.Data;

namespace VendingMachine.Infrastructure.Repositories;

public class ProductRepository : IProductRepository
{
    private readonly VendingMachineDbContext _context;

    public ProductRepository(VendingMachineDbContext context)
    {
        _context = context;
    }

    public async Task<Product?> GetByIdAsync(int id)
    {
        return await _context.Products
            .Include(p => p.Seller)
            .FirstOrDefaultAsync(p => p.Id == id);
    }

    public async Task<IEnumerable<Product>> GetAllAsync()
    {
        return await _context.Products
            .Include(p => p.Seller)
            .ToListAsync();
    }

    public async Task<IEnumerable<Product>> GetBySellerIdAsync(string sellerId)
    {
        return await _context.Products
            .Include(p => p.Seller)
            .Where(p => p.SellerId == sellerId)
            .ToListAsync();
    }

    public async Task<Product> CreateAsync(Product product)
    {
        _context.Products.Add(product);
        await _context.SaveChangesAsync();
        return await GetByIdAsync(product.Id) ?? product;
    }

    public async Task UpdateAsync(Product product)
    {
        _context.Entry(product).State = EntityState.Modified;
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<bool> ExistsAsync(int id)
    {
        return await _context.Products.AnyAsync(p => p.Id == id);
    }

    public async Task<bool> IsOwnerAsync(int productId, string sellerId)
    {
        return await _context.Products
            .AnyAsync(p => p.Id == productId && p.SellerId == sellerId);
    }
}

