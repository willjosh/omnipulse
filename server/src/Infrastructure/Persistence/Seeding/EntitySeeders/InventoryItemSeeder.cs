using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class InventoryItemSeeder : IEntitySeeder
{
    private const int SeedCount = 3;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<InventoryItem> _inventoryItemDbSet;
    private readonly ILogger<InventoryItemSeeder> _logger;

    public InventoryItemSeeder(OmnipulseDatabaseContext context, ILogger<InventoryItemSeeder> logger)
    {
        _dbContext = context;
        _inventoryItemDbSet = context.InventoryItems;
        _logger = logger;
    }

    private List<InventoryItem> CreateInventoryItems()
    {
        var now = DateTime.UtcNow;
        var inventoryItems = new List<InventoryItem>();

        for (int i = 1; i <= SeedCount; i++)
        {
            inventoryItems.Add(new InventoryItem
            {
                ID = 0,
                ItemNumber = $"ITEM-{i:000}",
                ItemName = $"Inventory Item {i}",
                Description = $"Description for item {i}",
                Category = i % 2 == 0 ? InventoryItemCategoryEnum.TIRES : InventoryItemCategoryEnum.BODY,
                Manufacturer = $"Manufacturer {i}",
                ManufacturerPartNumber = $"MPN-{i:0000}",
                UniversalProductCode = $"12345678901{i}",
                UnitCost = 10.0m * i,
                UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Gram,
                Supplier = $"Supplier {i}",
                WeightKG = 1.5 * i,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                Inventories = [],
                WorkOrderLineItems = []
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} InventoryItems: {@InventoryItems}", nameof(CreateInventoryItems), inventoryItems.Count, inventoryItems);
        return inventoryItems;
    }

    public void Seed()
    {
        if (_inventoryItemDbSet.Any()) return;

        var inventoryItems = CreateInventoryItems();

        _inventoryItemDbSet.AddRange(inventoryItems);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _inventoryItemDbSet.AnyAsync(ct)) return;

        var inventoryItems = CreateInventoryItems();

        await _inventoryItemDbSet.AddRangeAsync(inventoryItems, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}