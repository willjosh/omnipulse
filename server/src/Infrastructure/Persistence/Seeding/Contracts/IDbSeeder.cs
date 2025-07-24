using Domain.Entities;

using Microsoft.AspNetCore.Identity;

using Persistence.DatabaseContext;

namespace Persistence.Seeding.Contracts;

/// <summary>
/// Main database seeder that orchestrates all seeding operations.
/// </summary>
public interface IDbSeeder
{
    /// <summary>
    /// Seeds the database.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="userManager">User manager for creating users</param>
    /// <param name="roleManager">Role manager for creating roles</param>
    /// <returns>A task representing the asynchronous seeding operation</returns>
    Task SeedAsync(OmnipulseDatabaseContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager);
}