// UserRepository.cs
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using VendingMachine.Domain.Entities;
using VendingMachine.Domain.Exceptions;
using VendingMachine.Domain.Interfaces;
using VendingMachine.Infrastructure.Data;

namespace VendingMachine.Infrastructure.Repositories;

public class UserRepository : IUserRepository
{
    private readonly VendingMachineDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;

    public UserRepository(VendingMachineDbContext context, UserManager<ApplicationUser> userManager)
    {
        _context = context;
        _userManager = userManager;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<ApplicationUser?> GetByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users.ToListAsync();
    }

    public async Task<ApplicationUser> CreateAsync(ApplicationUser user, string password)
    {
        var result = await _userManager.CreateAsync(user, password);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new DomainException($"Failed to create user: {errors}");
        }
        return user;
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        var result = await _userManager.UpdateAsync(user);
        if (!result.Succeeded)
        {
            var errors = string.Join(", ", result.Errors.Select(e => e.Description));
            throw new DomainException($"Failed to update user: {errors}");
        }
    }

    public async Task DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user != null)
        {
            var result = await _userManager.DeleteAsync(user);
            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new DomainException($"Failed to delete user: {errors}");
            }
        }
    }

    public async Task<bool> ExistsAsync(string id)
    {
        return await _userManager.FindByIdAsync(id) != null;
    }

    public async Task UpdateDepositAsync(string userId, int newDeposit)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            throw new UserNotFoundException(userId);

        user.Deposit = newDeposit;
        await UpdateAsync(user);
    }
}