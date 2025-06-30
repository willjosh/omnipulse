using System;
using Application.Features.Vehicles.Command.CreateVehicle;
using Domain.Entities.Enums;
using Xunit;

namespace Application.Test.Vehicles.CommandTest.CreateVehicle;

public class CreateVehicleCommandValidatorTest
{
    private readonly CreateVehicleCommandValidator _validator;

    public CreateVehicleCommandValidatorTest()
    {
        _validator = new CreateVehicleCommandValidator();
    }

    private CreateVehicleCommand CreateValidCommand(
        string name = "Test Vehicle",
        string make = "Toyota",
        string model = "Corolla",
        int year = 2023,
        string vin = "1C3CDFBA5DD298669",
        string licensePlate = "TEST123",
        DateTime? licenseExpiration = null,
        VehicleTypeEnum vehicleType = VehicleTypeEnum.CAR,
        int vehicleGroupID = 1,
        string trim = "Base",
        double mileage = 0,
        double engineHours = 0,
        double fuelCapacity = 50,
        FuelTypeEnum fuelType = FuelTypeEnum.PETROL,
        DateTime? purchaseDate = null,
        decimal purchasePrice = 25000,
        VehicleStatusEnum status = VehicleStatusEnum.ACTIVE,
        string location = "Test Location",
        string? assignedTechnicianID = null)
    {
        return new CreateVehicleCommand(
            Name: name,
            Make: make,
            Model: model,
            Year: year,
            VIN: vin,
            LicensePlate: licensePlate,
            LicensePlateExpirationDate: licenseExpiration ?? new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            VehicleType: vehicleType,
            VehicleGroupID: vehicleGroupID,
            Trim: trim,
            Mileage: mileage,
            EngineHours: engineHours,
            FuelCapacity: fuelCapacity,
            FuelType: fuelType,
            PurchaseDate: purchaseDate ?? new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePrice: purchasePrice,
            Status: status,
            Location: location,
            AssignedTechnicianID: assignedTechnicianID
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Given
        var command = CreateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // NAME VALIDATION TESTS
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Name_Is_Empty(string invalidName)
    {
        // Given
        var command = CreateValidCommand(name: invalidName);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(101)]  // Exceeds 100 limit
    [InlineData(200)]  // Way over limit
    public async Task Validator_Should_Fail_When_Name_Exceeds_MaxLength(int nameLength)
    {
        // Given
        var command = CreateValidCommand(name: new string('A', nameLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" &&
                                           e.ErrorMessage.Contains("100 characters"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Make_Is_Empty(string invalidMake)
    {
        // Given
        var command = CreateValidCommand(make: invalidMake);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Make");
    }

    [Theory]
    [InlineData(31)]   // Exceeds 30 limit
    [InlineData(50)]   // Way over limit
    public async Task Validator_Should_Fail_When_Make_Exceeds_MaxLength(int makeLength)
    {
        // Given
        var command = CreateValidCommand(make: new string('A', makeLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Make" &&
                                           e.ErrorMessage.Contains("30 characters"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Model_Is_Empty(string invalidModel)
    {
        // Given
        var command = CreateValidCommand(model: invalidModel);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Model");
    }

    [Theory]
    [InlineData(51)]   // Exceeds 50 limit
    [InlineData(100)]  // Way over limit
    public async Task Validator_Should_Fail_When_Model_Exceeds_MaxLength(int modelLength)
    {
        // Given
        var command = CreateValidCommand(model: new string('A', modelLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Model" &&
                                           e.ErrorMessage.Contains("50 characters"));
    }

    // YEAR VALIDATION TESTS
    [Fact]
    public async Task Validator_Should_Fail_When_Year_Is_Zero()
    {
        // Given
        var command = CreateValidCommand(year: 0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Year");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(1800)]  // Too old
    [InlineData(2101)]  // Future year
    public async Task Validator_Should_Fail_When_Year_Is_Invalid_Range(int invalidYear)
    {
        // Given
        var command = CreateValidCommand(year: invalidYear);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Year");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_VIN_Is_Empty(string invalidVin)
    {
        // Given
        var command = CreateValidCommand(vin: invalidVin);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VIN");
    }

    [Theory]
    [InlineData("AAAAAAAAAAAAAAAA")]     // 16 chars - too short
    [InlineData("AAAAAAAAAAAAAAAAAA")]   // 18 chars - too long
    [InlineData("123456789012345")]      // 15 chars - too short
    [InlineData("123456789012345678")]   // 18 chars - too long
    public async Task Validator_Should_Fail_When_VIN_Has_Invalid_Length(string invalidVin)
    {
        // Given
        var command = CreateValidCommand(vin: invalidVin);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VIN");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Trim_Is_Empty(string invalidTrim)
    {
        // Given
        var command = CreateValidCommand(trim: invalidTrim);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Trim");
    }

    [Theory]
    [InlineData(51)]   // Exceeds 50 limit
    [InlineData(100)]  // Way over limit
    public async Task Validator_Should_Fail_When_Trim_Exceeds_MaxLength(int trimLength)
    {
        // Given
        var command = CreateValidCommand(trim: new string('A', trimLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Trim");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Location_Is_Empty(string invalidLocation)
    {
        // Given
        var command = CreateValidCommand(location: invalidLocation);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Location");
    }

    [Theory]
    [InlineData(101)]  // Exceeds 100 limit
    [InlineData(200)]  // Way over limit
    public async Task Validator_Should_Fail_When_Location_Exceeds_MaxLength(int locationLength)
    {
        // Given
        var command = CreateValidCommand(location: new string('A', locationLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Location");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_LicensePlate_Is_Empty(string invalidLicensePlate)
    {
        // Given
        var command = CreateValidCommand(licensePlate: invalidLicensePlate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LicensePlate");
    }

    [Theory]
    [InlineData(21)]   // Exceeds 20 limit
    [InlineData(30)]   // Way over limit
    public async Task Validator_Should_Fail_When_LicensePlate_Exceeds_MaxLength(int licensePlateLength)
    {
        // Given
        var command = CreateValidCommand(licensePlate: new string('A', licensePlateLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LicensePlate");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_Mileage_Is_Negative(double mileage)
    {
        // Given
        var command = CreateValidCommand(mileage: mileage);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Mileage");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_EngineHours_Is_Negative(double engineHours)
    {
        // Given
        var command = CreateValidCommand(engineHours: engineHours);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EngineHours");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_FuelCapacity_Is_Zero_Or_Negative(double fuelCapacity)
    {
        // Given
        var command = CreateValidCommand(fuelCapacity: fuelCapacity);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FuelCapacity");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_PurchasePrice_Is_Negative(decimal purchasePrice)
    {
        // Given
        var command = CreateValidCommand(purchasePrice: purchasePrice);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PurchasePrice");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_VehicleGroupID_Is_Invalid_Format(int invalidGroupId)
    {
        // Given
        var command = CreateValidCommand(vehicleGroupID: invalidGroupId);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VehicleGroupID");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_LicenseExpiration_Is_Before_PurchaseDate()
    {
        var purchaseDate = new DateTime(2023, 6, 1, 0, 0, 0, DateTimeKind.Utc);
        var expirationDate = new DateTime(2023, 5, 31, 0, 0, 0, DateTimeKind.Utc);

        var command = CreateValidCommand(
            purchaseDate: purchaseDate,
            licenseExpiration: expirationDate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LicensePlateExpirationDate");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_VehicleType_Is_Default()
    {
        // Given - This would be an invalid enum value
        var command = CreateValidCommand(vehicleType: default);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VehicleType");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_FuelType_Is_Default()
    {
        // Given
        var command = CreateValidCommand(fuelType: default);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FuelType");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Status_Is_Default()
    {
        // Given
        var command = CreateValidCommand(status: default);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Status");
    }
}