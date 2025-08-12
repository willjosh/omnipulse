using Application.Features.FuelLogging.Command.UpdateFuelPurchase;

namespace Application.Test.FuelPurchases.CommandTest.UpdateFuelPurchase;

public class UpdateFuelPurchaseCommandValidatorTest
{
    private readonly UpdateFuelPurchaseCommandValidator _validator = new();

    private static UpdateFuelPurchaseCommand CreateValidCommand(
        int fuelPurchaseId = 1,
        int vehicleId = 1,
        string purchasedByUserId = "user-1",
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

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_FuelPurchaseId_Is_Invalid(int invalidId)
    {
        var command = CreateValidCommand(fuelPurchaseId: invalidId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.FuelPurchaseId));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_VehicleId_Is_Invalid(int invalidVehicleId)
    {
        var command = CreateValidCommand(vehicleId: invalidVehicleId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.VehicleId));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_PurchasedByUserId_Is_Empty(string invalidUserId)
    {
        var command = CreateValidCommand(purchasedByUserId: invalidUserId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.PurchasedByUserId));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_PurchaseDate_In_Future()
    {
        var command = CreateValidCommand(purchaseDate: DateTime.UtcNow.AddDays(1));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.PurchaseDate));
    }

    [Theory]
    [InlineData(-0.01)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_OdometerReading_Is_Negative(double invalidReading)
    {
        var command = CreateValidCommand(odometerReading: invalidReading);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.OdometerReading));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_Volume_Is_Not_Positive(double invalidVolume)
    {
        var command = CreateValidCommand(volume: invalidVolume);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.Volume));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_PricePerUnit_Is_Not_Positive(decimal invalidPrice)
    {
        var command = CreateValidCommand(pricePerUnit: invalidPrice);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.PricePerUnit));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validator_Should_Fail_When_FuelStation_Is_Invalid(string invalidStation)
    {
        var command = CreateValidCommand(fuelStation: invalidStation);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.FuelStation));
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    public async Task Validator_Should_Fail_When_ReceiptNumber_Is_Invalid(string invalidReceipt)
    {
        var command = CreateValidCommand(receiptNumber: invalidReceipt);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateFuelPurchaseCommand.ReceiptNumber));
    }
}