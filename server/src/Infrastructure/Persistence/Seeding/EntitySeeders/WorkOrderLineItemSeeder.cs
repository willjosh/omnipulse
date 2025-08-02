using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class WorkOrderLineItemSeeder : IEntitySeeder
{
    private const int SeedCount = 30;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<WorkOrderLineItem> _workOrderLineItemDbSet;
    private readonly ILogger<WorkOrderLineItemSeeder> _logger;

    public WorkOrderLineItemSeeder(OmnipulseDatabaseContext context, ILogger<WorkOrderLineItemSeeder> logger)
    {
        _dbContext = context;
        _workOrderLineItemDbSet = context.WorkOrderLineItems;
        _logger = logger;
    }

    private List<WorkOrderLineItem> CreateWorkOrderLineItems()
    {
        var now = DateTime.UtcNow;
        var workOrderLineItems = new List<WorkOrderLineItem>();

        // Check if required entities exist
        if (!SeedingHelper.CheckEntitiesExist<WorkOrder>(_dbContext, _logger)) return workOrderLineItems;
        if (!SeedingHelper.CheckEntitiesExist<ServiceTask>(_dbContext, _logger)) return workOrderLineItems;
        if (!SeedingHelper.CheckEntitiesExist<User>(_dbContext, _logger)) return workOrderLineItems;
        if (!SeedingHelper.CheckEntitiesExist<InventoryItem>(_dbContext, _logger)) return workOrderLineItems;

        for (int i = 1; i <= SeedCount; i++)
        {
            var workOrderId = SeedingHelper.ProjectEntityByIndex<WorkOrder, int>(_dbContext, wo => wo.ID, (i - 1) % 15, _logger);
            if (workOrderId == 0) continue;

            var serviceTaskId = SeedingHelper.ProjectEntityByIndex<ServiceTask, int>(_dbContext, st => st.ID, (i - 1) % 5, _logger);
            if (serviceTaskId == 0) continue;

            var userId = SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, (i - 1) % 3, _logger);
            if (string.IsNullOrEmpty(userId)) continue;

            var inventoryItemId = i % 2 == 0 ? SeedingHelper.ProjectEntityByIndex<InventoryItem, int>(_dbContext, ii => ii.ID, (i - 1) % 10, _logger) : 0;

            workOrderLineItems.Add(new WorkOrderLineItem
            {
                ID = 0,
                WorkOrderID = workOrderId,
                ServiceTaskID = serviceTaskId,
                ItemType = (LineItemTypeEnum)(i % 3),
                Quantity = 1 + (i % 3),
                InventoryItemID = inventoryItemId > 0 ? inventoryItemId : null,
                AssignedToUserID = userId,
                Description = $"Line item {i} description",
                LaborHours = i % 2 == 0 ? 1.0 + (i % 4) : null,
                UnitPrice = i % 2 == 1 ? 25.00m + (i % 5) * 5 : null,
                HourlyRate = i % 2 == 0 ? 50.00m + (i % 3) * 10 : null,
                TotalCost = 0,
                WorkOrder = null!,
                User = null!,
                InventoryItem = null!,
                ServiceTask = null!,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        // Calculate total cost for each line item
        foreach (var lineItem in workOrderLineItems)
        {
            lineItem.CalculateTotalCost();
        }

        _logger.LogInformation("{MethodName}() - Created {Count} WorkOrderLineItems: {@WorkOrderLineItems}",
            nameof(CreateWorkOrderLineItems), workOrderLineItems.Count, workOrderLineItems);
        return workOrderLineItems;
    }

    public void Seed()
    {
        if (_workOrderLineItemDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(WorkOrderLineItem));

        var workOrderLineItems = CreateWorkOrderLineItems();

        _workOrderLineItemDbSet.AddRange(workOrderLineItems);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _workOrderLineItemDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(WorkOrderLineItem));

        var workOrderLineItems = CreateWorkOrderLineItems();

        await _workOrderLineItemDbSet.AddRangeAsync(workOrderLineItems, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}