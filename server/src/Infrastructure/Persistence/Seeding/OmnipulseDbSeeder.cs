using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;

namespace Persistence.Seeding;

public class OmnipulseDbSeeder
{
    private readonly OmnipulseDatabaseContext _context;
    private readonly IEnumerable<IEntitySeeder> _seeders;

    public OmnipulseDbSeeder(OmnipulseDatabaseContext context, IEnumerable<IEntitySeeder> seeders)
    {
        _context = context;
        _seeders = seeders;
    }

    public void SeedAll()
    {
        foreach (var seeder in _seeders)
            seeder.Seed();
    }

    public async Task SeedAllAsync(CancellationToken ct)
    {
        foreach (var seeder in _seeders)
            await seeder.SeedAsync(ct);
    }
}