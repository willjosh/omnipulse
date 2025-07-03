using System;
using System.Linq;
using System.Linq.Expressions;

using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Persistence.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;

    public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
    {
        _userManager = userManager;
        _roleManager = roleManager;
    }

    // Basic CRUD Operations
    public async Task<IdentityResult> AddAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<IdentityResult> UpdateAsync(User user)
    {
        return await _userManager.UpdateAsync(user);
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> DeleteAsync(User user)
    {
        return await _userManager.DeleteAsync(user);
    }

    // Query Operations
    public async Task<User?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users.Cast<User>().ToList();
    }

    public async Task<User?> GetFirstOrDefaultAsync(Expression<Func<User, bool>> predicate)
    {
        var userPredicate = ConvertPredicate(predicate);
        return await _userManager.Users.FirstOrDefaultAsync(userPredicate);
    }

    public async Task<IReadOnlyList<User>> FindAsync(Expression<Func<User, bool>> predicate)
    {
        var userPredicate = ConvertPredicate(predicate);
        var users = await _userManager.Users.Where(userPredicate).ToListAsync();
        return users.Cast<User>().ToList();
    }

    public async Task<int> CountAsync()
    {
        return await _userManager.Users.CountAsync();
    }

    // Existence Checks
    public async Task<bool> ExistsAsync(string id)
    {
        return await _userManager.FindByIdAsync(id) != null;
    }

    public async Task<bool> EmailExistsAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email) != null;
    }

    public async Task<bool> UsernameExistsAsync(string username)
    {
        return await _userManager.FindByNameAsync(username) != null;
    }

    // Password Operations
    public async Task<bool> CheckPasswordAsync(User user, string password)
    {
        return await _userManager.CheckPasswordAsync(user, password);
    }

    public async Task<IdentityResult> ChangePasswordAsync(User user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync(user, currentPassword, newPassword);
    }

    public async Task<IdentityResult> ResetPasswordAsync(User user, string token, string newPassword)
    {
        return await _userManager.ResetPasswordAsync(user, token, newPassword);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(User user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync(user);
    }

    // Email Operations
    public async Task<string> GenerateEmailConfirmationTokenAsync(User user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync(user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(User user, string token)
    {
        return await _userManager.ConfirmEmailAsync(user, token);
    }

    // Role Operations
    public async Task<IdentityResult> AddToRoleAsync(User user, string role)
    {
        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        return await _userManager.AddToRoleAsync(user, role);
    }

    public async Task<IdentityResult> RemoveFromRoleAsync(User user, string role)
    {
        return await _userManager.RemoveFromRoleAsync(user, role);
    }

    public async Task<IList<string>> GetRolesAsync(User user)
    {
        return await _userManager.GetRolesAsync(user);
    }

    public async Task<bool> IsInRoleAsync(User user, string role)
    {
        return await _userManager.IsInRoleAsync(user, role);
    }

    public async Task<IReadOnlyList<User>> GetUsersInRoleAsync(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        return users.Cast<User>().ToList();
    }

    // Lockout Operations
    public async Task<IdentityResult> LockUserAsync(User user, DateTimeOffset? lockoutEnd)
    {
        return await _userManager.SetLockoutEndDateAsync(user, lockoutEnd);
    }

    public async Task<IdentityResult> UnlockUserAsync(User user)
    {
        return await _userManager.SetLockoutEndDateAsync(user, null);
    }

    // Custom Business Logic Methods
    public async Task<IReadOnlyList<User>> GetActiveUsersAsync()
    {
        var users = await _userManager.Users
            .Where(u => u.IsActive && (u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow))
            .ToListAsync();
        return users.Cast<User>().ToList();
    }

    public async Task<IReadOnlyList<User>> GetInactiveUsersAsync()
    {
        var users = await _userManager.Users
            .Where(u => !u.IsActive || (u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow))
            .ToListAsync();
        return users.Cast<User>().ToList();
    }

    // Helper method to convert Expression<Func<User, bool>> to Expression<Func<User, bool>>
    private Expression<Func<User, bool>> ConvertPredicate(Expression<Func<User, bool>> predicate)
    {
        var parameter = Expression.Parameter(typeof(User), predicate.Parameters[0].Name);
        var body = new ParameterReplacementVisitor(predicate.Parameters[0], parameter).Visit(predicate.Body);
        return Expression.Lambda<Func<User, bool>>(body, parameter);
    }

    // Helper class for expression conversion
    private class ParameterReplacementVisitor : ExpressionVisitor
    {
        private readonly ParameterExpression _oldParameter;
        private readonly ParameterExpression _newParameter;

        public ParameterReplacementVisitor(ParameterExpression oldParameter, ParameterExpression newParameter)
        {
            _oldParameter = oldParameter;
            _newParameter = newParameter;
        }

        protected override Expression VisitParameter(ParameterExpression node)
        {
            return node == _oldParameter ? _newParameter : base.VisitParameter(node);
        }
    }
}