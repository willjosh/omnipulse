using Bogus;

using FluentAssertions;

using MediatR;

using Microsoft.Extensions.DependencyInjection;

using Persistence.DatabaseContext;

namespace IntegrationTests.Abstractions;

/// <summary>
/// Base class for integration tests.
/// </summary>
[Trait("TestCategory", "Integration")]
[Collection("IntegrationTests")]
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected virtual DateTimeOffset BaselineNow => new(2025, 6, 2, 9, 0, 0, TimeSpan.Zero);

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Factory = factory;

        // Ensure fake time is not before BaselineNow (FakeTimeProvider can't move backwards)
        DateTimeOffset now = Factory.FakeTimeProvider.GetUtcNow();
        if (now < BaselineNow)
        {
            Factory.FakeTimeProvider.SetUtcNow(BaselineNow);
        }

        _scope = Factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<OmnipulseDatabaseContext>();

        Faker = new Faker();
    }

    protected IntegrationTestWebAppFactory Factory { get; }

    protected ISender Sender { get; }
    protected OmnipulseDatabaseContext DbContext { get; }
    protected Faker Faker { get; }

    protected DateTime GetUtcToday()
        => DateTime.SpecifyKind(Factory.FakeTimeProvider.GetUtcNow().UtcDateTime.Date, DateTimeKind.Utc);

    protected DateTime GetUtcTodayPlusDays(int days)
        => DateTime.SpecifyKind(Factory.FakeTimeProvider.GetUtcNow().UtcDateTime.Date.AddDays(days), DateTimeKind.Utc);

    /// <summary>Advance the fake clock by a duration.</summary>
    /// <param name="by">The amount of time to advance.</param>
    /// <example><c>ClockAdvance(TimeSpan.FromDays(1))</c></example>
    protected void ClockAdvance(TimeSpan by) => Factory.FakeTimeProvider.Advance(by);

    /// <summary>Set the fake clock to an exact instant.</summary>
    /// <remarks>Can throw if you pass a time earlier than current fake time</remarks>
    protected void ClockSetNow(DateTimeOffset now) => Factory.FakeTimeProvider.SetUtcNow(now);

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this); // Prevent finalisation if subclass adds a finaliser
    }

    [Fact]
    public void Should_HaveDatabaseContext_When_TestStarts()
    {
        // Assert
        DbContext.Should().NotBeNull();
    }

    [Fact]
    public void Should_HaveSender_When_TestStarts()
    {
        // Assert
        Sender.Should().NotBeNull();
    }

    [Fact]
    public void Should_HaveFaker_When_TestStarts()
    {
        // Assert
        Faker.Should().NotBeNull();
    }
}