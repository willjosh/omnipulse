using Application.Features.FuelLogging.Command.CreateFuelPurchase;
using Application.Features.Users.Command.CreateTechnician;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.FuelPurchases;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(FuelPurchase))]
public class CreateFuelPurchaseIntegrationTests : BaseIntegrationTest
{
    public CreateFuelPurchaseIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddFuelPurchaseToDatabase_When_CommandIsValid()
    {
        // Arrange - Dependencies
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"[Dependency] {nameof(VehicleGroup)} - {nameof(Should_AddFuelPurchaseToDatabase_When_CommandIsValid)}",
            Description: $"[Dependency] {nameof(VehicleGroup)} - {nameof(Should_AddFuelPurchaseToDatabase_When_CommandIsValid)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"[Dependency] {nameof(Vehicle)} - {nameof(Should_AddFuelPurchaseToDatabase_When_CommandIsValid)}",
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
            EngineHours: 0,
            FuelCapacity: 70.0,
            FuelType: FuelTypeEnum.DIESEL,
            PurchaseDate: DateTime.UtcNow.AddDays(-1),
            PurchasePrice: 45000m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Main Depot",
            AssignedTechnicianID: null
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        var createTechnicianCommand = new CreateTechnicianCommand(
            Email: $"technician{Faker.Random.AlphaNumeric(5)}@omnipulse.com",
            Password: Faker.Internet.Password(length: 50, regexPattern: @"[a-zA-Z0-9!@#$%^&*()_+=\-]"),
            FirstName: Faker.Name.FirstName(),
            LastName: Faker.Name.LastName(),
            HireDate: DateTime.UtcNow.AddYears(-1),
            IsActive: true
        );
        Guid technicianId = await Sender.Send(createTechnicianCommand);

        // Arrange
        var createCommand = new CreateFuelPurchaseCommand(
            VehicleId: vehicleId,
            PurchasedByUserId: technicianId.ToString(),
            PurchaseDate: DateTime.UtcNow,
            OdometerReading: 15000.0,
            Volume: 50.0,
            PricePerUnit: 1.50m,
            TotalCost: 75.00m,
            FuelStation: $"Test {nameof(FuelPurchase)} Fuel Station {Faker.Random.AlphaNumeric(5)}",
            ReceiptNumber: $"REC{Faker.Random.AlphaNumeric(8)}",
            Notes: $"Test {nameof(FuelPurchase)} notes {Faker.Random.AlphaNumeric(10)}"
        );

        // Act
        int createdFuelPurchaseId = await Sender.Send(createCommand);

        // Assert
        FuelPurchase? createdFuelPurchaseEntity = await DbContext.FuelPurchases.FindAsync(createdFuelPurchaseId);
        createdFuelPurchaseEntity.Should().NotBeNull();

        createdFuelPurchaseEntity.VehicleId.Should().Be(createCommand.VehicleId);
        createdFuelPurchaseEntity.PurchasedByUserId.Should().Be(createCommand.PurchasedByUserId);
        createdFuelPurchaseEntity.PurchaseDate.Should().Be(createCommand.PurchaseDate);
        createdFuelPurchaseEntity.OdometerReading.Should().Be(createCommand.OdometerReading);
        createdFuelPurchaseEntity.Volume.Should().Be(createCommand.Volume);
        createdFuelPurchaseEntity.PricePerUnit.Should().Be(createCommand.PricePerUnit);
        createdFuelPurchaseEntity.TotalCost.Should().Be(createCommand.TotalCost);
        createdFuelPurchaseEntity.FuelStation.Should().Be(createCommand.FuelStation);
        createdFuelPurchaseEntity.ReceiptNumber.Should().Be(createCommand.ReceiptNumber);
        createdFuelPurchaseEntity.Notes.Should().Be(createCommand.Notes);
    }
}