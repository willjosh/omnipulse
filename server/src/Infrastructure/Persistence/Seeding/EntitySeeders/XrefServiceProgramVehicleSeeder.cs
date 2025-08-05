using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class XrefServiceProgramVehicleSeeder : IEntitySeeder
{
    private const int SeedCount = 10;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<XrefServiceProgramVehicle> _xrefDbSet;
    private readonly ILogger<XrefServiceProgramVehicleSeeder> _logger;

    public XrefServiceProgramVehicleSeeder(OmnipulseDatabaseContext context, ILogger<XrefServiceProgramVehicleSeeder> logger)
    {
        _dbContext = context;
        _xrefDbSet = context.XrefServiceProgramVehicles;
        _logger = logger;
    }

    private List<XrefServiceProgramVehicle> CreateXrefServiceProgramVehicles()
    {
        var now = DateTime.UtcNow;
        var xrefs = new List<XrefServiceProgramVehicle>();

        // Check if ServicePrograms, Vehicles, and Users exist before creating Xrefs
        if (!SeedingHelper.CheckEntitiesExist<ServiceProgram>(_dbContext, _logger)) return xrefs;
        if (!SeedingHelper.CheckEntitiesExist<Vehicle>(_dbContext, _logger)) return xrefs;
        if (!SeedingHelper.CheckEntitiesExist<User>(_dbContext, _logger)) return xrefs;

        for (int i = 1; i <= SeedCount; i++)
        {
            var serviceProgramId = SeedingHelper.ProjectEntityByIndex<ServiceProgram, int>(_dbContext, sp => sp.ID, i - 1, _logger);
            if (serviceProgramId == 0) continue;

            var vehicleId = SeedingHelper.ProjectEntityByIndex<Vehicle, int>(_dbContext, v => v.ID, i - 1, _logger);
            if (vehicleId == 0) continue;

            var userId = SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, i - 1, _logger); // TODO
            if (string.IsNullOrEmpty(userId)) continue;

            var vehicle = _dbContext.Vehicles.FirstOrDefault(v => v.ID == vehicleId);
            if (vehicle == null) continue;

            xrefs.Add(new XrefServiceProgramVehicle
            {
                ServiceProgramID = serviceProgramId,
                VehicleID = vehicleId,
                AddedAt = now.AddDays(-i),
                VehicleMileageAtAssignment = vehicle.Mileage,
                ServiceProgram = null!,
                Vehicle = null!,
                // User = null! // TODO XrefServiceProgramVehicle User
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} XrefServiceProgramVehicles: {@Xrefs}", nameof(CreateXrefServiceProgramVehicles), xrefs.Count, xrefs);
        return xrefs;
    }

    public void Seed()
    {
        if (_xrefDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(XrefServiceProgramVehicle));

        var xrefs = CreateXrefServiceProgramVehicles();

        _xrefDbSet.AddRange(xrefs);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _xrefDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(XrefServiceProgramVehicle));

        var xrefs = CreateXrefServiceProgramVehicles();

        await _xrefDbSet.AddRangeAsync(xrefs, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}