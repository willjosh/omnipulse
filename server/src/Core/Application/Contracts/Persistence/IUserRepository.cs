using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;

namespace Application.Contracts.Persistence;

public interface IUserRepository
{
    // Basic CRUD operations (similar to generic but for Identity User)
    Task<User?> GetByIdAsync(string id);
    Task<IReadOnlyList<User>> GetAllAsync();
    Task<IdentityResult> AddAsync(User user, string password);
    Task<IdentityResult> UpdateAsync(User user);
    Task<IdentityResult> DeleteAsync(string id);
    Task<IdentityResult> AddAsyncWithRole(User user, string password, string role);
    Task<IdentityResult> DeleteAsync(User user);

    // Identity-specific operations
    Task<User?> GetByEmailAsync(string email);
    Task<User?> GetByUsernameAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> CheckPasswordAsync(User user, string password);
    Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword);
    Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword);

    // Role operations
    Task<IdentityResult> AddToRoleAsync(User user, string role);
    Task<IdentityResult> RemoveFromRoleAsync(User user, string role);
    Task<IList<string>> GetRolesAsync(User user);
    Task<bool> IsInRoleAsync(User user, string role);
    Task<IReadOnlyList<User>> GetUsersInRoleAsync(string role);

    // Query operations
    Task<IReadOnlyList<User>> FindAsync(Expression<Func<User, bool>> predicate);
    Task<User?> GetFirstOrDefaultAsync(Expression<Func<User, bool>> predicate);
    Task<bool> ExistsAsync(string id);
    Task<int> CountAsync();

    // Token operations
    Task<string> GenerateEmailConfirmationTokenAsync(User user);
    Task<string> GeneratePasswordResetTokenAsync(User user);
    Task<IdentityResult> ConfirmEmailAsync(User user, string token);

    // Status operations
    Task<IReadOnlyList<User>> GetActiveUsersAsync();
    Task<IReadOnlyList<User>> GetInactiveUsersAsync();
    Task<IdentityResult> LockUserAsync(User user, DateTimeOffset? lockoutEnd);
    Task<IdentityResult> UnlockUserAsync(User user);
}