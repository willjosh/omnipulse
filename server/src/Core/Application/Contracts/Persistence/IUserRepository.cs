using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;

namespace Application.Contracts.Persistence;

public interface IUserRepository
{
    // Basic CRUD operations (similar to generic but for Identity User)
    Task<IdentityUser?> GetByIdAsync(string id);
    Task<IReadOnlyList<IdentityUser>> GetAllAsync();
    Task<IdentityResult> AddAsync(IdentityUser user, string password);
    Task<IdentityResult> UpdateAsync(IdentityUser user);
    Task<IdentityResult> DeleteAsync(string id);
    Task<IdentityResult> DeleteAsync(IdentityUser user);
    
    // Identity-specific operations
    Task<IdentityUser?> GetByEmailAsync(string email);
    Task<IdentityUser?> GetByUsernameAsync(string username);
    Task<bool> EmailExistsAsync(string email);
    Task<bool> UsernameExistsAsync(string username);
    Task<bool> CheckPasswordAsync(IdentityUser user, string password);
    Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword);
    Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPassword);
    
    // Role operations
    Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role);
    Task<IdentityResult> RemoveFromRoleAsync(IdentityUser user, string role);
    Task<IList<string>> GetRolesAsync(IdentityUser user);
    Task<bool> IsInRoleAsync(IdentityUser user, string role);
    Task<IReadOnlyList<IdentityUser>> GetUsersInRoleAsync(string role);
    
    // Query operations
    Task<IReadOnlyList<IdentityUser>> FindAsync(Expression<Func<IdentityUser, bool>> predicate);
    Task<IdentityUser?> GetFirstOrDefaultAsync(Expression<Func<IdentityUser, bool>> predicate);
    Task<bool> ExistsAsync(string id);
    Task<int> CountAsync();
    
    // Token operations
    Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user);
    Task<string> GeneratePasswordResetTokenAsync(IdentityUser user);
    Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token);
    
    // Status operations
    Task<IReadOnlyList<IdentityUser>> GetActiveUsersAsync();
    Task<IReadOnlyList<IdentityUser>> GetInactiveUsersAsync();
    Task<IdentityResult> LockUserAsync(IdentityUser user, DateTimeOffset? lockoutEnd);
    Task<IdentityResult> UnlockUserAsync(IdentityUser user);
}