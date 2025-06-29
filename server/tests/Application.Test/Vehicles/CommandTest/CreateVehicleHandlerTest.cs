using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using Moq;

namespace Application.Test.Vehicles.CommandTest;

public class CreateVehicleHandlerTest
{
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly CreateVehicleCommandHandler _createVehicleCommandHandler;
    private readonly Mock<IAppLogger<CreateVehicleCommandHandler>> _mockLogger;

    private readonly Mock<IUserRepository> _mockUserRepository;

    public CreateVehicleHandlerTest()
    {
        _mockUserRepository = new();
        _mockVehicleRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _createVehicleCommandHandler = new(_mockVehicleRepository.Object, mapper, _mockLogger.Object);
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
        string TechnicianID = "f5095320-3f0a-42a1-8de6-b6792c980213"
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
            Location: location
        );
    }

    [Fact(Skip = "No Implementation yet")]
    public async Task Handler_Should_Return_VehicleID_On_Success()
    {
        // Given 
        var command = CreateValidCommand();
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
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };

        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(false);
        _mockVehicleRepository.Setup(r => r.AddAsync(It.IsAny<Vehicle>())).ReturnsAsync(expectedVehicle);
        _mockVehicleRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createVehicleCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedVehicle.ID, result);
        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_VIN()
    {
        // Given 
        var command = CreateValidCommand();
        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Throw_BadRequestException_On_Negative_Year()
    {
        // Given 
        var command = CreateValidCommand(year: -1);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData("AAAAAAAAAAAAAAAA")]     // 16 chars - too short
    [InlineData("AAAAAAAAAAAAAAAAAA")]   // 18 chars - too long
    [InlineData("")]                     // Empty
    [InlineData("123456789012345")]      // 15 chars - too short
    [InlineData("123456789012345678")]   // 18 chars - too long
    public async Task Handler_Should_Throw_BadRequestException_On_Invalid_VIN(string invalidVin)
    {
        // Given 
        var command = CreateValidCommand(vin: invalidVin);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(51)]   // Exceeds 50 limit
    [InlineData(100)]  // Way over limit
    public async Task Handler_Should_Throw_BadRequestException_On_Name_Exceeds_MaxLength(int nameLength)
    {
        // Given 
        var command = CreateValidCommand(name: new string('A', nameLength));

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(31)]   // Exceeds 30 limit
    [InlineData(50)]   // Way over limit
    public async Task Handler_Should_Throw_BadRequestException_On_Make_Exceeds_MaxLength(int makeLength)
    {
        // Given 
        var command = CreateValidCommand(make: new string('A', makeLength));

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(51)]   // Exceeds 50 limit
    [InlineData(100)]  // Way over limit
    public async Task Handler_Should_Throw_BadRequestException_On_Model_Exceeds_MaxLength(int modelLength)
    {
        // Given 
        var command = CreateValidCommand(model: new string('A', modelLength));

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(51)]   // Exceeds 50 limit
    [InlineData(100)]  // Way over limit
    public async Task Handler_Should_Throw_BadRequestException_On_Trim_Exceeds_MaxLength(int trimLength)
    {
        // Given 
        var command = CreateValidCommand(trim: new string('A', trimLength));

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(101)]  // Exceeds 100 limit
    [InlineData(200)]  // Way over limit
    public async Task Handler_Should_Throw_BadRequestException_On_Location_Exceeds_MaxLength(int locationLength)
    {
        // Given 
        var command = CreateValidCommand(location: new string('A', locationLength));

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(16)]   // Too short
    [InlineData(18)]   // Too long
    public async Task Handler_Should_Throw_BadRequestException_On_VIN_Invalid_Length(int vinLength)
    {
        // Given 
        var command = CreateValidCommand(vin: new string('A', vinLength));

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(11)]   // Exceeds 10 limit (assuming max 10)
    [InlineData(20)]   // Way over limit
    public async Task Handler_Should_Throw_BadRequestException_On_LicensePlate_Exceeds_MaxLength(int licensePlateLength)
    {
        // Given 
        var command = CreateValidCommand(licensePlate: new string('A', licensePlateLength));

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Throw_BadRequestException_On_LicenseExpiration_Before_PurchaseDate()
    {
        // Given 
        var purchaseDate = DateTime.UtcNow;
        var expirationDate = purchaseDate.AddDays(-1); // Expires before purchase

        var command = CreateValidCommand(
            purchaseDate: purchaseDate,
            licenseExpiration: expirationDate
        );

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(0)]    // Zero is invalid ID format
    [InlineData(-1)]   // Negative is invalid ID format
    [InlineData(-100)] // Negative is invalid ID format
    public async Task Handler_Should_Throw_BadRequestException_On_Invalid_GroupID_Format(int invalidGroupId)
    {
        // Given 
        var command = CreateValidCommand(vehicleGroupID: invalidGroupId);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Throw_NotFoundException_On_Nonexistent_GroupID()
    {
        // Given - valid ID format but group doesn't exist
        var command = CreateValidCommand(vehicleGroupID: 999);

        // Mock that VIN doesn't exist (so we get past VIN validation)
        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(false);

        // Mock that group doesn't exist
        // Note: You'll need to add IVehicleGroupRepository to your handler
        // _mockVehicleGroupRepository.Setup(r => r.ExistsAsync(999)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(command.VIN), Times.Once);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Throw_BadRequestException_On_Negative_Mileage(int mileage)
    {
        // Given 
        var command = CreateValidCommand(mileage: mileage);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Throw_BadRequestException_On_Negative_EngineHours(int engineHours)
    {
        // Given 
        var command = CreateValidCommand(engineHours: engineHours);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Throw_BadRequestException_On_Negative_FuelCapacity(int fuelCapacity)
    {
        // Given 
        var command = CreateValidCommand(fuelCapacity: fuelCapacity);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Handler_Should_Throw_BadRequestException_On_Negative_PurchasePrice(decimal purchasePrice)
    {
        // Given 
        var command = CreateValidCommand(purchasePrice: purchasePrice);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Handler_Should_Throw_BadRequestException_On_Empty_Required_Fields(string invalidValue)
    {
        // Test multiple required fields
        var commands = new[]
        {
            CreateValidCommand(name: invalidValue),
            CreateValidCommand(make: invalidValue),
            CreateValidCommand(model: invalidValue),
            CreateValidCommand(vin: invalidValue),
            CreateValidCommand(location: invalidValue)
        };

        foreach (var command in commands)
        {
            // When & Then
            await Assert.ThrowsAsync<BadRequestException>(
                () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
            );
        }

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory(Skip = "Not yet implemented")]
    [InlineData(1800)] // Too old
    [InlineData(2100)] // Future year
    public async Task Handler_Should_Throw_BadRequestException_On_Invalid_Year_Range(int year)
    {
        // Given 
        var command = CreateValidCommand(year: year);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Throw_NotFoundException_On_Nonexistent_TechnicianID()
    {
        // Given
        var command = CreateValidCommand();

        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(false);

        // Mock that technician doesn't exist
        _mockUserRepository.Setup(r => r.ExistsAsync("f5095320-3f0a-42a1-8de6-b6792c980213")).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );
    }

    // Additional tests for proper validation order
    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Validate_Input_Before_Checking_VIN_Existence()
    {
        // Given - invalid input that should fail before VIN check
        var command = CreateValidCommand(year: -1);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify VIN check was never called due to early validation failure
        _mockVehicleRepository.Verify(r => r.VinExistAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact(Skip = "Not yet implemented")]
    public async Task Handler_Should_Check_VIN_Existence_Before_Database_Operations()
    {
        // Given - duplicate VIN
        var command = CreateValidCommand();
        _mockVehicleRepository.Setup(r => r.VinExistAsync(command.VIN)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _createVehicleCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify that no database changes were attempted
        _mockVehicleRepository.Verify(r => r.AddAsync(It.IsAny<Vehicle>()), Times.Never);
        _mockVehicleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}