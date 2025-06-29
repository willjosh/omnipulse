using System;
using Application.Contracts.Persistence;
using Application.Features.Vehicle.Command.CreateVehicle;
using Application.Features.Vehicles.Command.CreateVehicle;
using Domain.Entities;
using Moq;

namespace Application.Test.Vehicles.CommandTest;

public class CreateVehicleHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly CreateVehicleCommandHandler _createVehicleCommandHandler;
    public CreateVehicleHandlerTest()
    {
        _mockVehicleRepository = new();
        _createVehicleCommandHandler = new(_mockVehicleRepository.Object);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_VehicleID_On_Success()
    {
        // Given 
        var CreateVehicleCommand = new CreateVehicleCommand(
            Name: "James The Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1007",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 0,
            Trim: "JMS",
            Mileage: 300000,
            EngineHours: 1000,
            FuelCapacity: 10000,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 20000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Kuala Lumpur"
        );

        var ExpectedVehicle = new Vehicle()
        {
            ID = 123,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = CreateVehicleCommand.Name,
            Make = CreateVehicleCommand.Make,
            Model = CreateVehicleCommand.Model,
            Year = CreateVehicleCommand.Year,
            VIN = CreateVehicleCommand.VIN,
            LicensePlate = CreateVehicleCommand.LicensePlate,
            LicensePlateExpirationDate = CreateVehicleCommand.LicensePlateExpirationDate,
            VehicleType = CreateVehicleCommand.VehicleType,
            VehicleGroupID = CreateVehicleCommand.VehicleGroupID,
            Trim = CreateVehicleCommand.Trim,
            Mileage = CreateVehicleCommand.Mileage,
            EngineHours = CreateVehicleCommand.EngineHours,
            FuelCapacity = CreateVehicleCommand.FuelCapacity,
            FuelType = CreateVehicleCommand.FuelType,
            PurchaseDate = CreateVehicleCommand.PurchaseDate,
            PurchasePrice = CreateVehicleCommand.PurchasePrice,
            Status = CreateVehicleCommand.Status,
            Location = CreateVehicleCommand.Location,
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>())).ReturnsAsync(ExpectedVehicle);
        _mockVehicleRepository
            .Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        var result = await _createVehicleCommandHandler.Handle(CreateVehicleCommand, CancellationToken.None);

        // Then
        Assert.Equal(ExpectedVehicle.ID, result);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), times: Times.Once);

        // Verify repository was called with correct vehicle
        _mockVehicleRepository.Verify(
            r => r.AddAsync(It.Is<Vehicle>(v =>
                v.Name == ExpectedVehicle.Name &&
                v.Make == ExpectedVehicle.Make &&
                v.VIN == ExpectedVehicle.VIN &&
                v.ID == 0)),
            Times.Once);
    }
}
