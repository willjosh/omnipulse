using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.Vehicles.CommandTest;

public class CreateVehicleHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly CreateVehicleCommandHandler _createVehicleCommandHandler;
    private readonly Mock<IAppLogger<CreateVehicleCommandHandler>> _mockLogger;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IVehicleGroupRepository> _mockVehicleGroupRepository;
    private readonly Mock<IValidator<CreateVehicleCommand>> _mockValidator;

    public CreateVehicleHandlerTest()
    {
        _mockVehicleRepository = new();
        _mockLogger = new();
        _mockUserRepository = new();
        _mockVehicleGroupRepository = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<VehicleMappingProfile>());
        var mapper = config.CreateMapper();

        _createVehicleCommandHandler = new(_mockVehicleRepository.Object, _mockUserRepository.Object, _mockVehicleGroupRepository.Object, mapper, _mockLogger.Object, _mockValidator.Object);
    }

    // Helper method to create valid command with minimal data
    private CreateVehicleCommand CreateValidCommand(
        string name = "Test Vehicle",
        string make = "Toyota",
        string model = "Corolla",
        int year = 2023,
        string vin = "1C3CDFBA5DD298669",
        string licensePlate = "TEST123",
        DateTime? licenseExpiration = null,
        Domain.Entities.Enums.VehicleTypeEnum vehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
        int vehicleGroupID = 1,
        string trim = "Base",
        int mileage = 0,
        int engineHours = 0,
        int fuelCapacity = 50,
        Domain.Entities.Enums.FuelTypeEnum fuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
        DateTime? purchaseDate = null,
        decimal purchasePrice = 25000,
        Domain.Entities.Enums.VehicleStatusEnum status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
        string location = "Test Location",
        string? AssignedTechnicianID = null
    )
    {
        return new CreateVehicleCommand(
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
            PurchaseDate: purchaseDate ?? DateTime.UtcNow,
            PurchasePrice: purchasePrice,
            Status: status,
            Location: location,
            AssignedTechnicianID
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(CreateVehicleCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(CreateVehicleCommand command, string propertyName = "Name", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_VehicleID_On_Success()
    {
        // Given 
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var expectedVehicle = new Vehicle()
        {
            ID = 123,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = command.Name,
            Make = command.Make,
            Model = command.Model,
            Year = command.Year,
            VIN = command.VIN,
            LicensePlate = command.LicensePlate,
            LicensePlateExpirationDate = command.LicensePlateExpirationDate,
            VehicleType = command.VehicleType,
            VehicleGroupID = command.VehicleGroupID,
            Trim = command.Trim,
            Mileage = command.Mileage,
            EngineHours = command.EngineHours,
            FuelCapacity = command.FuelCapacity,
            FuelType = command.FuelType,
            PurchaseDate = command.PurchaseDate,
            PurchasePrice = command.PurchasePrice,
            Status = command.Status,
            Location = command.Location,
            VehicleGroup = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(false);
        _mockVehicleGroupRepository.Setup(r => r.ExistsAsync(command.VehicleGroupID)).ReturnsAsync(true);
        _mockVehicleRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>())).ReturnsAsync(expectedVehicle);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createVehicleCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedVehicle.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_VIN()
    {
        // Given 
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_NonExistent_GroupID()
    {
        // Given 
        var command = CreateValidCommand(vehicleGroupID: 123);
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(false);
        _mockVehicleGroupRepository.Setup(r => r.ExistsAsync(command.VehicleGroupID)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleGroupRepository.Verify(r => r.ExistsAsync(command.VehicleGroupID), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_Nonexistent_TechnicianID()
    {
        // Given
        var technicianId = "f5095320-3f0a-42a1-8de6-b6792c980213";
        var command = CreateValidCommand(AssignedTechnicianID: technicianId);
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(false);
        _mockVehicleGroupRepository.Setup(r => r.ExistsAsync(command.VehicleGroupID)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(technicianId)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleGroupRepository.Verify(r => r.ExistsAsync(command.VehicleGroupID), Times.Once);
        _mockUserRepository.Verify(r => r.ExistsAsync(technicianId), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "Year", "Year must be valid");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify validation was called but business logic was not
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Validate_Input_Before_Checking_VIN_Existence()
    {
        // Given - mock validation failure
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "VIN", "VIN is invalid");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify validation runs before business logic
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Check_VIN_Existence_Before_Database_Operations()
    {
        // Given - duplicate VIN
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify that no database changes were attempted
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}