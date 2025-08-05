using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class InspectionFormItemSeeder : IEntitySeeder
{
    private const int SeedCount = 15;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<InspectionFormItem> _inspectionFormItemDbSet;
    private readonly ILogger<InspectionFormItemSeeder> _logger;

    public InspectionFormItemSeeder(OmnipulseDatabaseContext context, ILogger<InspectionFormItemSeeder> logger)
    {
        _dbContext = context;
        _inspectionFormItemDbSet = context.InspectionFormItems;
        _logger = logger;
    }

    private List<InspectionFormItem> CreateInspectionFormItems()
    {
        var now = DateTime.UtcNow;
        var inspectionFormItems = new List<InspectionFormItem>();

        // Check if InspectionForms exist before creating items
        if (!SeedingHelper.CheckEntitiesExist<InspectionForm>(_dbContext, _logger))
            return inspectionFormItems;

        for (int i = 1; i <= SeedCount; i++)
        {
            var formId = SeedingHelper.ProjectEntityByIndex<InspectionForm, int>(_dbContext, f => f.ID, (i - 1) % 3, _logger);
            if (formId == 0) continue;

            inspectionFormItems.Add(new InspectionFormItem
            {
                ID = 0,
                InspectionFormID = formId,
                ItemLabel = $"Inspection Item {i}",
                ItemDescription = $"Description for inspection item {i}",
                ItemInstructions = $"Instructions for inspection item {i}",
                IsRequired = i % 3 != 0, // Every 3rd item is optional
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                InspectionForm = null!
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} InspectionFormItems: {@InspectionFormItems}",
            nameof(CreateInspectionFormItems), inspectionFormItems.Count, inspectionFormItems);
        return inspectionFormItems;
    }

    public void Seed()
    {
        if (_inspectionFormItemDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(InspectionFormItem));

        var inspectionFormItems = CreateInspectionFormItems();

        _inspectionFormItemDbSet.AddRange(inspectionFormItems);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _inspectionFormItemDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(InspectionFormItem));

        var inspectionFormItems = CreateInspectionFormItems();

        await _inspectionFormItemDbSet.AddRangeAsync(inspectionFormItems, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}