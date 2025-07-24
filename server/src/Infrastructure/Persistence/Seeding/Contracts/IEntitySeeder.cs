using Persistence.DatabaseContext;

namespace Persistence.Seeding.Contracts;

/// <summary>
/// Entity seeding operations.
/// </summary>
public interface IEntitySeeder
{
    /// <summary>
    /// Seeds entity data into the database.
    /// </summary>
    /// <param name="context">The database context</param>
    /// <returns>A task representing the asynchronous seeding operation</returns>
    Task SeedAsync(OmnipulseDatabaseContext context);
}