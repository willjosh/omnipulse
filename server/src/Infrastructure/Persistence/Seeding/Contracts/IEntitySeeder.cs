namespace Persistence.Seeding.Contracts;

public interface IEntitySeeder
{
    void Seed();
    Task SeedAsync(CancellationToken ct);
}