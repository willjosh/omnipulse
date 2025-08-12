using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.FuelLogging.Command.UpdateFuelPurchase;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.FuelPurchases.CommandTest.UpdateFuelPurchase;

public class UpdateFuelPurchaseCommandHandlerTest
{
    private readonly UpdateFuelPurchaseCommandHandler _handler;
    private readonly Mock<IFuelPurchaseRepository> _mockFuelPurchaseRepository = new();
    private readonly Mock<IVehicleRepository> _mockVehicleRepository = new();
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IValidator<UpdateFuelPurchaseCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<UpdateFuelPurchaseCommandHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    public UpdateFuelPurchaseCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<FuelPurchaseMappingProfile>());
        _mapper = config.CreateMapper();
        _handler = new UpdateFuelPurchaseCommandHandler(
            _mockFuelPurchaseRepository.Object,
            _mockVehicleRepository.Object,
            _mockUserRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    private static UpdateFuelPurchaseCommand CreateValidCommand(
        int fuelPurchaseId = 1,
        int vehicleId = 1,
        string purchasedByUserId = "00000000-0000-0000-0000-000000000001",
        DateTime? purchaseDate = null,
        double odometerReading = 150000.75,
        double volume = 50.0,
        decimal pricePerUnit = 2.99m,
        string fuelStation = "Shell",
        string receiptNumber = "RCPT-001",
        string? notes = "Notes")
    {
        return new UpdateFuelPurchaseCommand(
            FuelPurchaseId: fuelPurchaseId,
            VehicleId: vehicleId,
            PurchasedByUserId: purchasedByUserId,
            PurchaseDate: purchaseDate ?? DateTime.UtcNow.AddDays(-1),
            OdometerReading: odometerReading,
            Volume: volume,
            PricePerUnit: pricePerUnit,
            FuelStation: fuelStation,
            ReceiptNumber: receiptNumber,
            Notes: notes
        );
    }

    private void SetupValidValidation(UpdateFuelPurchaseCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>())).ReturnsAsync(validResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ID_On_Success()
    {
        // ===== Arrange =====
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var existing = new FuelPurchase
        {
            ID = command.FuelPurchaseId,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-9),
            VehicleId = 1,
            PurchasedByUserId = "00000000-0000-0000-0000-000000000001",
            PurchaseDate = DateTime.UtcNow.AddDays(-2),
            OdometerReading = 140000,
            Volume = 30,
            PricePerUnit = 2.5m,
            TotalCost = 75m,
            FuelStation = "Shell",
            ReceiptNumber = "OLD-001",
            Notes = "Old",
            Vehicle = null!,
            User = null!
        };

        _mockFuelPurchaseRepository.Setup(r => r.GetByIdAsync(command.FuelPurchaseId)).ReturnsAsync(existing);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockFuelPurchaseRepository.Setup(r => r.IsReceiptNumberUniqueAsync(command.ReceiptNumber)).ReturnsAsync(true);
        _mockFuelPurchaseRepository.Setup(r => r.IsValidOdometerReading(command.VehicleId, command.OdometerReading)).ReturnsAsync(true);
        _mockFuelPurchaseRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // ===== Act =====
        var result = await _handler.Handle(command, CancellationToken.None);

        // ===== Assert =====
        Assert.Equal(existing.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockFuelPurchaseRepository.Verify(r => r.GetByIdAsync(command.FuelPurchaseId), Times.Once);
        _mockFuelPurchaseRepository.Verify(r => r.Update(It.IsAny<FuelPurchase>()), Times.Once);
        _mockFuelPurchaseRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_When_Entity_Not_Found()
    {
        // ===== Arrange =====
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockFuelPurchaseRepository.Setup(r => r.GetByIdAsync(command.FuelPurchaseId)).ReturnsAsync((FuelPurchase?)null);

        // ===== Act =====
        Task Act() => _handler.Handle(command, CancellationToken.None);

        // ===== Assert =====
        await Assert.ThrowsAsync<EntityNotFoundException>(Act);
    }

    [Fact]
    public async Task Handler_Should_Throw_When_Receipt_Duplicate_On_Change()
    {
        // ===== Arrange =====
        var command = CreateValidCommand(receiptNumber: "NEW-RCPT");
        SetupValidValidation(command);

        var existing = new FuelPurchase
        {
            ID = command.FuelPurchaseId,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-9),
            VehicleId = command.VehicleId,
            PurchasedByUserId = command.PurchasedByUserId,
            PurchaseDate = command.PurchaseDate.AddDays(-1),
            OdometerReading = 140000,
            Volume = 30,
            PricePerUnit = 2.5m,
            TotalCost = 75m,
            FuelStation = "Shell",
            ReceiptNumber = "OLD-RCPT",
            Vehicle = null!,
            User = null!
        };

        _mockFuelPurchaseRepository.Setup(r => r.GetByIdAsync(command.FuelPurchaseId)).ReturnsAsync(existing);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.PurchasedByUserId)).ReturnsAsync(true);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockFuelPurchaseRepository.Setup(r => r.IsReceiptNumberUniqueAsync(command.ReceiptNumber)).ReturnsAsync(false);

        // ===== Act =====
        Task Act() => _handler.Handle(command, CancellationToken.None);

        // ===== Assert =====
        await Assert.ThrowsAsync<DuplicateEntityException>(Act);
    }
}