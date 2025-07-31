using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class UserSeeder : IEntitySeeder
{
    private const int SeedCount = 3;
    private const string DefaultRole = "Technician";

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<User> _userDbSet;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger<UserSeeder> _logger;

    public UserSeeder(
        OmnipulseDatabaseContext context,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<UserSeeder> logger)
    {
        _dbContext = context;
        _userDbSet = context.Users;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    private List<User> CreateUsers()
    {
        var now = DateTime.UtcNow;
        var users = new List<User>();

        for (int i = 1; i <= SeedCount; i++)
        {
            users.Add(new User
            {
                UserName = $"user{i}",
                Email = $"user{i}@example.com",
                EmailConfirmed = true,
                FirstName = $"First{i}",
                LastName = $"Last{i}",
                HireDate = now.AddYears(-i),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                MaintenanceHistories = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                VehicleInspections = [],
                Vehicles = [],
                InventoryTransactions = [],
            });
        }

        var usernames = users.Select(u => u.UserName);
        _logger.LogInformation("{MethodName}() - Prepared {Count} users: {Usernames}", nameof(CreateUsers), users.Count, string.Join(", ", usernames));
        return users;
    }

    public void Seed()
    {
        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(User));

        SeedAsync(CancellationToken.None).GetAwaiter().GetResult();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(User));

        var users = CreateUsers();

        foreach (var user in users)
        {
            if (string.IsNullOrWhiteSpace(user.UserName))
            {
                _logger.LogWarning("User has no username. Skipping user creation.");
                continue;
            }

            var existingUser = await _userManager.FindByNameAsync(user.UserName);
            if (existingUser != null)
            {
                _logger.LogWarning("User {UserName} already exists. Skipping.", user.UserName);
                continue;
            }

            var identityResult = await _userManager.CreateAsync(user, "DefaultPassword123!");
            if (!identityResult.Succeeded)
            {
                var errorMessages = string.Join(", ", identityResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create user {UserName}: {Errors}", user.UserName, errorMessages);
                continue;
            }

            await AssignUserToRoleAsync(user, DefaultRole, ct);

            _logger.LogInformation("User {UserName} created successfully.", user.UserName);
        }
    }

    private async Task AssignUserToRoleAsync(User user, string role, CancellationToken ct)
    {
        if (!await _roleManager.RoleExistsAsync(role))
        {
            var roleResult = await _roleManager.CreateAsync(new IdentityRole(role));
            if (!roleResult.Succeeded)
            {
                var errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                _logger.LogError("Failed to create role '{Role}': {Errors}", role, errors);
                return;
            }

            _logger.LogInformation("Created role '{Role}'", role);
        }

        var addToRoleResult = await _userManager.AddToRoleAsync(user, role);
        if (!addToRoleResult.Succeeded)
        {
            var errors = string.Join(", ", addToRoleResult.Errors.Select(e => e.Description));
            _logger.LogError("Failed to assign user '{UserName}' to role '{Role}': {Errors}", user.UserName, role, errors);
            return;
        }

        _logger.LogInformation("User '{UserName}' assigned to role '{Role}'", user.UserName, role);
    }
}