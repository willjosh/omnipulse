using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;
using Persistence.Seeding.EntitySeeders;

namespace Persistence.Seeding;

public static class SeedingExtensions
{
    public static DbContextOptionsBuilder UseOmnipulseDbSeeding(this DbContextOptionsBuilder options)
    {
        return options
            .UseSeeding((context, _) =>
            {
                var omnipulseDbContext = context as OmnipulseDatabaseContext ?? throw new InvalidCastException($"Expected {nameof(OmnipulseDatabaseContext)}");

                var dbSeeder = new OmnipulseDbSeeder(omnipulseDbContext, [
                    new ServiceTaskSeeder(omnipulseDbContext),
                ]);

                dbSeeder.SeedAll();
            })
            .UseAsyncSeeding(async (context, _, ct) =>
            {
                var omnipulseDbContext = context as OmnipulseDatabaseContext ?? throw new InvalidCastException($"Expected {nameof(OmnipulseDatabaseContext)}");

                var dbSeeder = new OmnipulseDbSeeder(omnipulseDbContext, [
                    new ServiceTaskSeeder(omnipulseDbContext),
                ]);

                await dbSeeder.SeedAllAsync(ct);
            });
    }
}