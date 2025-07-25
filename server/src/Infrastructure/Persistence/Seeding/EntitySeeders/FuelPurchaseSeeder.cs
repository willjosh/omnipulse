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
    private readonly List<string> _userIds = [];
    private readonly Random _random = new();

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

        for (int i = 1; i <= SeedCount; i++)
        {
            var userId = GetRandomUserId();
            if (userId == null)
            {
                _logger.LogWarning("No users found. Skipping fuel purchase seeding.");
                break;
            }

            fuelPurchases.Add(new FuelPurchase
            {
                ID = 0,
                VehicleId = 1,
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

        var purchases = CreateFuelPurchases();

        _fuelPurchaseDbSet.AddRange(purchases);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _fuelPurchaseDbSet.AnyAsync(ct)) return;

        var purchases = CreateFuelPurchases();

        await _fuelPurchaseDbSet.AddRangeAsync(purchases, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    private string? GetRandomUserId()
    {
        if (_userIds.Count == 0) _userIds.AddRange([.. _dbContext.Users.Select(u => u.Id)]);

        return _userIds.Count == 0
            ? null
            : _userIds[_random.Next(_userIds.Count)];
    }
}