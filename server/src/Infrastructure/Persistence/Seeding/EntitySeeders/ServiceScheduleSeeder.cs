using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class ServiceScheduleSeeder : IEntitySeeder
{
    private const int SeedCount = 5;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<ServiceSchedule> _serviceScheduleDbSet;
    private readonly ILogger<ServiceScheduleSeeder> _logger;

    public ServiceScheduleSeeder(OmnipulseDatabaseContext context, ILogger<ServiceScheduleSeeder> logger)
    {
        _dbContext = context;
        _serviceScheduleDbSet = context.ServiceSchedules;
        _logger = logger;
    }

    private List<ServiceSchedule> CreateServiceSchedules()
    {
        var now = DateTime.UtcNow;
        var serviceSchedules = new List<ServiceSchedule>();

        // Check if ServicePrograms exist before creating ServiceSchedules
        if (!SeedingHelper.CheckEntitiesExist<ServiceProgram>(_dbContext, _logger))
            return serviceSchedules;

        for (int i = 0; i < SeedCount; i++)
        {
            var serviceProgramId = SeedingHelper.ProjectEntityByIndex<ServiceProgram, int>(
                _dbContext, sp => sp.ID, i, _logger
            );
            if (serviceProgramId == 0) continue;

            serviceSchedules.Add(new ServiceSchedule
            {
                ID = 0,
                ServiceProgramID = serviceProgramId,
                Name = $"Service Schedule {i + 1} Name",
                TimeIntervalValue = 6 * (i + 1),
                TimeIntervalUnit = TimeUnitEnum.Days,
                TimeBufferValue = 1,
                TimeBufferUnit = TimeUnitEnum.Days,
                MileageInterval = 10000 * (i + 1),
                MileageBuffer = 1000,
                FirstServiceTimeValue = 3 * (i + 1),
                FirstServiceTimeUnit = TimeUnitEnum.Days,
                FirstServiceMileage = 5000 * (i + 1),
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                XrefServiceScheduleServiceTasks = [],
                ServiceProgram = null!
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} Service Schedules: {@ServiceSchedules}", nameof(CreateServiceSchedules), serviceSchedules.Count, serviceSchedules);
        return serviceSchedules;
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