using System;
using Identity.DbContext;
using Microsoft.EntityFrameworkCore;

namespace Identity;

public class OmnipulseIdentityDbContextFactory
{
    public IdentityDbContext createDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<IdentityDbContext>();

        // TODO: add db url
        optionsBuilder.UseSqlServer("");

        return new IdentityDbContext(optionsBuilder.Options);
    }
}
