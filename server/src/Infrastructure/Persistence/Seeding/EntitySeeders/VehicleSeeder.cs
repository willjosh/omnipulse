using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class VehicleSeeder : IEntitySeeder
{
    private const int SeedCount = 7;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<Vehicle> _vehicleDbSet;
    private readonly ILogger<VehicleSeeder> _logger;

    public VehicleSeeder(OmnipulseDatabaseContext context, ILogger<VehicleSeeder> logger)
    {
        _dbContext = context;
        _vehicleDbSet = context.Vehicles;
        _logger = logger;
    }

    private List<Vehicle> CreateVehicles()
    {
        var now = DateTime.UtcNow;
        var vehicles = new List<Vehicle>();

        // Check if VehicleGroups exist before creating Vehicles
        if (!SeedingHelper.CheckEntitiesExist<VehicleGroup>(_dbContext, _logger)) return vehicles;

        for (int i = 1; i <= SeedCount; i++)
        {
            var vehicleGroupId = SeedingHelper.ProjectEntityByIndex<VehicleGroup, int>(_dbContext, vg => vg.ID, i - 1, _logger);
            if (vehicleGroupId == 0) continue;

            vehicles.Add(new Vehicle
            {
                ID = 0,
                Name = $"Vehicle {i}",
                Make = $"Make {i}",
                Model = $"Model {i}",
                Year = 2020 + i,
                VIN = $"VIN00000{i}",
                LicensePlate = $"PLATE{i:000}",
                LicensePlateExpirationDate = now.AddYears(1),
                VehicleType = VehicleTypeEnum.CAR,
                VehicleGroupID = vehicleGroupId,
                AssignedTechnicianID = null,
                Trim = $"Trim {i}",
                Mileage = 10000 * i,
                EngineHours = 100 * i,
                FuelCapacity = 50 + i * 5,
                FuelType = FuelTypeEnum.PETROL,
                PurchaseDate = now.AddYears(-i),
                PurchasePrice = 20000 + i * 5000,
                Status = VehicleStatusEnum.ACTIVE,
                Location = $"Garage {i}",
                User = null,
                VehicleGroup = null!,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                Inspections = [],
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} Vehicles: {@Vehicles}", nameof(CreateVehicles), vehicles.Count, vehicles);
        return vehicles;
    }

    public void Seed()
    {
        if (_vehicleDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(Vehicle));

        var vehicles = CreateVehicles();

        _vehicleDbSet.AddRange(vehicles);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _vehicleDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(Vehicle));

        var vehicles = CreateVehicles();

        await _vehicleDbSet.AddRangeAsync(vehicles, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}