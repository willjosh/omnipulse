using Domain.Entities;

using Microsoft.AspNetCore.Identity;

using Persistence.DatabaseContext;

namespace Persistence.Seeding.Contracts;

/// <summary>
/// User seeding operations. Requires user management services.
/// </summary>
public interface IUserSeeder
{
    /// <summary>
    /// Seeds user and role data into the database.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <param name="userManager">The user manager for creating users</param>
    /// <param name="roleManager">The role manager for creating roles</param>
    /// <returns>A task representing the asynchronous seeding operation</returns>
    Task SeedAsync(OmnipulseDatabaseContext context, UserManager<User> userManager, RoleManager<IdentityRole> roleManager);
}