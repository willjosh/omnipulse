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
        var schedules = _dbContext.ServiceSchedules.Take(5).ToList();
        var tasks = _dbContext.ServiceTasks.Take(5).ToList();
        var xrefs = new List<XrefServiceScheduleServiceTask>();

        foreach (var schedule in schedules)
        {
            foreach (var task in tasks)
            {
                xrefs.Add(new XrefServiceScheduleServiceTask
                {
                    ServiceScheduleID = schedule.ID,
                    ServiceTaskID = task.ID,
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

        var xrefs = CreateXrefServiceScheduleServiceTasks();

        _xrefDbSet.AddRange(xrefs);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _xrefDbSet.AnyAsync(ct)) return;

        var xrefs = CreateXrefServiceScheduleServiceTasks();

        await _xrefDbSet.AddRangeAsync(xrefs, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}