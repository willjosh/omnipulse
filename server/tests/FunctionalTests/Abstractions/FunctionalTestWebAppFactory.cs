using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

using Persistence.DatabaseContext;

using Testcontainers.MsSql;

namespace FunctionalTests.Abstractions;

public class FunctionalTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private const string MsSqlImage = "mcr.microsoft.com/mssql/server:2022-latest";
    private const string MsSqlPassword = "YourStrong!Passw0rd";

    private readonly MsSqlContainer _dbContainer = new MsSqlBuilder()
        .WithImage(MsSqlImage)
        .WithPassword(MsSqlPassword)
        .Build();

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            services.RemoveAll<DbContextOptions<OmnipulseDatabaseContext>>();

            services.AddDbContext<OmnipulseDatabaseContext>(options =>
                options.UseSqlServer(_dbContainer.GetConnectionString()));
        });
    }

    public Task InitializeAsync() => _dbContainer.StartAsync();
    public new Task DisposeAsync() => _dbContainer.StopAsync();
}