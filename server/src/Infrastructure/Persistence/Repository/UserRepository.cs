using System.Linq.Expressions;

using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class UserRepository : IUserRepository
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly OmnipulseDatabaseContext _context;

    public UserRepository(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, OmnipulseDatabaseContext context)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _context = context;
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

    public async Task<User?> GetTechnicianByIdAsync(string id)
    {
        // Get user by ID first (single database query)
        var user = await _userManager.FindByIdAsync(id);

        if (user == null)
            return null;

        // Check if user has technician role (single database query)
        var isInRole = await _userManager.IsInRoleAsync(user, "TECHNICIAN");

        return isInRole ? user : null;
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

    public async Task<IdentityResult> AddAsyncWithRole(User user, string password, string role)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Try to create new user
            var result = await _userManager.CreateAsync(user, password);

            // If result fails, return result (no rollback needed since nothing was committed)
            if (!result.Succeeded)
            {
                return result;
            }

            // Assign role to user
            var roleAssignResult = await _userManager.AddToRoleAsync(user, role);

            // If role assignment fails, rollback transaction and return error
            if (!roleAssignResult.Succeeded)
            {
                await transaction.RollbackAsync();

                // Include original role assignment errors for better debugging
                var combinedErrors = new List<IdentityError>
                {
                    new() { Code = "RoleAssignmentFailed", Description = $"Failed to assign role '{role}' to user." }
                };
                combinedErrors.AddRange(roleAssignResult.Errors);

                return IdentityResult.Failed([.. combinedErrors]);
            }

            // Both operations succeeded, commit the transaction
            await transaction.CommitAsync();
            return IdentityResult.Success;
        }
        catch (Exception ex)
        {
            // Rollback on any unexpected error
            try
            {
                await transaction.RollbackAsync();
            }
            catch
            {
                // Ignore rollback errors - transaction might already be rolled back
            }

            return IdentityResult.Failed(new IdentityError
            {
                Code = "UnexpectedError",
                Description = $"An unexpected error occurred while creating user with role: {ex.Message}"
            });
        }
    }

    public async Task<PagedResult<User>> GetAllTechnicianPagedAsync(PaginationParameters parameters)
    {
        var technicianUsers = await _userManager.GetUsersInRoleAsync("TECHNICIAN");

        var query = technicianUsers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();
            query = query.Where(u =>
                u.FirstName.ToLowerInvariant().Contains(search) ||
                u.LastName.ToLowerInvariant().Contains(search) ||
                u.Email!.ToLowerInvariant().Contains(search)
            );
        }
        ;

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<User>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize,
        };
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

    private IQueryable<User> ApplySorting(IQueryable<User> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "firstname" => sortDescending ? query.OrderByDescending(u => u.FirstName) : query.OrderBy(u => u.FirstName),
            "lastname" => sortDescending ? query.OrderByDescending(u => u.LastName) : query.OrderBy(u => u.LastName),
            "email" => sortDescending ? query.OrderByDescending(u => u.Email) : query.OrderBy(u => u.Email),
            "username" => sortDescending ? query.OrderByDescending(u => u.UserName) : query.OrderBy(u => u.UserName),
            "hiredate" => sortDescending ? query.OrderByDescending(u => u.HireDate) : query.OrderBy(u => u.HireDate),
            _ => query.OrderBy(u => u.HireDate)
        };
    }
}