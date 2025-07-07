using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.FuelLogging.Command.CreateFuelPurchase;




using Application.MappingProfiles;


using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Application.Test.CreateFuelPurchases;

public class CreateFuelPurchaseCommandHandlerTest
{
    private readonly CreateFuelPurchaseCommandHandler _createFuelPurchasesCommandHandler;
    private readonly Mock<IFuelPurchaseRepository> _mockFuelPurchasesRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IValidator<CreateFuelPurchaseCommand>> _mockValidator;
    private readonly Mock<IAppLogger<CreateFuelPurchaseCommandHandler>> _mockLogger;

    public CreateFuelPurchaseCommandHandlerTest()
    {
        _mockFuelPurchasesRepository = new();
        _mockVehicleRepository = new();
        _mockUserRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<FuelPurchaseMappingProfile>());
        var mapper = config.CreateMapper();

        _createFuelPurchasesCommandHandler = new(
            _mockFuelPurchasesRepository.Object,
            _mockVehicleRepository.Object,
            _mockUserRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            mapper);
    }

    private static CreateFuelPurchaseCommand CreateValidCommand(
        int vehicleId = 1214,
        string purchasedByUserId = "1",
        DateTime? purchaseDate = null,
        double odometerReading = 150000.75,
        double volume = 90.7,
        decimal pricePerUnit = 5.19m,
        decimal totalCost = 470.733m,
        string fuelStation = "Test Fuel Station",
        string receiptNumber = "Test Receipt Number",
        string? notes = ""
    )
    {
        var actualPurchaseDate = purchaseDate ?? new DateTime(2025, 1, 1);
        return new CreateFuelPurchaseCommand(vehicleId, purchasedByUserId, actualPurchaseDate, odometerReading, volume, pricePerUnit, totalCost, fuelStation, receiptNumber, notes);
    }

    [Fact]
    public async Task Handle_Should_Return_FuelPurchasesID_On_Success()
    {
        // Given
        var command = CreateValidCommand();

        var expectedFuelPurchases = new FuelPurchase()
        {
            ID = 1214,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleId = command.VehicleId,
            PurchasedByUserId = command.PurchasedByUserId,
            PurchaseDate = command.PurchaseDate,
            OdometerReading = command.OdometerReading,
            Volume = command.Volume,
            PricePerUnit = command.PricePerUnit,
            TotalCost = command.TotalCost,
            FuelStation = command.FuelStation,
            ReceiptNumber = command.ReceiptNumber,
            Notes = command.Notes,
            Vehicle = null!,
            User = null!
        };

        _mockFuelPurchasesRepository.Setup(r => r.IsValidOdometerReading(command.VehicleId, command.OdometerReading)).ReturnsAsync(true);
        _mockFuelPurchasesRepository.Setup(r => r.IsReceiptNumberUniqueAsync(command.ReceiptNumber)).ReturnsAsync(true);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);

        _mockFuelPurchasesRepository.Setup(repo => repo.AddAsync(It.IsAny<FuelPurchase>())).ReturnsAsync(expectedFuelPurchases);
        _mockFuelPurchasesRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateFuelPurchaseCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // When
        var result = await _createFuelPurchasesCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedFuelPurchases.ID, result);
        _mockFuelPurchasesRepository.Verify(repo => repo.AddAsync(It.IsAny<FuelPurchase>()), Times.Once);
        _mockFuelPurchasesRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _mockValidator.Verify(validator => validator.ValidateAsync(command, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_On_VehicleId_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();

        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("VehicleId", "Vehicle ID not found"));

        // Setup mocks (though they won't be called due to validation failure)
        _mockFuelPurchasesRepository.Setup(r => r.IsValidOdometerReading(command.VehicleId, command.OdometerReading)).ReturnsAsync(true);
        _mockFuelPurchasesRepository.Setup(r => r.IsReceiptNumberUniqueAsync(command.ReceiptNumber)).ReturnsAsync(true);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(false);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);

        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateFuelPurchaseCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validationResult);

        // When
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _createFuelPurchasesCommandHandler.Handle(command, CancellationToken.None));

        // Then
        Assert.Equal("Vehicle ID not found", exception.Message);
        _mockFuelPurchasesRepository.Verify(repo => repo.AddAsync(It.IsAny<FuelPurchase>()), Times.Never);
        _mockFuelPurchasesRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockValidator.Verify(validator => validator.ValidateAsync(command, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_On_OdometerReading_Less_Than_Last_Recorded()
    {
        // Given
        var command = CreateValidCommand();

        _mockFuelPurchasesRepository.Setup(r => r.IsValidOdometerReading(command.VehicleId, command.OdometerReading)).ReturnsAsync(false);
        _mockFuelPurchasesRepository.Setup(r => r.IsReceiptNumberUniqueAsync(command.ReceiptNumber)).ReturnsAsync(true);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);

        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateFuelPurchaseCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
        _mockFuelPurchasesRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        var validationResult = new ValidationResult();
        validationResult.Errors.Add(new ValidationFailure("OdometerReading", "New odometer reading must be greater than last recorded reading."));

        // When
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _createFuelPurchasesCommandHandler.Handle(command, CancellationToken.None));

        // Then
        Assert.Equal("New odometer reading must be greater than last recorded reading.", exception.Message);
        _mockFuelPurchasesRepository.Verify(repo => repo.AddAsync(It.IsAny<FuelPurchase>()), Times.Never);
        _mockFuelPurchasesRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockValidator.Verify(validator => validator.ValidateAsync(command, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_DuplicateEntityException_On_Duplicate_ReceiptNumber()
    {
        // Given
        var command = CreateValidCommand();

        _mockFuelPurchasesRepository.Setup(r => r.IsValidOdometerReading(command.VehicleId, command.OdometerReading)).ReturnsAsync(true);
        _mockFuelPurchasesRepository.Setup(r => r.IsReceiptNumberUniqueAsync(command.ReceiptNumber)).ReturnsAsync(false);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);

        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateFuelPurchaseCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());

        // When
        var exception = await Assert.ThrowsAsync<DuplicateEntityException>(() => _createFuelPurchasesCommandHandler.Handle(command, CancellationToken.None));

        // Then
        Assert.Equal("ReceiptNumber", exception.PropertyName);
        Assert.Equal(command.ReceiptNumber, exception.PropertyValue);
        _mockFuelPurchasesRepository.Verify(repo => repo.AddAsync(It.IsAny<FuelPurchase>()), Times.Never);
        _mockFuelPurchasesRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockValidator.Verify(validator => validator.ValidateAsync(command, CancellationToken.None), Times.Once);
    }
}