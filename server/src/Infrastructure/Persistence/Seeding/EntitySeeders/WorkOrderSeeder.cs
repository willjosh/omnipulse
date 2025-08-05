using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class WorkOrderSeeder : IEntitySeeder
{
    private const int SeedCount = 15;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<WorkOrder> _workOrderDbSet;
    private readonly ILogger<WorkOrderSeeder> _logger;

    public WorkOrderSeeder(OmnipulseDatabaseContext context, ILogger<WorkOrderSeeder> logger)
    {
        _dbContext = context;
        _workOrderDbSet = context.WorkOrders;
        _logger = logger;
    }

    private List<WorkOrder> CreateWorkOrders()
    {
        var now = DateTime.UtcNow;
        var workOrders = new List<WorkOrder>();

        // Check if required entities exist before creating work orders
        if (!SeedingHelper.CheckEntitiesExist<Vehicle>(_dbContext, _logger)) return workOrders;
        if (!SeedingHelper.CheckEntitiesExist<User>(_dbContext, _logger)) return workOrders;

        for (int i = 1; i <= SeedCount; i++)
        {
            var vehicleId = SeedingHelper.ProjectEntityByIndex<Vehicle, int>(_dbContext, v => v.ID, (i - 1) % 7, _logger);
            if (vehicleId == 0) continue;

            var assignedToUserId = SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, (i - 1) % 3, _logger);
            if (string.IsNullOrEmpty(assignedToUserId)) continue;

            workOrders.Add(new WorkOrder
            {
                ID = 0,
                VehicleID = vehicleId,
                AssignedToUserID = assignedToUserId,
                Title = $"Work Order {i}",
                Description = $"Description for work order {i}",
                WorkOrderType = (WorkTypeEnum)(i % 2),
                PriorityLevel = (PriorityLevelEnum)(i % 4),
                Status = (WorkOrderStatusEnum)(i % 7),
                ScheduledStartDate = now.AddDays(i),
                ActualStartDate = now.AddDays(i).AddHours(2),
                ScheduledCompletionDate = now.AddDays(i + 2),
                ActualCompletionDate = now.AddDays(i + 1),
                StartOdometer = 50000 + (i * 1000),
                EndOdometer = 50000 + (i * 1000) + 100,
                Vehicle = null!,
                MaintenanceHistories = [],
                User = null!,
                WorkOrderLineItems = [],
                Invoices = [],
                InventoryTransactions = [],
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} WorkOrders: {@WorkOrders}",
            nameof(CreateWorkOrders), workOrders.Count, workOrders);
        return workOrders;
    }

    public void Seed()
    {
        if (_workOrderDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(WorkOrder));

        var workOrders = CreateWorkOrders();

        _workOrderDbSet.AddRange(workOrders);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _workOrderDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(WorkOrder));

        var workOrders = CreateWorkOrders();

        await _workOrderDbSet.AddRangeAsync(workOrders, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}