using Microsoft.EntityFrameworkCore;

namespace Persistence.Seeding.Contracts;

public interface IEntitySeeder
{
    void Seed(DbContext context);
    Task SeedAsync(DbContext context, CancellationToken ct);
}