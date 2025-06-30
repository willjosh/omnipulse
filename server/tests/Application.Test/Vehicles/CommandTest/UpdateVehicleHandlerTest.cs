using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicle.Command.UpdateVehicle;
using Application.Features.Vehicles.Command.UpdateVehicle;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using Moq;
using Xunit;

namespace Application.Test.Vehicles.CommandTest;

public class UpdateVehicleHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly UpdateVehicleCommandHandler _updateVehicleCommandHandler;
    private readonly Mock<IAppLogger<UpdateVehicleCommandHandler>> _mockLogger;

    public UpdateVehicleHandlerTest()
    {
        _mockVehicleRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _updateVehicleCommandHandler = new(_mockVehicleRepository.Object, mapper, _mockLogger.Object);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_VehicleID_On_Successfully_Updating_Vehicle()
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        var existingVehicle = new Vehicle()
        {
            ID = updateVehicleCommand.VehicleID,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Name = "Old Name",
            Make = "Old Make",
            Model = "Old Model",
            Year = 2020,
            VIN = "OLD1234567890123",
            LicensePlate = "OLD123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddDays(-10),
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.TRUCK,
            VehicleGroupID = 2,
            Trim = "Old Trim",
            Mileage = 200000,
            EngineHours = 800,
            FuelCapacity = 30,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.DIESEL,
            PurchaseDate = DateTime.UtcNow.AddYears(-3),
            PurchasePrice = 15000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.INACTIVE,
            Location = "Old Location",
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(updateVehicleCommand.VehicleID)).ReturnsAsync(existingVehicle);
        _mockVehicleRepository.Setup(r => r.VinExistAsync(updateVehicleCommand.VIN)).ReturnsAsync(false);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None);

        // Then
        Assert.Equal(updateVehicleCommand.VehicleID, result);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(updateVehicleCommand.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        // Verify that Update was called with a vehicle containing the updated values
        _mockVehicleRepository.Verify(
            r => r.Update(It.Is<Vehicle>(v =>
                v.ID == updateVehicleCommand.VehicleID &&
                v.Name == updateVehicleCommand.Name &&
                v.Make == updateVehicleCommand.Make &&
                v.VIN == updateVehicleCommand.VIN &&
                v.LicensePlate == updateVehicleCommand.LicensePlate &&
                v.Mileage == updateVehicleCommand.Mileage)),
            Times.Once);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_EntityNotFoundException_On_Invalid_VehicleID()
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 999,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(updateVehicleCommand.VehicleID)).ReturnsAsync((Vehicle?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(updateVehicleCommand.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_DuplicateEntityException_On_Duplicate_VIN_From_Different_Vehicle()
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "DUPLICATE1234567890", // This VIN exists in another vehicle
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        var existingVehicle = new Vehicle()
        {
            ID = updateVehicleCommand.VehicleID,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Name = "Existing Vehicle",
            Make = "Toyota",
            Model = "Camry",
            Year = 2021,
            VIN = "ORIGINAL123456789", // Different VIN than the update command
            LicensePlate = "OLD123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddDays(365),
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Original Trim",
            Mileage = 100000,
            EngineHours = 500,
            FuelCapacity = 40,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-2),
            PurchasePrice = 20000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location = "Original Location",
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(updateVehicleCommand.VehicleID)).ReturnsAsync(existingVehicle);
        _mockVehicleRepository.Setup(r => r.VinExistAsync(updateVehicleCommand.VIN)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(updateVehicleCommand.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(updateVehicleCommand.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Allow_Update_With_Same_VIN_As_Current_Vehicle()
    {
        // Given - updating vehicle with the same VIN it already has (should be allowed)
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Name Only",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "CURRENT123456789", // Same VIN as current vehicle
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        var existingVehicle = new Vehicle()
        {
            ID = updateVehicleCommand.VehicleID,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Name = "Original Name",
            Make = "Toyota",
            Model = "Corolla",
            Year = 2021,
            VIN = "CURRENT123456789", // Same VIN as in update command
            LicensePlate = "OLD123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddDays(365),
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Original Trim",
            Mileage = 100000,
            EngineHours = 500,
            FuelCapacity = 40,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-2),
            PurchasePrice = 20000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location = "Original Location",
            VehicleGroup = It.IsAny<VehicleGroup>(),
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(updateVehicleCommand.VehicleID)).ReturnsAsync(existingVehicle);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None);

        // Then
        Assert.Equal(updateVehicleCommand.VehicleID, result);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(updateVehicleCommand.VehicleID), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never); // Should not check VIN if it's the same
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_BadRequestException_On_Negative_Year()
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: -1, // Invalid year
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "No Implementation yet")]
    [InlineData("AAAAAAAAAAAAAAAA")] // 16 characters (too short)
    [InlineData("AAAAAAAAAAAAAAAAAA")] // 18 characters (too long)
    public async Task Handler_Should_Return_BadRequestException_On_Invalid_VIN_Length(string VIN)
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: VIN, // Invalid VIN length
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "No Implementation yet")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Return_BadRequestException_On_Negative_Mileage(double mileage)
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: mileage, // Invalid mileage
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "No Implementation yet")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Return_BadRequestException_On_Negative_EngineHours(double engineHours)
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: engineHours, // Invalid engine hours
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "No Implementation yet")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Return_BadRequestException_On_Negative_FuelCapacity(double fuelCapacity)
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: fuelCapacity, // Invalid fuel capacity
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "No Implementation yet")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Return_BadRequestException_On_Negative_PurchasePrice(double purchasePrice)
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: purchasePrice, // Invalid purchase price
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_BadRequestException_On_ExceedMaxLength_Name()
    {
        // Given
        var nameExceedingLimit = new string('A', 51); // 51 characters - exceeds 50 limit

        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: nameExceedingLimit, // Exceeds max length
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_BadRequestException_On_LicensePlateExpiration_Before_PurchaseDate()
    {
        // Given
        var purchaseDate = DateTime.UtcNow;
        var expiredLicenseDate = purchaseDate.AddDays(-1); // License expired before purchase

        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: expiredLicenseDate, // Invalid expiration date
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: purchaseDate,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_BadRequestException_On_Empty_Required_Fields()
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "", // Empty required field
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: 1,
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_BadRequestException_On_Invalid_VehicleGroupID()
    {
        // Given
        var updateVehicleCommand = new UpdateVehicleCommand(
            VehicleID: 123,
            Name: "Updated Toyota Corolla",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2023,
            VIN: "1C3CDFBA5DD298669",
            LicensePlate: "BM1008",
            LicensePlateExpirationDate: DateTime.MaxValue,
            VehicleType: Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID: -1, // Invalid group ID
            Trim: "Updated Trim",
            Mileage: 350000,
            EngineHours: 1200,
            FuelCapacity: 45,
            FuelType: Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.UtcNow,
            PurchasePrice: 25000,
            Status: Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location: "Updated Location"
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            async () => await _updateVehicleCommandHandler.Handle(updateVehicleCommand, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.Update(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}
