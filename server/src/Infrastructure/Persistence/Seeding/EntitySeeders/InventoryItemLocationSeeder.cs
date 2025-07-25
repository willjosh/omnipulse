using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class InventoryItemLocationSeeder : IEntitySeeder
{
    private const int SeedCount = 3;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<InventoryItemLocation> _itemLocationDbSet;
    private readonly ILogger<InventoryItemLocationSeeder> _logger;

    public InventoryItemLocationSeeder(OmnipulseDatabaseContext context, ILogger<InventoryItemLocationSeeder> logger)
    {
        _dbContext = context;
        _itemLocationDbSet = context.InventoryItemLocations;
        _logger = logger;
    }

    private List<InventoryItemLocation> CreateLocations()
    {
        var now = DateTime.UtcNow;
        var itemLocations = new List<InventoryItemLocation>();

        for (int i = 1; i <= SeedCount; i++)
        {
            itemLocations.Add(new InventoryItemLocation
            {
                ID = 0,
                LocationName = $"Location {i}",
                Address = $"{i} Main St, City {i}",
                Longitude = 150.0 + i,
                Latitude = -33.0 - i,
                Capacity = 100 * i,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                Inventories = []
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} InventoryItemLocations: {@ItemLocations}", nameof(CreateLocations), itemLocations.Count, itemLocations);
        return itemLocations;
    }

    public void Seed()
    {
        if (_itemLocationDbSet.Any()) return;

        var itemLocations = CreateLocations();

        _itemLocationDbSet.AddRange(itemLocations);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _itemLocationDbSet.AnyAsync(ct)) return;

        var itemLocations = CreateLocations();

        await _itemLocationDbSet.AddRangeAsync(itemLocations, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}