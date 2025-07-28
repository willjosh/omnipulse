using Application.Features.VehicleGroups.Command.CreateVehicleGroup;

using Domain.Entities;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.VehicleGroups;

[Trait("TestCategory", "Integration")]
[Trait("Entity", "Vehicle Group")]
public class CreateVehicleGroupIntegrationTests : BaseIntegrationTest
{
    public CreateVehicleGroupIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddVehicleGroupToDatabase_When_CommandIsValid()
    {
        // Arrange
        var createCommand = new CreateVehicleGroupCommand(
            Name: $"Test {nameof(VehicleGroup)} {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test {nameof(VehicleGroup)} Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );

        // Act
        int createdVehicleGroupId = await Sender.Send(createCommand);

        // Assert
        VehicleGroup? createdVehicleGroupEntity = await DbContext.VehicleGroups.FindAsync(createdVehicleGroupId);
        createdVehicleGroupEntity.Should().NotBeNull();

        createdVehicleGroupEntity.Name.Should().Be(createCommand.Name);
        createdVehicleGroupEntity.Description.Should().Be(createCommand.Description);
        createdVehicleGroupEntity.IsActive.Should().Be(createCommand.IsActive);
    }
}