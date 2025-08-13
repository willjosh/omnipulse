using Application.Features.FuelPurchases.Command.DeleteFuelPurchase;

namespace Application.Test.FuelPurchases.CommandTest.DeleteFuelPurchase;

public class DeleteFuelPurchaseCommandValidatorTest
{
    private readonly DeleteFuelPurchaseCommandValidator _validator = new();

    private static DeleteFuelPurchaseCommand CreateCommand(int fuelPurchaseID) => new(fuelPurchaseID);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_FuelPurchaseID_Is_Invalid(int invalidId)
    {
        // ===== Arrange =====
        var command = CreateCommand(invalidId);

        // ===== Act =====
        var result = await _validator.ValidateAsync(command);

        // ===== Assert =====
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteFuelPurchaseCommand.FuelPurchaseID));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_FuelPurchaseID_Is_Positive()
    {
        // ===== Arrange =====
        var command = CreateCommand(123);

        // ===== Act =====
        var result = await _validator.ValidateAsync(command);

        // ===== Assert =====
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}