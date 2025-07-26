using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class VehicleGroupSeeder : IEntitySeeder
{
    private const int SeedCount = 3;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<VehicleGroup> _vehicleGroupDbSet;
    private readonly ILogger<VehicleGroupSeeder> _logger;

    public VehicleGroupSeeder(OmnipulseDatabaseContext context, ILogger<VehicleGroupSeeder> logger)
    {
        _dbContext = context;
        _vehicleGroupDbSet = context.VehicleGroups;
        _logger = logger;
    }

    private List<VehicleGroup> CreateVehicleGroups()
    {
        var now = DateTime.UtcNow;
        var vehicleGroups = new List<VehicleGroup>();

        for (int i = 1; i <= SeedCount; i++)
        {
            vehicleGroups.Add(new VehicleGroup
            {
                ID = 0,
                Name = $"Vehicle Group {i} Name",
                Description = $"Vehicle Group {i} Description",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} Vehicle Groups: {@VehicleGroups}", nameof(CreateVehicleGroups), vehicleGroups.Count, vehicleGroups);
        return vehicleGroups;
    }

    public void Seed()
    {
        if (_vehicleGroupDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(VehicleGroup));

        var vehicleGroups = CreateVehicleGroups();

        _vehicleGroupDbSet.AddRange(vehicleGroups);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _vehicleGroupDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(VehicleGroup));

        var vehicleGroups = CreateVehicleGroups();

        await _vehicleGroupDbSet.AddRangeAsync(vehicleGroups, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}