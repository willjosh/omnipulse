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
        const int resolvedEveryNthIssue = 3;

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

            var reportedDate = now.AddDays(-i * 7);
            var isResolved = i % resolvedEveryNthIssue == 0;
            var resolvedDate = isResolved ? reportedDate.AddDays(1) : (DateTime?)null; // Resolved >= Reported

            if (resolvedDate.HasValue && resolvedDate < reportedDate)
            {
                _logger.LogError("❌ Skipping issue {Index} - ResolvedDate before ReportedDate", i);
                continue;
            }

            var resolvedByUserId = isResolved
                ? SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, (i + 1) % 3, _logger)
                : null;

            issues.Add(new Issue
            {
                ID = 0,
                VehicleID = vehicleId,
                IssueNumber = i + 1,
                Title = $"Issue {i} Title",
                Description = $"Issue {i} Description.",
                Category = (IssueCategoryEnum)((i - 1) % Enum.GetValues<IssueCategoryEnum>().Length),
                PriorityLevel = (PriorityLevelEnum)((i - 1) % Enum.GetValues<PriorityLevelEnum>().Length),
                Status = isResolved ? IssueStatusEnum.RESOLVED : IssueStatusEnum.OPEN,
                ReportedDate = reportedDate,
                ReportedByUserID = reportedByUserId,
                ResolvedDate = resolvedDate,
                ResolvedByUserID = resolvedByUserId,
                ResolutionNotes = isResolved ? $"Issue {i} has been resolved successfully." : null,
                CreatedAt = now,
                UpdatedAt = now,
                IssueAttachments = [],
                IssueAssignments = [],
                Vehicle = null!,
                ReportedByUser = null!,
                ResolvedByUser = null
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