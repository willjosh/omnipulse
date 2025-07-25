using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class ServiceTaskSeeder : IEntitySeeder
{
    private const int SeedCount = 5;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<ServiceTask> _serviceTaskDbSet;
    private readonly ILogger<ServiceTaskSeeder> _logger;

    public ServiceTaskSeeder(OmnipulseDatabaseContext context, ILogger<ServiceTaskSeeder> logger)
    {
        _dbContext = context;
        _serviceTaskDbSet = context.ServiceTasks;
        _logger = logger;
    }

    private List<ServiceTask> CreateServiceTasks()
    {
        var now = DateTime.UtcNow;
        var serviceTasks = new List<ServiceTask>();

        for (int i = 1; i <= SeedCount; i++)
        {
            serviceTasks.Add(new ServiceTask
            {
                ID = 0,
                Name = $"Service Task {i} Name",
                Description = $"Service Task {i} Description",
                EstimatedLabourHours = 1.0 + i * 0.5,
                EstimatedCost = 50m + i * 25,
                Category = i % 2 == 0 ? ServiceTaskCategoryEnum.PREVENTIVE : ServiceTaskCategoryEnum.CORRECTIVE,
                IsActive = true,
                UpdatedAt = now,
                CreatedAt = now,
                XrefServiceScheduleServiceTasks = [],
                MaintenanceHistories = [],
                WorkOrderLineItems = []
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} Service Tasks: {@ServiceTasks}", nameof(CreateServiceTasks), serviceTasks.Count, serviceTasks);
        return serviceTasks;
    }

    public void Seed()
    {
        if (_serviceTaskDbSet.Any()) return;

        var serviceTasks = CreateServiceTasks();

        _serviceTaskDbSet.AddRange(serviceTasks);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _serviceTaskDbSet.AnyAsync(ct)) return;

        var serviceTasks = CreateServiceTasks();

        await _serviceTaskDbSet.AddRangeAsync(serviceTasks, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}