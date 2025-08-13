using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Command.RemoveVehicleFromServiceProgram;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Moq;

namespace Application.Test.ServicePrograms.CommandTest.RemoveVehicleFromServiceProgram;

public class RemoveVehicleFromServiceProgramCommandHandlerTest
{
    private readonly RemoveVehicleFromServiceProgramCommandHandler _commandHandler;
    private readonly Mock<IXrefServiceProgramVehicleRepository> _mockXrefRepository = new();
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<IVehicleRepository> _mockVehicleRepository = new();
    private readonly Mock<ISender> _mockSender = new();
    private readonly Mock<IValidator<RemoveVehicleFromServiceProgramCommand>> _mockValidator = new();

    public RemoveVehicleFromServiceProgramCommandHandlerTest()
    {
        var mockLogger = new Mock<IAppLogger<RemoveVehicleFromServiceProgramCommandHandler>>();
        _commandHandler = new RemoveVehicleFromServiceProgramCommandHandler(
            _mockXrefRepository.Object,
            _mockServiceProgramRepository.Object,
            _mockVehicleRepository.Object,
            _mockSender.Object,
            _mockValidator.Object,
            mockLogger.Object
        );
    }

    private void SetupValidValidation(RemoveVehicleFromServiceProgramCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(RemoveVehicleFromServiceProgramCommand command, string propertyName = nameof(RemoveVehicleFromServiceProgramCommand.ServiceProgramID), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handle_Should_Remove_Xref_When_Valid()
    {
        // Arrange
        var command = new RemoveVehicleFromServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = new ServiceProgram
        {
            ID = 1,
            Name = "Test Service Program Name",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        var vehicle = new Vehicle
        {
            ID = 2,
            Name = "Test Vehicle Name",
            Make = "Test Vehicle Make",
            Model = "Test Vehicle Model",
            Year = 2020,
            VIN = "VIN",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = DateTime.UtcNow,
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Base",
            Mileage = 0,
            FuelCapacity = 0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 0,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleGroup = null!,
            User = null,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = []
        };
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(vehicle);
        _mockXrefRepository.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(true);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal((1, 2), result);
        _mockXrefRepository.Verify(r => r.RemoveAsync(1, 2), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Xref_Not_Found()
    {
        // Arrange
        var command = new RemoveVehicleFromServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = new ServiceProgram
        {
            ID = 1,
            Name = "Test Service Program Name",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        var vehicle = new Vehicle
        {
            ID = 2,
            Name = "Test Vehicle Name",
            Make = "Test Vehicle Make",
            Model = "Test Vehicle Model",
            Year = 2020,
            VIN = "VIN",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = DateTime.UtcNow,
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Base",
            Mileage = 0,
            FuelCapacity = 0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 0,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Test",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleGroup = null!,
            User = null,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = []
        };
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(vehicle);
        _mockXrefRepository.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_ServiceProgram_Not_Found()
    {
        // Arrange
        var command = new RemoveVehicleFromServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ServiceProgram?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Vehicle_Not_Found()
    {
        // Arrange
        var command = new RemoveVehicleFromServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = new ServiceProgram
        {
            ID = 1,
            Name = "Test",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Vehicle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = new RemoveVehicleFromServiceProgramCommand(1, 2);
        SetupInvalidValidation(command);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }
}