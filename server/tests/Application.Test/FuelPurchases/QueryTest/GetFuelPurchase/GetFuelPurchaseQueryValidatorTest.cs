using Application.Features.FuelPurchases.Query.GetFuelPurchase;

namespace Application.Test.FuelPurchases.QueryTest.GetFuelPurchase;

public class GetFuelPurchaseQueryValidatorTest
{
    private readonly GetFuelPurchaseQueryValidator _validator = new();

    private static GetFuelPurchaseQuery CreateQuery(int id) => new(id);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_Id_Is_Not_Positive(int invalidId)
    {
        // ===== Arrange =====
        var query = CreateQuery(invalidId);

        // ===== Act =====
        var result = await _validator.ValidateAsync(query);

        // ===== Assert =====
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetFuelPurchaseQuery.FuelPurchaseID));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Id_Is_Positive()
    {
        // ===== Arrange =====
        var query = CreateQuery(123);

        // ===== Act =====
        var result = await _validator.ValidateAsync(query);

        // ===== Assert =====
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}