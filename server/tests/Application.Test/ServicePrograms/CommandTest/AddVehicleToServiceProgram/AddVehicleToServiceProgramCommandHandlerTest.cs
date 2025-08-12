using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.ServicePrograms.CommandTest.AddVehicleToServiceProgram;

public class AddVehicleToServiceProgramCommandHandlerTest
{
    private readonly AddVehicleToServiceProgramCommandHandler _commandHandler;
    private readonly Mock<IXrefServiceProgramVehicleRepository> _mockXrefRepository = new();
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<IVehicleRepository> _mockVehicleRepository = new();
    private readonly Mock<IValidator<AddVehicleToServiceProgramCommand>> _mockValidator = new();

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public AddVehicleToServiceProgramCommandHandlerTest()
    {
        var mockLogger = new Mock<IAppLogger<AddVehicleToServiceProgramCommandHandler>>();
        var mapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile<XrefServiceProgramVehicleMappingProfile>()));

        _commandHandler = new AddVehicleToServiceProgramCommandHandler(
            _mockXrefRepository.Object,
            _mockServiceProgramRepository.Object,
            _mockVehicleRepository.Object,
            _mockValidator.Object,
            mockLogger.Object,
            mapper
        );
    }

    private void SetupValidValidation(AddVehicleToServiceProgramCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(AddVehicleToServiceProgramCommand command, string propertyName = nameof(AddVehicleToServiceProgramCommand.ServiceProgramID), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    private static ServiceProgram CreateServiceProgram(int id = 1, bool isActive = true) => new()
    {
        ID = id,
        Name = "Test Service Program Name",
        IsActive = isActive,
        CreatedAt = FixedDate,
        UpdatedAt = FixedDate,
        ServiceSchedules = [],
        XrefServiceProgramVehicles = []
    };

    private static Vehicle CreateVehicle(int id = 1, VehicleStatusEnum status = VehicleStatusEnum.ACTIVE) => new()
    {
        ID = id,
        Name = "Test Vehicle Name",
        Make = "Toyota",
        Model = "Corolla",
        Year = 2020,
        VIN = "VIN123456789",
        LicensePlate = "ABC123",
        LicensePlateExpirationDate = FixedDate.AddYears(1),
        VehicleType = VehicleTypeEnum.CAR,
        VehicleGroupID = 1,
        Trim = "LE",
        Mileage = 10000,
        FuelCapacity = 50,
        FuelType = FuelTypeEnum.PETROL,
        PurchaseDate = FixedDate.AddYears(-1),
        PurchasePrice = 20000,
        Status = status,
        Location = "London",
        CreatedAt = FixedDate,
        UpdatedAt = FixedDate,
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

    [Fact]
    public async Task Handle_Should_Add_Xref_When_Valid()
    {
        // Arrange
        var command = new AddVehicleToServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = CreateServiceProgram(1, true);
        var vehicle = CreateVehicle(2, VehicleStatusEnum.ACTIVE);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(vehicle);
        _mockXrefRepository.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(false);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal((1, 2), result);
        _mockXrefRepository.Verify(r => r.AddAsync(It.Is<XrefServiceProgramVehicle>(x => x.ServiceProgramID == 1 && x.VehicleID == 2)), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_DuplicateEntityException_When_Xref_Exists()
    {
        // Arrange
        var command = new AddVehicleToServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = CreateServiceProgram(1, true);
        var vehicle = CreateVehicle(2, VehicleStatusEnum.ACTIVE);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(vehicle);
        _mockXrefRepository.Setup(r => r.ExistsAsync(1, 2)).ReturnsAsync(true);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_ServiceProgram_Not_Found()
    {
        // Arrange
        var command = new AddVehicleToServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync((ServiceProgram?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_ServiceProgram_Not_Active()
    {
        // Arrange
        var command = new AddVehicleToServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = CreateServiceProgram(1, false);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Vehicle_Not_Found()
    {
        // Arrange
        var command = new AddVehicleToServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = CreateServiceProgram(1, true);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync((Vehicle?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Vehicle_Not_Active()
    {
        // Arrange
        var command = new AddVehicleToServiceProgramCommand(1, 2);
        SetupValidValidation(command);
        var serviceProgram = CreateServiceProgram(1, true);
        var vehicle = CreateVehicle(2, VehicleStatusEnum.INACTIVE);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(serviceProgram);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(2)).ReturnsAsync(vehicle);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }
}