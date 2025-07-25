using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

using Persistence.DatabaseContext;
using Persistence.Seeding.Contracts;
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
                var dbSeeder = new OmnipulseDbSeeder(omnipulseDbContext, GetSeeders(omnipulseDbContext));
                dbSeeder.SeedAll();
            })
            .UseAsyncSeeding(async (context, _, ct) =>
            {
                var omnipulseDbContext = context as OmnipulseDatabaseContext ?? throw new InvalidCastException($"Expected {nameof(OmnipulseDatabaseContext)}");
                var dbSeeder = new OmnipulseDbSeeder(omnipulseDbContext, GetSeeders(omnipulseDbContext));
                await dbSeeder.SeedAllAsync(ct);
            });
    }

    private static IEnumerable<IEntitySeeder> GetSeeders(OmnipulseDatabaseContext context)
    {
        return
        [
            context.GetService<ServiceProgramSeeder>(),
            context.GetService<ServiceTaskSeeder>(),
            context.GetService<ServiceScheduleSeeder>(),
            context.GetService<XrefServiceScheduleServiceTaskSeeder>(),
            context.GetService<VehicleGroupSeeder>(),
            context.GetService<UserSeeder>(),
            context.GetService<InventoryItemLocationSeeder>(),
            context.GetService<FuelPurchaseSeeder>(),
        ];
    }
}