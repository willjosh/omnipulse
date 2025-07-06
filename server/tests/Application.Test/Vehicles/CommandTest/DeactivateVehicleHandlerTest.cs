using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicles.Command.DeactivateVehicle;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using Moq;

using Xunit;

namespace Application.Test.Vehicles.CommandTest;

public class DeactivateVehicleHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly DeactivateVehicleCommandHandler _deactivateVehicleCommandHandler;
    private readonly Mock<IAppLogger<DeactivateVehicleCommandHandler>> _mockLogger;

    public DeactivateVehicleHandlerTest()
    {
        _mockVehicleRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<VehicleMappingProfile>());
        var mapper = config.CreateMapper();

        _deactivateVehicleCommandHandler = new(_mockVehicleRepository.Object, _mockLogger.Object, mapper);
    }

    [Fact]
    public async Task Handler_Should_Return_VehicleID_On_Deactivating_Vehicle_In_Active()
    {
        // Given 
        var DeactivateVehicleCommand = new DeactivateVehicleCommand(
            VehicleID: 123
        );

        var ReturnedVehicle = new Vehicle()
        {
            ID = DeactivateVehicleCommand.VehicleID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "James The Toyota Corolla",
            Make = "Toyota",
            Model = "Corolla",
            Year = 2023,
            VIN = "1C3CDFBA5DD298669",
            LicensePlate = "BM1007",
            LicensePlateExpirationDate = DateTime.MaxValue,
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 0,
            Trim = "JMS",
            Mileage = 300000,
            EngineHours = 1000,
            FuelCapacity = 40,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 20000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location = "Kuala Lumpur",
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(DeactivateVehicleCommand.VehicleID)).ReturnsAsync(ReturnedVehicle);
        _mockVehicleRepository.Setup(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID));

        // When
        var result = await _deactivateVehicleCommandHandler.Handle(DeactivateVehicleCommand, CancellationToken.None);

        // Then
        Assert.Equal(DeactivateVehicleCommand.VehicleID, result);
        _mockVehicleRepository.Verify(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }


    [Fact]
    public async Task Handler_Should_Return_NotFoundException_On_InvalidVehicleID()
    {
        // Given
        var DeactivateVehicleCommand = new DeactivateVehicleCommand(
            VehicleID: 123
        );

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(DeactivateVehicleCommand.VehicleID)).ReturnsAsync((Vehicle?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            async () => await _deactivateVehicleCommandHandler.Handle(DeactivateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Return_BadRequestException_On_Deactivating_Already_Deactivated_Vehicle()
    {
        // Given
        var DeactivateVehicleCommand = new DeactivateVehicleCommand(
            VehicleID: 123
        );

        var ReturnedVehicle = new Vehicle()
        {
            ID = DeactivateVehicleCommand.VehicleID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "James The Toyota Corolla",
            Make = "Toyota",
            Model = "Corolla",
            Year = 2023,
            VIN = "1C3CDFBA5DD298669",
            LicensePlate = "BM1007",
            LicensePlateExpirationDate = DateTime.MaxValue,
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 0,
            Trim = "JMS",
            Mileage = 300000,
            EngineHours = 1000,
            FuelCapacity = 40,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 20000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.INACTIVE,
            Location = "Kuala Lumpur",
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(DeactivateVehicleCommand.VehicleID)).ReturnsAsync(ReturnedVehicle);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _deactivateVehicleCommandHandler.Handle(DeactivateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Return_VehicleID_On_Deactivating_Vehicle_In_Maintenance()
    {
        // Given 
        var DeactivateVehicleCommand = new DeactivateVehicleCommand(
            VehicleID: 123
        );

        var ReturnedVehicle = new Vehicle()
        {
            ID = DeactivateVehicleCommand.VehicleID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "James The Toyota Corolla",
            Make = "Toyota",
            Model = "Corolla",
            Year = 2023,
            VIN = "1C3CDFBA5DD298669",
            LicensePlate = "BM1007",
            LicensePlateExpirationDate = DateTime.MaxValue,
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 0,
            Trim = "JMS",
            Mileage = 300000,
            EngineHours = 1000,
            FuelCapacity = 40,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 20000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.MAINTENANCE,
            Location = "Kuala Lumpur",
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(DeactivateVehicleCommand.VehicleID)).ReturnsAsync(ReturnedVehicle);
        _mockVehicleRepository.Setup(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID));

        // When
        var result = await _deactivateVehicleCommandHandler.Handle(DeactivateVehicleCommand, CancellationToken.None);

        // Then
        Assert.Equal(DeactivateVehicleCommand.VehicleID, result);
        _mockVehicleRepository.Verify(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_VehicleID_On_Deactivating_Vehicle_In_Out_Of_Service()
    {
        // Given 
        var DeactivateVehicleCommand = new DeactivateVehicleCommand(
            VehicleID: 123
        );

        var ReturnedVehicle = new Vehicle()
        {
            ID = DeactivateVehicleCommand.VehicleID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "James The Toyota Corolla",
            Make = "Toyota",
            Model = "Corolla",
            Year = 2023,
            VIN = "1C3CDFBA5DD298669",
            LicensePlate = "BM1007",
            LicensePlateExpirationDate = DateTime.MaxValue,
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 0,
            Trim = "JMS",
            Mileage = 300000,
            EngineHours = 1000,
            FuelCapacity = 40,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 20000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.OUT_OF_SERVICE,
            Location = "Kuala Lumpur",
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(DeactivateVehicleCommand.VehicleID)).ReturnsAsync(ReturnedVehicle);
        _mockVehicleRepository.Setup(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID));

        // When
        var result = await _deactivateVehicleCommandHandler.Handle(DeactivateVehicleCommand, CancellationToken.None);

        // Then
        Assert.Equal(DeactivateVehicleCommand.VehicleID, result);
        _mockVehicleRepository.Verify(r => r.VehicleDeactivateAsync(DeactivateVehicleCommand.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

}