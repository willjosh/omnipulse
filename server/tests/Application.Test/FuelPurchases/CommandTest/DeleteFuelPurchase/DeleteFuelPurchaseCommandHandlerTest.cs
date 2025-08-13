using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.FuelPurchases.Command.DeleteFuelPurchase;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.FuelPurchases.CommandTest.DeleteFuelPurchase;

public class DeleteFuelPurchaseCommandHandlerTest
{
    private readonly DeleteFuelPurchaseCommandHandler _handler;
    private readonly Mock<IFuelPurchaseRepository> _mockRepo = new();
    private readonly Mock<IValidator<DeleteFuelPurchaseCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<DeleteFuelPurchaseCommandHandler>> _mockLogger = new();

    public DeleteFuelPurchaseCommandHandlerTest()
    {
        _handler = new DeleteFuelPurchaseCommandHandler(
            _mockRepo.Object,
            _mockValidator.Object,
            _mockLogger.Object
        );
    }

    private static DeleteFuelPurchaseCommand CreateCommand(int id = 1) => new(id);

    private void SetupValid(DeleteFuelPurchaseCommand command)
    {
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult());
    }

    private void SetupInvalid(DeleteFuelPurchaseCommand command, string propertyName = nameof(DeleteFuelPurchaseCommand.FuelPurchaseID))
    {
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new ValidationResult([new ValidationFailure(propertyName, "Invalid")]));
    }

    [Fact]
    public async Task Handler_Should_Return_ID_On_Success()
    {
        // ===== Arrange =====
        var command = CreateCommand(7);
        SetupValid(command);
        var existing = new FuelPurchase
        {
            ID = 7,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleId = 1,
            PurchasedByUserId = Guid.NewGuid().ToString(),
            PurchaseDate = DateTime.UtcNow,
            OdometerReading = 1000,
            Volume = 10,
            PricePerUnit = 2m,
            TotalCost = 20m,
            FuelStation = "FS",
            ReceiptNumber = "R-1",
            Notes = null,
            Vehicle = null!,
            User = null!
        };
        _mockRepo.Setup(r => r.GetByIdAsync(command.FuelPurchaseID)).ReturnsAsync(existing);
        _mockRepo.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // ===== Act =====
        var result = await _handler.Handle(command, default);

        // ===== Assert =====
        Assert.Equal(7, result);
        _mockRepo.Verify(r => r.Delete(existing), Times.Once);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_NotFound_When_Missing()
    {
        // ===== Arrange =====
        var command = CreateCommand(999);
        SetupValid(command);
        _mockRepo.Setup(r => r.GetByIdAsync(command.FuelPurchaseID)).ReturnsAsync((FuelPurchase?)null);

        // ===== Act & Assert =====
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, default));
        _mockRepo.Verify(r => r.Delete(It.IsAny<FuelPurchase>()), Times.Never);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequest_On_Invalid()
    {
        // ===== Arrange =====
        var command = CreateCommand(0);
        SetupInvalid(command);

        // ===== Act & Assert =====
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, default));
        _mockRepo.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockRepo.Verify(r => r.Delete(It.IsAny<FuelPurchase>()), Times.Never);
        _mockRepo.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}