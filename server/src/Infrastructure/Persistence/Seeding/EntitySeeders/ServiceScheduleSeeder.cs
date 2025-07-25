using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class ServiceScheduleSeeder : IEntitySeeder
{
    private const int SeedCount = 5;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<ServiceSchedule> _serviceScheduleDbSet;

    public ServiceScheduleSeeder(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
        _serviceScheduleDbSet = context.ServiceSchedules;
    }

    private static List<ServiceSchedule> CreateServiceSchedules()
    {
        var now = DateTime.UtcNow;
        var schedules = new List<ServiceSchedule>();

        for (int i = 1; i <= SeedCount; i++)
        {
            schedules.Add(new ServiceSchedule
            {
                ID = 0,
                ServiceProgramID = 1,
                Name = $"Service Schedule {i} Name",
                TimeIntervalValue = 6 * i,
                TimeIntervalUnit = TimeUnitEnum.Days,
                TimeBufferValue = 1,
                TimeBufferUnit = TimeUnitEnum.Days,
                MileageInterval = 10000 * i,
                MileageBuffer = 1000,
                FirstServiceTimeValue = 3 * i,
                FirstServiceTimeUnit = TimeUnitEnum.Days,
                FirstServiceMileage = 5000 * i,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                XrefServiceScheduleServiceTasks = [],
                ServiceProgram = null!
            });
        }

        return schedules;
    }

    public void Seed()
    {
        if (_serviceScheduleDbSet.Any()) return;

        var serviceSchedules = CreateServiceSchedules();

        _serviceScheduleDbSet.AddRange(serviceSchedules);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _serviceScheduleDbSet.AnyAsync(ct)) return;

        var serviceSchedules = CreateServiceSchedules();

        await _serviceScheduleDbSet.AddRangeAsync(serviceSchedules, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}