using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Persistence.DatabaseContext;

namespace Persistence;

/// <summary>
/// Design-time factory for creating <see cref="OmnipulseDatabaseContext"/> instances.
/// Required by Entity Framework Core tools (e.g. <c>dotnet ef migrations</c>) when the <see cref="OmnipulseDatabaseContext"/> constructor expects runtime-supplied options.
/// </summary>
public class OmnipulseDbContextFactory : IDesignTimeDbContextFactory<OmnipulseDatabaseContext>
{
    /// <summary>
    /// Creates a new <see cref="OmnipulseDatabaseContext"/> using a design-time connection string.
    /// </summary>
    /// <param name="args">Command-line arguments passed by EF Core tools.</param>
    /// <returns>A configured <see cref="OmnipulseDatabaseContext"/>.</returns>
    public OmnipulseDatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OmnipulseDatabaseContext>();

        // Try to get connection string from environment variable
        var connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Connection string not found. Set the ConnectionStrings__DefaultConnection environment variable.");
        }

        optionsBuilder.UseSqlServer(connectionString);
        return new OmnipulseDatabaseContext(optionsBuilder.Options);
    }
}