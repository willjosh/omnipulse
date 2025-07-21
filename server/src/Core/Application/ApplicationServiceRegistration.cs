using System.Reflection;

using FluentValidation;

using Microsoft.Extensions.DependencyInjection;

namespace Application;

public static class ApplicationServiceRegistration
{
    /// <summary>
    /// Adds Application services to the DI container.
    /// </summary>
    /// <param name="services">The service collection to register services with.</param>
    /// <returns>The service collection with the Application services added.</returns>
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        // Register AutoMapper - all mapping profiles
        services.AddAutoMapper(Assembly.GetExecutingAssembly());

        // Register MediatR - all handlers
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly()));

        // Register FluentValidation - all validators
        services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());

        return services;
    }
}