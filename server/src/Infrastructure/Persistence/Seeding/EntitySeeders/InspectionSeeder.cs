using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class InspectionSeeder : IEntitySeeder
{
    private const int SeedCount = 10;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<Inspection> _inspectionDbSet;
    private readonly ILogger<InspectionSeeder> _logger;

    public InspectionSeeder(OmnipulseDatabaseContext context, ILogger<InspectionSeeder> logger)
    {
        _dbContext = context;
        _inspectionDbSet = context.Inspections;
        _logger = logger;
    }

    private List<Inspection> CreateInspections()
    {
        var now = DateTime.UtcNow;
        var inspections = new List<Inspection>();

        // Check if required entities exist before creating inspections
        if (!SeedingHelper.CheckEntitiesExist<InspectionForm>(_dbContext, _logger)) return inspections;
        if (!SeedingHelper.CheckEntitiesExist<Vehicle>(_dbContext, _logger)) return inspections;
        if (!SeedingHelper.CheckEntitiesExist<User>(_dbContext, _logger)) return inspections;
        if (!SeedingHelper.CheckEntitiesExist<InspectionFormItem>(_dbContext, _logger)) return inspections;

        for (int i = 1; i <= SeedCount; i++)
        {
            var inspectionFormId = SeedingHelper.ProjectEntityByIndex<InspectionForm, int>(_dbContext, f => f.ID, (i - 1) % 3, _logger);
            if (inspectionFormId == 0) continue;

            var vehicleId = SeedingHelper.ProjectEntityByIndex<Vehicle, int>(_dbContext, v => v.ID, (i - 1) % 7, _logger);
            if (vehicleId == 0) continue;

            var technicianId = SeedingHelper.ProjectEntityByIndex<User, string>(_dbContext, u => u.Id, (i - 1) % 3, _logger);
            if (string.IsNullOrEmpty(technicianId)) continue;

            var inspectionForm = SeedingHelper.GetEntityByIndex<InspectionForm>(_dbContext, (i - 1) % 3, _logger);
            if (inspectionForm == null) continue;

            var inspectionStartTime = now.AddDays(-i * 7).AddHours(9); // 9 AM
            var inspectionEndTime = inspectionStartTime.AddHours(1 + (i % 2)); // 1-2 hours duration
            var odometerReading = 50000 + (i * 1000) + (i * 100);

            var inspection = new Inspection
            {
                ID = 0,
                InspectionFormID = inspectionFormId,
                VehicleID = vehicleId,
                TechnicianID = technicianId,
                InspectionStartTime = inspectionStartTime,
                InspectionEndTime = inspectionEndTime,
                OdometerReading = odometerReading,
                VehicleCondition = (VehicleConditionEnum)(i % 3), // Distribute across all conditions
                Notes = i % 3 == 0 ? $"Inspection notes for inspection {i}" : null,
                SnapshotFormTitle = inspectionForm.Title,
                SnapshotFormDescription = inspectionForm.Description,
                InspectionForm = null!,
                InspectionPassFailItems = [],
                Vehicle = null!,
                User = null!,
                CreatedAt = now,
                UpdatedAt = now
            };

            // Create InspectionPassFailItems for each InspectionFormItem in the form
            var inspectionPassFailItems = CreateInspectionPassFailItems(inspection, inspectionFormId, now);
            inspection.InspectionPassFailItems = inspectionPassFailItems;

            inspections.Add(inspection);
        }

        _logger.LogInformation("{MethodName}() - Created {Count} Inspections: {@Inspections}",
            nameof(CreateInspections), inspections.Count, inspections);
        return inspections;
    }

    private List<InspectionPassFailItem> CreateInspectionPassFailItems(Inspection inspection, int inspectionFormId, DateTime now)
    {
        var passFailItems = new List<InspectionPassFailItem>();

        // Get all InspectionFormItems for this InspectionForm
        var inspectionFormItems = _dbContext.InspectionFormItems
            .Where(ifi => ifi.InspectionFormID == inspectionFormId && ifi.IsActive)
            .ToList();

        // Determine if this inspection should have all passed items (80% chance)
        var inspectionRandom = new Random(inspection.ID);
        var shouldHaveAllPassed = inspectionRandom.Next(100) < 80; // 80% chance

        foreach (var formItem in inspectionFormItems)
        {
            bool passed;

            if (shouldHaveAllPassed)
            {
                // 80% of inspections: all items pass
                passed = true;
            }
            else
            {
                // 20% of inspections: some items may fail
                var itemRandom = new Random(inspection.ID + formItem.ID);
                passed = itemRandom.Next(100) < 70; // 70% chance of pass for failed inspections
            }

            passFailItems.Add(new InspectionPassFailItem
            {
                InspectionID = 0, // Will be set by EF Core after inspection is saved
                InspectionFormItemID = formItem.ID,
                Passed = passed,
                Comment = passed ? null : $"Issue found with {formItem.ItemLabel}",
                SnapshotItemLabel = formItem.ItemLabel,
                SnapshotItemDescription = formItem.ItemDescription,
                SnapshotItemInstructions = formItem.ItemInstructions,
                SnapshotIsRequired = formItem.IsRequired,
                SnapshotInspectionFormItemTypeEnum = formItem.InspectionFormItemTypeEnum,
                Inspection = inspection,
                InspectionFormItem = null! // Will be set by EF Core
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} InspectionPassFailItems for Inspection {InspectionId} (AllPassed: {AllPassed})",
            nameof(CreateInspectionPassFailItems), passFailItems.Count, inspection.ID, shouldHaveAllPassed);
        return passFailItems;
    }

    public void Seed()
    {
        if (_inspectionDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(Inspection));

        var inspections = CreateInspections();

        _inspectionDbSet.AddRange(inspections);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _inspectionDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(Inspection));

        var inspections = CreateInspections();

        await _inspectionDbSet.AddRangeAsync(inspections, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}