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

        for (int i = 1; i <= SeedCount; i++)
        {
            var serviceProgramId = SeedingHelper.ProjectEntityByIndex<ServiceProgram, int>(
                _dbContext, sp => sp.ID, i - 1, _logger
            );
            if (serviceProgramId == 0) continue;

            // Alternate between time-based and mileage-based schedules
            var isTimeBased = i % 2 == 1; // Odd numbers are time-based, even numbers are mileage-based

            var serviceSchedule = new ServiceSchedule
            {
                ID = 0,
                ServiceProgramID = serviceProgramId,
                Name = $"Service Schedule {i} Name",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                XrefServiceScheduleServiceTasks = [],
                ServiceProgram = null!
            };

            if (isTimeBased)
            {
                // Time-based schedule
                serviceSchedule.TimeIntervalValue = 6 * i;
                serviceSchedule.TimeIntervalUnit = TimeUnitEnum.Days;
                serviceSchedule.TimeBufferValue = 1;
                serviceSchedule.TimeBufferUnit = TimeUnitEnum.Days;
                serviceSchedule.FirstServiceDate = now.AddDays(i * 30);
            }
            else
            {
                // Mileage-based schedule
                serviceSchedule.MileageInterval = 10000 * i;
                serviceSchedule.MileageBuffer = 1000;
                serviceSchedule.FirstServiceMileage = 15000 + (100 * i);
            }

            serviceSchedules.Add(serviceSchedule);
        }

        _logger.LogInformation("{MethodName}() - Created {Count} Service Schedules: {@ServiceSchedules}", nameof(CreateServiceSchedules), serviceSchedules.Count, serviceSchedules);
        return serviceSchedules;
    }

    public void Seed()
    {
        if (_serviceScheduleDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(ServiceSchedule));

        var serviceSchedules = CreateServiceSchedules();

        _serviceScheduleDbSet.AddRange(serviceSchedules);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _serviceScheduleDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(ServiceSchedule));

        var serviceSchedules = CreateServiceSchedules();

        await _serviceScheduleDbSet.AddRangeAsync(serviceSchedules, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}