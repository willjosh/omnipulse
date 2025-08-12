using Application.Features.FuelPurchases.Command.CreateFuelPurchase;

namespace Application.Test.FuelPurchases.CommandTest.CreateFuelPurchase;

public class CreateFuelPurchaseCommandValidatorTest
{
    private readonly CreateFuelPurchaseCommandValidator _validator;

    public CreateFuelPurchaseCommandValidatorTest()
    {
        _validator = new CreateFuelPurchaseCommandValidator();
    }

    private CreateFuelPurchaseCommand CreateValidCommand(
        int vehicleId = 1,
        string purchasedByUserId = "1",
        DateTime purchaseDate = default,
        double odometerReading = 150000.75,
        double volume = 90.7,
        decimal pricePerUnit = 5.19m,
        decimal totalCost = 470.733m,
        string fuelStation = "Test Fuel Station",
        string receiptNumber = "Test Receipt Number",
        string? notes = ""
    )
    {
        return new CreateFuelPurchaseCommand(vehicleId, purchasedByUserId, purchaseDate, odometerReading, volume, pricePerUnit, totalCost, fuelStation, receiptNumber, notes);
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
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_VehicleId_Is_Empty(int invalidVehicleId)
    {
        // Given
        var command = CreateValidCommand(vehicleId: invalidVehicleId);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "VehicleId");
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public async Task Validator_Should_Fail_When_PurchasedByUserId_Is_Empty(string invalidPurchasedByUserId)
    {
        // Given
        var command = CreateValidCommand(purchasedByUserId: invalidPurchasedByUserId);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PurchasedByUserId");
    }

    [Theory]
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_OdometerReading_Is_Empty(double invalidOdometerReading)
    {
        // Given
        var command = CreateValidCommand(odometerReading: invalidOdometerReading);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "OdometerReading");
    }

    [Theory]
    [InlineData(-12.2)]
    [InlineData(-680.2)]
    public async Task Validator_Should_Fail_When_OdometerReading_Is_Less_Than_Zero(double invalidOdometerReading)
    {
        // Given
        var command = CreateValidCommand(odometerReading: invalidOdometerReading);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "OdometerReading");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_PurchasedDate_Is_Empty()
    {
        // Given
        var command = new CreateFuelPurchaseCommand(1, "1", DateTime.MinValue,
        150000.75,
        90.7,
        5.19m,
        470.733m,
        "Test Fuel Station",
        "Test Receipt Number", "");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PurchaseDate");
    }

    [Theory]
    [InlineData(0.0)]
    public async Task Validator_Should_Fail_When_Volume_Is_Empty(double invalidVolume)
    {
        // Given
        var command = CreateValidCommand(volume: invalidVolume);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Volume");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-12301.32)]
    public async Task Validator_Should_Fail_When_Volume_Is_Negative(double invalidVolume)
    {
        // Given
        var command = CreateValidCommand(volume: invalidVolume);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Volume");
    }

    [Theory]
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_PricePerUnit_Is_Empty(decimal invalidPricePerUnit)
    {
        // Given
        var command = CreateValidCommand(pricePerUnit: invalidPricePerUnit);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PricePerUnit");
    }

    [Theory]
    [InlineData(-1.20)]
    [InlineData(-12.39)]
    public async Task Validator_Should_Fail_When_PricePerUnit_Is_Negative(decimal invalidPricePerUnit)
    {
        // Given
        var command = CreateValidCommand(pricePerUnit: invalidPricePerUnit);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PricePerUnit");
    }

    [Theory]
    [InlineData(0)]
    public async Task Validator_Should_Fail_When_TotalCost_Is_Empty(decimal invalidTotalCost)
    {
        // Given
        var command = CreateValidCommand(totalCost: invalidTotalCost);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TotalCost");
    }

    [Theory]
    [InlineData(-1.2)]
    [InlineData(-5.3)]
    public async Task Validator_Should_Fail_When_TotalCost_Is_Negative(decimal invalidTotalCost)
    {
        // Given
        var command = CreateValidCommand(totalCost: invalidTotalCost);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TotalCost");
    }

    [Theory]
    [InlineData(5)]
    [InlineData(2)]
    public async Task Validator_Should_Fail_When_TotalCost_Is_Less_Than_Price_Per_Unit(decimal invalidTotalCost)
    {
        // Given
        var command = CreateValidCommand(totalCost: invalidTotalCost);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "TotalCost");
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public async Task Validator_Should_Fail_When_FuelStation_Is_Empty(string invalidFuelStation)
    {
        // Given
        var command = CreateValidCommand(fuelStation: invalidFuelStation);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "FuelStation");
    }

    [Theory]
    [InlineData("")]
    [InlineData("      ")]
    public async Task Validator_Should_Fail_When_ReceiptNumber_Is_Empty(string invalidReceiptNumber)
    {
        // Given
        var command = CreateValidCommand(receiptNumber: invalidReceiptNumber);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ReceiptNumber");
    }
}