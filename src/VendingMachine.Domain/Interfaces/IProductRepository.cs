// IProductRepository.cs
using VendingMachine.Domain.Entities;

namespace VendingMachine.Domain.Interfaces;

public interface IProductRepository
{
    Task<Product?> GetByIdAsync(int id);
    Task<IEnumerable<Product>> GetAllAsync();
    Task<IEnumerable<Product>> GetBySellerIdAsync(string sellerId);
    Task<Product> CreateAsync(Product product);
    Task UpdateAsync(Product product);
    Task DeleteAsync(int id);
    Task<bool> ExistsAsync(int id);
    Task<bool> IsOwnerAsync(int productId, string sellerId);
}