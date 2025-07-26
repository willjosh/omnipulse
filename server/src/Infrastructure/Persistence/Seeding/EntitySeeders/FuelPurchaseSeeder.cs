using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class FuelPurchaseSeeder : IEntitySeeder
{
    private const int SeedCount = 3;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<FuelPurchase> _fuelPurchaseDbSet;
    private readonly ILogger<FuelPurchaseSeeder> _logger;

    public FuelPurchaseSeeder(OmnipulseDatabaseContext context, ILogger<FuelPurchaseSeeder> logger)
    {
        _dbContext = context;
        _fuelPurchaseDbSet = context.FuelPurchases;
        _logger = logger;
    }

    private List<FuelPurchase> CreateFuelPurchases()
    {
        var now = DateTime.UtcNow;
        var fuelPurchases = new List<FuelPurchase>();

        // Check if Vehicles exist before creating FuelPurchases
        if (!SeedingHelper.CheckEntitiesExist<Vehicle>(_dbContext, _logger)) return fuelPurchases;

        for (int i = 1; i <= SeedCount; i++)
        {
            var userId = SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, i - 1, _logger);
            if (string.IsNullOrEmpty(userId)) continue;

            var vehicleId = SeedingHelper.ProjectEntityByIndex<Vehicle, int>(_dbContext, v => v.ID, i - 1, _logger);
            if (vehicleId == 0) continue;

            fuelPurchases.Add(new FuelPurchase
            {
                ID = 0,
                VehicleId = vehicleId,
                PurchasedByUserId = userId,
                PurchaseDate = now.AddDays(-i),
                OdometerReading = 10000 + i * 500,
                Volume = 50 + i * 5,
                PricePerUnit = 2.5m + i * 0.1m,
                TotalCost = 0, // Will be calculated
                FuelStation = $"Station {i}",
                ReceiptNumber = $"RCPT-{i:0000}",
                Notes = $"Fuel purchase {i}",
                CreatedAt = now,
                UpdatedAt = now,
                Vehicle = null!,
                User = null!
            });
        }

        // Calculate total cost for each purchase
        foreach (var fuelPurchase in fuelPurchases)
        {
            fuelPurchase.CalculateTotalCost();
        }

        _logger.LogInformation("{MethodName}() - Created {Count} FuelPurchases: {@FuelPurchases}", nameof(CreateFuelPurchases), fuelPurchases.Count, fuelPurchases);
        return fuelPurchases;
    }

    public void Seed()
    {
        if (_fuelPurchaseDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(FuelPurchase));

        var purchases = CreateFuelPurchases();

        _fuelPurchaseDbSet.AddRange(purchases);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _fuelPurchaseDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(FuelPurchase));

        var purchases = CreateFuelPurchases();

        await _fuelPurchaseDbSet.AddRangeAsync(purchases, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}