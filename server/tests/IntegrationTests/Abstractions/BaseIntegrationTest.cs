using Bogus;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Persistence.DatabaseContext;

namespace IntegrationTests.Abstractions;

/// <summary>
/// Base class for integration tests.
/// </summary>
[Trait("TestCategory", "Integration")]
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<OmnipulseDatabaseContext>();

        Faker = new Faker();
    }

    protected ISender Sender { get; }
    protected OmnipulseDatabaseContext DbContext { get; }
    protected Faker Faker { get; }

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this); // Prevent finalisation if subclass adds a finaliser
    }
}