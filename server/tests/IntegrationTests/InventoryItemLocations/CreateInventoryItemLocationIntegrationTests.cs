using Application.Features.InventoryItemLocations.Command;

using Domain.Entities;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.InventoryItemLocations;

[Trait("Entity", "Inventory Item Location")]
public class CreateInventoryItemLocationIntegrationTests : BaseIntegrationTest
{
    public CreateInventoryItemLocationIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddInventoryItemLocationToDatabase_When_CommandIsValid()
    {
        // Arrange
        var createCommand = new CreateInventoryItemLocationCommand(
            LocationName: $"Test {nameof(InventoryItemLocation)} {Faker.Random.AlphaNumeric(5)}",
            Address: $"123 Test Street, Test City, {Faker.Random.AlphaNumeric(2)}",
            Longitude: -73.935242,
            Latitude: 40.730610,
            Capacity: 1000
        );

        // Act
        int createdInventoryItemLocationId = await Sender.Send(createCommand);

        // Assert
        InventoryItemLocation? createdInventoryItemLocationEntity = await DbContext.InventoryItemLocations.FindAsync(createdInventoryItemLocationId);
        createdInventoryItemLocationEntity.Should().NotBeNull();

        createdInventoryItemLocationEntity.LocationName.Should().Be(createCommand.LocationName);
        createdInventoryItemLocationEntity.Address.Should().Be(createCommand.Address);
        createdInventoryItemLocationEntity.Longitude.Should().Be(createCommand.Longitude);
        createdInventoryItemLocationEntity.Latitude.Should().Be(createCommand.Latitude);
        createdInventoryItemLocationEntity.Capacity.Should().Be(createCommand.Capacity);
        createdInventoryItemLocationEntity.IsActive.Should().Be(true);
    }
}