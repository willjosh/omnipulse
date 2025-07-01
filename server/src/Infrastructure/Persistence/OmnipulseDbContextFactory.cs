using System;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

using Persistence.DatabaseContext;

namespace Persistence;

public class OmnipulseDbContextFactory : IDesignTimeDbContextFactory<OmnipulseDatabaseContext>
{
    public OmnipulseDatabaseContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<OmnipulseDatabaseContext>();

        // TODO: add db url
        optionsBuilder.UseSqlServer("URL_HERE");

        return new OmnipulseDatabaseContext(optionsBuilder.Options);
    }
}