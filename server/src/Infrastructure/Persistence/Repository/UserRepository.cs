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
    public async Task<IdentityResult> AddAsync(IdentityUser user, string password)
    {
        return await _userManager.CreateAsync((User)user, password);
    }

    public async Task<IdentityResult> UpdateAsync(IdentityUser user)
    {
        return await _userManager.UpdateAsync((User)user);
    }

    public async Task<IdentityResult> DeleteAsync(string id)
    {
        var user = await _userManager.FindByIdAsync(id);
        if (user == null)
            return IdentityResult.Failed(new IdentityError { Description = "User not found" });

        return await _userManager.DeleteAsync(user);
    }

    public async Task<IdentityResult> DeleteAsync(IdentityUser user)
    {
        return await _userManager.DeleteAsync((User)user);
    }

    // Query Operations
    public async Task<IdentityUser?> GetByIdAsync(string id)
    {
        return await _userManager.FindByIdAsync(id);
    }

    public async Task<IdentityUser?> GetByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<IdentityUser?> GetByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }

    public async Task<IReadOnlyList<IdentityUser>> GetAllAsync()
    {
        var users = await _userManager.Users.ToListAsync();
        return users.Cast<IdentityUser>().ToList();
    }

    public async Task<IdentityUser?> GetFirstOrDefaultAsync(Expression<Func<IdentityUser, bool>> predicate)
    {
        var userPredicate = ConvertPredicate(predicate);
        return await _userManager.Users.FirstOrDefaultAsync(userPredicate);
    }

    public async Task<IReadOnlyList<IdentityUser>> FindAsync(Expression<Func<IdentityUser, bool>> predicate)
    {
        var userPredicate = ConvertPredicate(predicate);
        var users = await _userManager.Users.Where(userPredicate).ToListAsync();
        return users.Cast<IdentityUser>().ToList();
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
    public async Task<bool> CheckPasswordAsync(IdentityUser user, string password)
    {
        return await _userManager.CheckPasswordAsync((User)user, password);
    }

    public async Task<IdentityResult> ChangePasswordAsync(IdentityUser user, string currentPassword, string newPassword)
    {
        return await _userManager.ChangePasswordAsync((User)user, currentPassword, newPassword);
    }

    public async Task<IdentityResult> ResetPasswordAsync(IdentityUser user, string token, string newPassword)
    {
        return await _userManager.ResetPasswordAsync((User)user, token, newPassword);
    }

    public async Task<string> GeneratePasswordResetTokenAsync(IdentityUser user)
    {
        return await _userManager.GeneratePasswordResetTokenAsync((User)user);
    }

    // Email Operations
    public async Task<string> GenerateEmailConfirmationTokenAsync(IdentityUser user)
    {
        return await _userManager.GenerateEmailConfirmationTokenAsync((User)user);
    }

    public async Task<IdentityResult> ConfirmEmailAsync(IdentityUser user, string token)
    {
        return await _userManager.ConfirmEmailAsync((User)user, token);
    }

    // Role Operations
    public async Task<IdentityResult> AddToRoleAsync(IdentityUser user, string role)
    {
        // Ensure role exists
        if (!await _roleManager.RoleExistsAsync(role))
        {
            await _roleManager.CreateAsync(new IdentityRole(role));
        }

        return await _userManager.AddToRoleAsync((User)user, role);
    }

    public async Task<IdentityResult> RemoveFromRoleAsync(IdentityUser user, string role)
    {
        return await _userManager.RemoveFromRoleAsync((User)user, role);
    }

    public async Task<IList<string>> GetRolesAsync(IdentityUser user)
    {
        return await _userManager.GetRolesAsync((User)user);
    }

    public async Task<bool> IsInRoleAsync(IdentityUser user, string role)
    {
        return await _userManager.IsInRoleAsync((User)user, role);
    }

    public async Task<IReadOnlyList<IdentityUser>> GetUsersInRoleAsync(string role)
    {
        var users = await _userManager.GetUsersInRoleAsync(role);
        return users.Cast<IdentityUser>().ToList();
    }

    // Lockout Operations
    public async Task<IdentityResult> LockUserAsync(IdentityUser user, DateTimeOffset? lockoutEnd)
    {
        return await _userManager.SetLockoutEndDateAsync((User)user, lockoutEnd);
    }

    public async Task<IdentityResult> UnlockUserAsync(IdentityUser user)
    {
        return await _userManager.SetLockoutEndDateAsync((User)user, null);
    }

    // Custom Business Logic Methods
    public async Task<IReadOnlyList<IdentityUser>> GetActiveUsersAsync()
    {
        var users = await _userManager.Users
            .Where(u => u.IsActive && (u.LockoutEnd == null || u.LockoutEnd <= DateTimeOffset.UtcNow))
            .ToListAsync();
        return users.Cast<IdentityUser>().ToList();
    }

    public async Task<IReadOnlyList<IdentityUser>> GetInactiveUsersAsync()
    {
        var users = await _userManager.Users
            .Where(u => !u.IsActive || (u.LockoutEnd != null && u.LockoutEnd > DateTimeOffset.UtcNow))
            .ToListAsync();
        return users.Cast<IdentityUser>().ToList();
    }

    // Helper method to convert Expression<Func<IdentityUser, bool>> to Expression<Func<User, bool>>
    private Expression<Func<User, bool>> ConvertPredicate(Expression<Func<IdentityUser, bool>> predicate)
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