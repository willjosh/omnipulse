using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class XrefServiceScheduleServiceTaskSeeder : IEntitySeeder
{
    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<XrefServiceScheduleServiceTask> _xrefDbSet;

    public XrefServiceScheduleServiceTaskSeeder(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
        _xrefDbSet = context.XrefServiceScheduleServiceTasks;
    }

    private static List<XrefServiceScheduleServiceTask> CreateXrefs(OmnipulseDatabaseContext context)
    {
        var schedules = context.ServiceSchedules.Take(5).ToList();
        var tasks = context.ServiceTasks.Take(5).ToList();
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

        return xrefs;
    }

    public void Seed()
    {
        if (_xrefDbSet.Any()) return;

        var xrefs = CreateXrefs(_dbContext);

        _xrefDbSet.AddRange(xrefs);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _xrefDbSet.AnyAsync(ct)) return;

        var xrefs = CreateXrefs(_dbContext);

        await _xrefDbSet.AddRangeAsync(xrefs, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}