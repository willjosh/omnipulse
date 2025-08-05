using Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class InspectionFormSeeder : IEntitySeeder
{
    private const int SeedCount = 3;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<InspectionForm> _inspectionFormDbSet;
    private readonly ILogger<InspectionFormSeeder> _logger;

    public InspectionFormSeeder(OmnipulseDatabaseContext context, ILogger<InspectionFormSeeder> logger)
    {
        _dbContext = context;
        _inspectionFormDbSet = context.InspectionForms;
        _logger = logger;
    }

    private List<InspectionForm> CreateInspectionForms()
    {
        var now = DateTime.UtcNow;
        var inspectionForms = new List<InspectionForm>();

        for (int i = 1; i <= SeedCount; i++)
        {
            inspectionForms.Add(new InspectionForm
            {
                ID = 0,
                Title = GetInspectionFormTitle(i),
                Description = GetInspectionFormDescription(i),
                IsActive = true,
                Inspections = [],
                InspectionFormItems = [],
                CreatedAt = now,
                UpdatedAt = now
            });
        }

        _logger.LogInformation("{MethodName}() - Created {Count} InspectionForms: {@InspectionForms}",
            nameof(CreateInspectionForms), inspectionForms.Count, inspectionForms);
        return inspectionForms;
    }

    private static string GetInspectionFormTitle(int index)
    {
        return index switch
        {
            1 => "Pre-Trip Vehicle Inspection",
            2 => "Post-Trip Vehicle Inspection",
            3 => "Monthly Maintenance Inspection",
            _ => $"Inspection Form {index}"
        };
    }

    private static string GetInspectionFormDescription(int index)
    {
        return index switch
        {
            1 => "Comprehensive pre-trip inspection checklist to ensure vehicle safety and compliance before departure.",
            2 => "Post-trip inspection to document any issues or damage that occurred during the trip.",
            3 => "Monthly comprehensive maintenance inspection covering all vehicle systems and components.",
            _ => $"Description for inspection form {index}"
        };
    }

    public void Seed()
    {
        if (_inspectionFormDbSet.Any()) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(Seed), nameof(InspectionForm));

        var inspectionForms = CreateInspectionForms();

        _inspectionFormDbSet.AddRange(inspectionForms);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _inspectionFormDbSet.AnyAsync(ct)) return;

        _logger.LogInformation("🌱 {MethodName}() - Seeding {EntityName}", nameof(SeedAsync), nameof(InspectionForm));

        var inspectionForms = CreateInspectionForms();

        await _inspectionFormDbSet.AddRangeAsync(inspectionForms, ct);
        await _dbContext.SaveChangesAsync(ct);
    }
}