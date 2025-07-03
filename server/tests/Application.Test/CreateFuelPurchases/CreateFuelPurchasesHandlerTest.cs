using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.FuelLogging.Command.CreateFuelPurchase;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.FuelPurchases.CommandTest;

public class CreateFuelPurchasesHandlerTest
{
    private readonly Mock<IFuelPurchaseRepository> _mockFuelPurchasesRepository;
    private readonly CreateFuelPurchaseCommandHandler _createFuelPurchasesCommandHandler;
    private readonly Mock<IAppLogger<CreateFuelPurchaseCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<CreateFuelPurchaseCommand>> _mockValidator;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;

    public CreateFuelPurchasesHandlerTest()
    {
        _mockFuelPurchasesRepository = new();
        _mockLogger = new();
        _mockValidator = new();
        _mockVehicleRepository = new();
        _mockUserRepository = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<FuelPurchaseMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _createFuelPurchasesCommandHandler = new(_mockFuelPurchasesRepository.Object, _mockVehicleRepository.Object, _mockUserRepository.Object, mapper, _mockLogger.Object, _mockValidator.Object);
    }

    private CreateFuelPurchaseCommand CreateValidCommand(
        int VehicleId = 1214,
        string PurchasedByUserId = "1",
        DateTime PurchaseDate = default,
        double OdometerReading = 150000.75,
        double Volume = 90.7,
        decimal PricePerUnit = 5.19m,
        decimal TotalCost = 470.733m,
        string FuelStation = "Test Fuel Station",
        string ReceiptNumber = "Test Receipt Number",
        string? Notes = ""
    )
    {
        return new CreateFuelPurchaseCommand(VehicleId, PurchasedByUserId, new DateTime(2025, 2, 1), OdometerReading, Volume, PricePerUnit, TotalCost, FuelStation, ReceiptNumber, Notes);
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
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);

        _mockFuelPurchasesRepository.Setup(repo => repo.AddAsync(It.IsAny<FuelPurchase>())).ReturnsAsync(expectedFuelPurchases);
        _mockFuelPurchasesRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateFuelPurchaseCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

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

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("VehicleId", "Vehicle ID not found"));

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
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);

        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateFuelPurchaseCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
        _mockFuelPurchasesRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        var validationResult = new FluentValidation.Results.ValidationResult();
        validationResult.Errors.Add(new FluentValidation.Results.ValidationFailure("OdometerReading", "New odometer reading must be greater than last recorded reading."));

        // When
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _createFuelPurchasesCommandHandler.Handle(command, CancellationToken.None));

        // Then
        Assert.Equal("New odometer reading must be greater than last recorded reading.", exception.Message);
        _mockFuelPurchasesRepository.Verify(repo => repo.AddAsync(It.IsAny<FuelPurchase>()), Times.Never);
        _mockFuelPurchasesRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockValidator.Verify(validator => validator.ValidateAsync(command, CancellationToken.None), Times.Once);
        ;
    }
}