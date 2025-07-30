using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Persistence.DatabaseContext;

using Testcontainers.MsSql;

namespace IntegrationTests.Abstractions;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string MsSqlImage = "mcr.microsoft.com/mssql/server:2022-latest";
    private const string MsSqlPassword = "YourStrong!Passw0rd";

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage(MsSqlImage)
        .WithPassword(MsSqlPassword)
        .Build();

    /// <summary>
    /// Configures the web host for integration testing by replacing the <see cref="OmnipulseDatabaseContext"/> with a test container-based SQL Server instance.
    /// </summary>
    /// <param name="builder">The web host builder to configure.</param>
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Remove the existing database context configuration
            services.RemoveAll<DbContextOptions<OmnipulseDatabaseContext>>();

            // Add the test container-based database context
            services.AddDbContext<OmnipulseDatabaseContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));
        });
    }

    /// <summary>
    /// Initialises the test container database before running tests.
    /// </summary>
    /// <returns>A task that represents the asynchronous initialisation operation.</returns>
    public Task InitializeAsync() => _dbContainer.StartAsync();

    /// <summary>
    /// Disposes of the test container database after tests complete.
    /// </summary>
    /// <returns>A task that represents the asynchronous disposal operation.</returns>
    public new Task DisposeAsync() => _dbContainer.StopAsync();
}