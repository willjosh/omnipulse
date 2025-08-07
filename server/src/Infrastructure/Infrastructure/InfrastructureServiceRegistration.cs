using Application.Contracts.JwtService;
using Application.Contracts.Logger;
using Application.Contracts.Services;
using Application.Contracts.UserServices;

using Infrastructure.BackgroundServices;
using Infrastructure.Logger;
using Infrastructure.PdfGeneration;
using Infrastructure.Services;

using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure;

public static class InfrastructureServiceRegistration
{
    /// <summary>
    /// Adds Infrastructure services to the DI container.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <returns>The service collection with the Infrastructure services added.</returns>
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        // Register the generic application logger implementation
        services.AddScoped(typeof(IAppLogger<>), typeof(AppLogger<>));
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<IJwtService, JwtService>();

        // Invoice PDF Generation Service
        services.AddScoped<IInvoicePdfService, HandlebarsPdfGenerator>();

        // Register UpdateServiceReminderStatusBackgroundService
        services.AddHostedService<UpdateServiceReminderStatusBackgroundService>();

        return services;
    }
}