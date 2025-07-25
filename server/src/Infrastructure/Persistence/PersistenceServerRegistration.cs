using Application.Contracts.Persistence;

using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Persistence.DatabaseContext;
using Persistence.Repository;
using Persistence.Seeding;
using Persistence.Seeding.Contracts;
using Persistence.Seeding.EntitySeeders;

namespace Persistence;

public static class PersistenceServerRegistration
{
    /// <summary>
    /// Adds Persistence services to the DI container.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <param name="config">The application configuration (used to load the connection string).</param>
    /// <returns>The service collection with the Persistence services added.</returns>
    public static IServiceCollection AddPersistenceServer(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OmnipulseDatabaseContext>(opt => opt
            .UseSqlServer(
                config.GetConnectionString("OmnipulseDatabaseConnection")
            )
            .UseOmnipulseDbSeeding()
        );

        services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<OmnipulseDatabaseContext>().AddDefaultTokenProviders();

        // Repository Dependency Injection Registration
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IFuelPurchaseRepository, FuelPurchasesRepository>();
        services.AddScoped<IInventoryItemLocationRepository, InventoryItemLocationRepository>();
        services.AddScoped<IInventoryItemRepository, InventoryItemRepository>();
        services.AddScoped<IIssueRepository, IssueRepository>();
        services.AddScoped<IMaintenanceHistoryRepository, MaintenanceHistoryRepository>();
        services.AddScoped<IServiceProgramRepository, ServiceProgramRepository>();
        services.AddScoped<IServiceReminderRepository, ServiceReminderRepository>();
        services.AddScoped<IServiceScheduleRepository, ServiceScheduleRepository>();
        services.AddScoped<IServiceTaskRepository, ServiceTaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVehicleGroupRepository, VehicleGroupRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        services.AddScoped<IWorkOrderIssueRepository, WorkOrderIssueRepository>();
        services.AddScoped<IWorkOrderLineItemRepository, WorkOrderLineItemRepository>();
        services.AddScoped<IWorkOrderRepository, WorkOrderRepository>();
        services.AddScoped<IXrefServiceProgramVehicleRepository, XrefServiceProgramVehicleRepository>();
        services.AddScoped<IXrefServiceScheduleServiceTaskRepository, XrefServiceScheduleServiceTaskRepository>();

        // Register Seeders
        services.AddScoped<ServiceProgramSeeder>();
        services.AddScoped<ServiceTaskSeeder>();
        services.AddScoped<ServiceScheduleSeeder>();
        services.AddScoped<XrefServiceScheduleServiceTaskSeeder>();
        services.AddScoped<VehicleGroupSeeder>();
        services.AddScoped<UserSeeder>();
        services.AddScoped<InventoryItemLocationSeeder>();
        services.AddScoped<FuelPurchaseSeeder>();
        services.AddScoped<InventoryItemSeeder>();

        return services;
    }
}