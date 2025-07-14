using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicles.Command.UpdateVehicle;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

using Xunit;

namespace Application.Test.Vehicles.CommandTest.UpdateVehicle;

public class UpdateVehicleCommandHandlerTest
{
    // Command Handler
    private readonly UpdateVehicleCommandHandler _updateVehicleCommandHandler;

    // Mock Repositories
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IVehicleGroupRepository> _mockVehicleGroupRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;

    // Mock Validator
    private readonly Mock<IValidator<UpdateVehicleCommand>> _mockValidator;

    // Mock Logger
    private readonly Mock<IAppLogger<UpdateVehicleCommandHandler>> _mockLogger;

    public UpdateVehicleCommandHandlerTest()
    {
        _mockVehicleRepository = new();
        _mockVehicleGroupRepository = new();
        _mockUserRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<VehicleMappingProfile>());
        var mapper = config.CreateMapper();

        _updateVehicleCommandHandler = new(
            _mockVehicleRepository.Object,
            _mockVehicleGroupRepository.Object,
            _mockUserRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            mapper);
    }

    // Helper method to create valid command with minimal data
    private UpdateVehicleCommand CreateValidCommand(
        int vehicleID = 123,
        string name = "Updated Vehicle",
        string make = "Toyota",
        string model = "Corolla",
        int year = 2023,
        string vin = "1C3CDFBA5DD298669",
        string licensePlate = "UPD123",
        DateTime? licenseExpiration = null,
        Domain.Entities.Enums.VehicleTypeEnum vehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
        int vehicleGroupID = 1,
        string trim = "Updated Trim",
        double mileage = 50000,
        double engineHours = 1000,
        double fuelCapacity = 50,
        Domain.Entities.Enums.FuelTypeEnum fuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
        DateTime? purchaseDate = null,
        double purchasePrice = 25000,
        Domain.Entities.Enums.VehicleStatusEnum status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
        string location = "Updated Location",
        string? assignedTechnicianID = null
    )
    {
        return new UpdateVehicleCommand(
            VehicleID: vehicleID,
            Name: name,
            Make: make,
            Model: model,
            Year: year,
            VIN: vin,
            LicensePlate: licensePlate,
            LicensePlateExpirationDate: licenseExpiration ?? DateTime.UtcNow.AddYears(1),
            VehicleType: vehicleType,
            VehicleGroupID: vehicleGroupID,
            Trim: trim,
            Mileage: mileage,
            EngineHours: engineHours,
            FuelCapacity: fuelCapacity,
            FuelType: fuelType,
            PurchaseDate: purchaseDate ?? DateTime.UtcNow.AddDays(-30),
            PurchasePrice: purchasePrice,
            Status: status,
            Location: location,
            AssignedTechnicianID: assignedTechnicianID
        );
    }

    // Helper method to create existing vehicle
    private Vehicle CreateExistingVehicle(
        int id = 123,
        string vin = "EXISTING1234567890",
        string name = "Original Vehicle"
    )
    {
        return new Vehicle()
        {
            ID = id,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Name = name,
            Make = "Toyota",
            Model = "Camry",
            Year = 2021,
            VIN = vin,
            LicensePlate = "OLD123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Original Trim",
            Mileage = 30000,
            EngineHours = 500,
            FuelCapacity = 40,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-2),
            PurchasePrice = 20000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location = "Original Location",
            VehicleGroup = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(UpdateVehicleCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(UpdateVehicleCommand command, string propertyName, string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_VehicleID_On_Successfully_Updating_Vehicle()
    {
        // Given
        var command = CreateValidCommand();
        var existingVehicle = CreateExistingVehicle();

        // Setup validation to pass
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync(existingVehicle);
        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(false);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockVehicleGroupRepository.Setup(r => r.ExistsAsync(command.VehicleGroupID)).ReturnsAsync(true);

        // When
        var result = await _updateVehicleCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.VehicleID, result);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        // Verify that Update was called with a vehicle containing the updated values
        _mockVehicleRepository.Verify(
            r => r.Update(It.Is<Vehicle>(v =>
                v.ID == command.VehicleID &&
                v.Name == command.Name &&
                v.Make == command.Make &&
                v.VIN == command.VIN &&
                v.LicensePlate == command.LicensePlate &&
                v.Mileage == command.Mileage)),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_Invalid_VehicleID()
    {
        // Given
        var command = CreateValidCommand(vehicleID: 999);

        // Setup validation to pass
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync((Vehicle?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_VIN_From_Different_Vehicle()
    {
        // Given
        var command = CreateValidCommand(vin: "DUPLICATE1234567890");
        var existingVehicle = CreateExistingVehicle(vin: "ORIGINAL123456789"); // Different VIN

        // Setup validation to pass
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync(existingVehicle);
        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _updateVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Allow_Update_With_Same_VIN_As_Current_Vehicle()
    {
        // Given - updating vehicle with the same VIN it already has (should be allowed)
        var sameVin = "CURRENT123456789";
        var command = CreateValidCommand(vin: sameVin);
        var existingVehicle = CreateExistingVehicle(vin: sameVin); // Same VIN

        // Setup validation to pass
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync(existingVehicle);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);
        _mockVehicleGroupRepository.Setup(r => r.ExistsAsync(command.VehicleGroupID)).ReturnsAsync(true);

        // When
        var result = await _updateVehicleCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.VehicleID, result);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never); // Should not check VIN if it's the same
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Check_Vehicle_Existence_Before_VIN_Duplication()
    {
        // Given - non-existent vehicle
        var command = CreateValidCommand();

        // Setup validation to pass
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync((Vehicle?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify that VIN check was not attempted since vehicle doesn't exist
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();

        // Setup validation to fail
        SetupInvalidValidation(command, "Name", "Vehicle name is required");

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _updateVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        Assert.Contains("Vehicle name is required", exception.Message);

        // Verify that no repository methods were called due to validation failure
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}