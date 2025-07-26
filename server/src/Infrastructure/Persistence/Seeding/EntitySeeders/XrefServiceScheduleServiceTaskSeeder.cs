using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class XrefServiceScheduleServiceTaskSeeder : IEntitySeeder
{
    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<XrefServiceScheduleServiceTask> _xrefDbSet;
    private readonly ILogger<XrefServiceScheduleServiceTaskSeeder> _logger;

    public XrefServiceScheduleServiceTaskSeeder(OmnipulseDatabaseContext context, ILogger<XrefServiceScheduleServiceTaskSeeder> logger)
    {
        _dbContext = context;
        _xrefDbSet = context.XrefServiceScheduleServiceTasks;
        _logger = logger;
    }

    private List<XrefServiceScheduleServiceTask> CreateXrefServiceScheduleServiceTasks()
    {
        if (!SeedingHelper.CheckEntitiesExist<ServiceSchedule>(_dbContext, _logger))
            return [];

        if (!SeedingHelper.CheckEntitiesExist<ServiceTask>(_dbContext, _logger))
            return [];

        var scheduleIds = SeedingHelper.ProjectEntities<ServiceSchedule, int>(_dbContext, s => s.ID, _logger);
        var taskIds = SeedingHelper.ProjectEntities<ServiceTask, int>(_dbContext, t => t.ID, _logger);

        var xrefs = new List<XrefServiceScheduleServiceTask>(scheduleIds.Count * taskIds.Count);

        foreach (var scheduleId in scheduleIds)
        {
            foreach (var taskId in taskIds)
            {
                xrefs.Add(new XrefServiceScheduleServiceTask
                {
                    ServiceScheduleID = scheduleId,
                    ServiceTaskID = taskId,
                    ServiceSchedule = null!,
                    ServiceTask = null!
                });
            }
        }

        _logger.LogInformation("{MethodName}() - Created {Count} XrefServiceScheduleServiceTasks: {@Xrefs}", nameof(CreateXrefServiceScheduleServiceTasks), xrefs.Count, xrefs);
        return xrefs;
    }

    public void Seed()
    {
        if (_xrefDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(XrefServiceScheduleServiceTask));

        var xrefs = CreateXrefServiceScheduleServiceTasks();

        _xrefDbSet.AddRange(xrefs);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _xrefDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(XrefServiceScheduleServiceTask));

        var xrefs = CreateXrefServiceScheduleServiceTasks();

        await _xrefDbSet.AddRangeAsync(xrefs, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}