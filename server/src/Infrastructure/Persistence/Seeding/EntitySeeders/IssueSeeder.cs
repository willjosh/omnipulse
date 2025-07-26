using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class IssueSeeder : IEntitySeeder
{
    private const int SeedCount = 7;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<Issue> _issueDbSet;
    private readonly ILogger<IssueSeeder> _logger;

    public IssueSeeder(OmnipulseDatabaseContext context, ILogger<IssueSeeder> logger)
    {
        _dbContext = context;
        _issueDbSet = context.Issues;
        _logger = logger;
    }

    private List<Issue> CreateIssues()
    {
        var now = DateTime.UtcNow;
        var issues = new List<Issue>();

        // Check if Users and Vehicles exist before creating Issues
        if (!SeedingHelper.CheckEntitiesExist<User>(_dbContext, _logger)) return issues;
        if (!SeedingHelper.CheckEntitiesExist<Vehicle>(_dbContext, _logger)) return issues;

        for (int i = 1; i <= SeedCount; i++)
        {
            var reportedByUserId = SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, i - 1, _logger);
            if (string.IsNullOrEmpty(reportedByUserId)) continue;

            var vehicleId = SeedingHelper.ProjectEntityByIndex<Vehicle, int>(_dbContext, v => v.ID, i - 1, _logger);
            if (vehicleId == 0) continue;

            var resolvedByUserId = i % 3 == 0 ? SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, (i + 1) % 3, _logger) : null;

            issues.Add(new Issue
            {
                ID = 0,
                VehicleID = vehicleId,
                IssueNumber = 1000 + i,
                ReportedByUserID = reportedByUserId,
                ReportedDate = now.AddDays(-i * 2),
                Title = $"Issue {i} Title",
                Description = $"Issue {i} Description.",
                Category = (IssueCategoryEnum)((i - 1) % 8), // Cycle through all categories
                PriorityLevel = (PriorityLevelEnum)((i - 1) % 4), // Cycle through all priority levels
                Status = i % 3 == 0 ? IssueStatusEnum.RESOLVED : IssueStatusEnum.OPEN, // Some resolved, some open
                ResolvedDate = i % 3 == 0 ? now.AddDays(-i) : null,
                ResolvedByUserID = resolvedByUserId,
                ResolutionNotes = i % 3 == 0 ? $"Issue {i} has been resolved successfully." : null,
                IssueAttachments = [],
                IssueAssignments = [],
                Vehicle = null!,
                ReportedByUser = null!,
                ResolvedByUser = null,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} Issues: {@Issues}", nameof(CreateIssues), issues.Count, issues);
        return issues;
    }

    public void Seed()
    {
        if (_issueDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(Issue));

        var issues = CreateIssues();

        _issueDbSet.AddRange(issues);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _issueDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(Issue));

        var issues = CreateIssues();

        await _issueDbSet.AddRangeAsync(issues, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}