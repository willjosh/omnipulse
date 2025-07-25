using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding.EntitySeeders;

public class ServiceProgramSeeder : IEntitySeeder
{
    private const int SeedCount = 3;

    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<ServiceProgram> _serviceProgramDbSet;

    public ServiceProgramSeeder(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
        _serviceProgramDbSet = context.ServicePrograms;
    }

    public void Seed()
    {
        if (_serviceProgramDbSet.Any()) return;

        var servicePrograms = CreateServicePrograms();

        _serviceProgramDbSet.AddRange(servicePrograms);
        _dbContext.SaveChanges();
    }

    public async Task SeedAsync(CancellationToken ct)
    {
        if (await _serviceProgramDbSet.AnyAsync(ct)) return;

        var servicePrograms = CreateServicePrograms();

        await _serviceProgramDbSet.AddRangeAsync(servicePrograms, ct);
        await _dbContext.SaveChangesAsync(ct);
    }

    private static List<ServiceProgram> CreateServicePrograms()
    {
        var now = DateTime.UtcNow;
        var servicePrograms = new List<ServiceProgram>();

        for (int i = 1; i <= SeedCount; i++)
        {
            servicePrograms.Add(new ServiceProgram
            {
                ID = 0,
                Name = $"Service Program {i} Name",
                Description = $"Service Program {i} Description",
                IsActive = true,
                CreatedAt = now,
                UpdatedAt = now,
                ServiceSchedules = [],
                XrefServiceProgramVehicles = []
            });
        }

        return servicePrograms;
    }
}