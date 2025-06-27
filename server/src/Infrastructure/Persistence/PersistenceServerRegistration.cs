using System;
using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Persistence.DatabaseContext;

namespace Persistence;

public static class PersistenceServerRegistration
{
    public static IServiceCollection AddPersistenceServer(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OmnipulseDatabaseContext>(opt =>
        {
            opt.UseSqlServer(config.GetConnectionString("OmnipulseDatabaseConnection"));
        });

        services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<OmnipulseDatabaseContext>().AddDefaultTokenProviders();

        return services;
    }
}
