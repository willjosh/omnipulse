using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.Vehicles;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(Vehicle))]
public class CreateVehicleIntegrationTests : BaseIntegrationTest
{
    public CreateVehicleIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddVehicleToDatabase_When_CommandIsValid()
    {
        // Arrange - Dependencies
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"[Dependency] {nameof(VehicleGroup)} - {nameof(Should_AddVehicleToDatabase_When_CommandIsValid)}",
            Description: $"[Dependency] {nameof(VehicleGroup)} - {nameof(Should_AddVehicleToDatabase_When_CommandIsValid)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange
        var createCommand = new CreateVehicleCommand(
            Name: $"Test {nameof(Vehicle)} Name",
            Make: Faker.Vehicle.Manufacturer(),
            Model: Faker.Vehicle.Model(),
            Year: 2024,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: $"ABC{Faker.Random.AlphaNumeric(3)}",
            LicensePlateExpirationDate: DateTime.UtcNow.AddYears(1),
            VehicleType: VehicleTypeEnum.VAN,
            VehicleGroupID: vehicleGroupId,
            Trim: "Base",
            Mileage: 0,
            FuelCapacity: 70.0,
            FuelType: FuelTypeEnum.DIESEL,
            PurchaseDate: DateTime.UtcNow.AddDays(-1),
            PurchasePrice: 45000m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Main Depot",
            AssignedTechnicianID: null
        );

        // Act
        int createdVehicleId = await Sender.Send(createCommand);

        // Assert
        Vehicle? createdVehicleEntity = await DbContext.Vehicles.FindAsync(createdVehicleId);
        createdVehicleEntity.Should().NotBeNull();

        createdVehicleEntity.Name.Should().Be(createCommand.Name);
        createdVehicleEntity.Make.Should().Be(createCommand.Make);
        createdVehicleEntity.Model.Should().Be(createCommand.Model);
        createdVehicleEntity.Year.Should().Be(createCommand.Year);
        createdVehicleEntity.VIN.Should().Be(createCommand.VIN);
        createdVehicleEntity.LicensePlate.Should().Be(createCommand.LicensePlate);
        createdVehicleEntity.LicensePlateExpirationDate.Should().Be(createCommand.LicensePlateExpirationDate);
        createdVehicleEntity.VehicleType.Should().Be(createCommand.VehicleType);
        createdVehicleEntity.VehicleGroupID.Should().Be(createCommand.VehicleGroupID);
        createdVehicleEntity.Trim.Should().Be(createCommand.Trim);
        createdVehicleEntity.Mileage.Should().Be(createCommand.Mileage);
        createdVehicleEntity.FuelCapacity.Should().Be(createCommand.FuelCapacity);
        createdVehicleEntity.FuelType.Should().Be(createCommand.FuelType);
        createdVehicleEntity.PurchaseDate.Should().Be(createCommand.PurchaseDate);
        createdVehicleEntity.PurchasePrice.Should().Be(createCommand.PurchasePrice);
        createdVehicleEntity.Status.Should().Be(createCommand.Status);
        createdVehicleEntity.Location.Should().Be(createCommand.Location);
        createdVehicleEntity.AssignedTechnicianID.Should().Be(createCommand.AssignedTechnicianID);
    }
}