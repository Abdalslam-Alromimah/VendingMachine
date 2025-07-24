// IUserRepository.cs
using VendingMachine.Domain.Entities;

namespace VendingMachine.Domain.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByUsernameAsync(string username);
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task<ApplicationUser> CreateAsync(ApplicationUser user, string password);
    Task UpdateAsync(ApplicationUser user);
    Task DeleteAsync(string id);
    Task<bool> ExistsAsync(string id);
    Task UpdateDepositAsync(string userId, int newDeposit);
}

