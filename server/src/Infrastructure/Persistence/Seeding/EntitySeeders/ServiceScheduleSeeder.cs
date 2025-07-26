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

        // Get existing ServicePrograms to use their actual IDs
        var existingServicePrograms = _dbContext.ServicePrograms.ToList();
        if (existingServicePrograms.Count == 0)
        {
            _logger.LogWarning($"{nameof(CreateServiceSchedules)}() - No {nameof(ServiceProgram)} found. {nameof(ServiceSchedule)}s will not be created.");
            return serviceSchedules;
        }

        for (int i = 1; i <= SeedCount; i++)
        {
            // Use modulo to cycle through available ServicePrograms
            var serviceProgram = existingServicePrograms[(i - 1) % existingServicePrograms.Count];

            serviceSchedules.Add(new ServiceSchedule
            {
                ID = 0,
                ServiceProgramID = serviceProgram.ID,
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