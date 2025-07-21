using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Persistence.DatabaseContext;
using Persistence.Repository;

namespace Persistence;

public static class PersistenceServerRegistration
{
    public static IServiceCollection AddPersistenceServer(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<OmnipulseDatabaseContext>(opt => opt.UseSqlServer(config.GetConnectionString("OmnipulseDatabaseConnection")));

        services.AddIdentity<User, IdentityRole>().AddEntityFrameworkStores<OmnipulseDatabaseContext>().AddDefaultTokenProviders();

        // Repository Dependency Injection Registration
        services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
        services.AddScoped<IFuelPurchaseRepository, FuelPurchasesRepository>();
        // services.AddScoped<IInventoryItemLocationRepository, InventoryItemLocationRepository>(); // TODO: Implement repository
        // services.AddScoped<IInventoryItemRepository, InventoryItemRepository>(); // TODO: Implement repository
        services.AddScoped<IIssueRepository, IssueRepository>();
        services.AddScoped<IMaintenanceHistoryRepository, MaintenanceHistoryRepository>();
        services.AddScoped<IServiceProgramRepository, ServiceProgramRepository>();
        // services.AddScoped<IServiceReminderRepository, ServiceReminderRepository>(); // TODO: Implement repository
        services.AddScoped<IServiceScheduleRepository, ServiceScheduleRepository>();
        services.AddScoped<IServiceTaskRepository, ServiceTaskRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IVehicleGroupRepository, VehicleGroupRepository>();
        services.AddScoped<IVehicleRepository, VehicleRepository>();
        // services.AddScoped<IWorkOrderIssueRepository, WorkOrderIssueRepository>(); // TODO: Implement repository
        // services.AddScoped<IWorkOrderLineItemRepository, WorkOrderLineItemRepository>(); // TODO: Implement repository
        // services.AddScoped<IWorkOrderRepository, WorkOrderRepository>(); // TODO: Implement repository
        services.AddScoped<IXrefServiceProgramVehicleRepository, XrefServiceProgramVehicleRepository>();
        services.AddScoped<IXrefServiceScheduleServiceTaskRepository, XrefServiceScheduleServiceTaskRepository>();

        return services;
    }
}