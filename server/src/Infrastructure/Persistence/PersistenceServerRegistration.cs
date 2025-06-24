using System;
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

        return services;     
    }
}
