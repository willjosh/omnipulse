using Application.Features.FuelPurchases.Query.GetAllFuelPurchase;
using Application.Models.PaginationModels;

namespace Application.Test.FuelPurchases.QueryTest;

public class GetAllFuelPurchaseQueryValidatorTest
{
    private readonly GetAllFuelPurchaseQueryValidator _validator;

    public GetAllFuelPurchaseQueryValidatorTest()
    {
        _validator = new GetAllFuelPurchaseQueryValidator();
    }

    private GetAllFuelPurchaseQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "receiptnumber",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllFuelPurchaseQuery(
            Parameters: new PaginationParameters
            {
                PageNumber = pageNumber,
                PageSize = pageSize,
                SortBy = sortBy,
                SortDescending = sortDescending,
                Search = search
            }
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Query()
    {
        // Given
        var query = CreateValidQuery();

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        //Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parameters_Is_Null()
    {
        // Given
        var query = new GetAllFuelPurchaseQuery(Parameters: null!);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    // FuelPurchase-SPECIFIC SORT FIELD VALIDATION TESTS
    [Theory]
    [InlineData("vehicleid")]
    [InlineData("purchasedbyuserid")]
    [InlineData("purchasedate")]
    [InlineData("odometerreading")]
    [InlineData("volume")]
    [InlineData("priceperunit")]
    [InlineData("totalcost")]
    [InlineData("fuelstation")]
    [InlineData("receiptnumber")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_FuelPurchases_Field(string validSortField)
    {
        // Given
        var query = CreateValidQuery(sortBy: validSortField);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData("name")]
    [InlineData("username")]
    [InlineData("description")]
    [InlineData("vehiclegroup")]
    [InlineData("price")]
    [InlineData("id")]
    [InlineData("invalidfield")]
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_FuelPurchases_Field(string invalidSortField)
    {
        // Given
        var query = CreateValidQuery(sortBy: invalidSortField);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }


    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Null_Or_Empty(string? sortBy)
    {
        // Given
        var query = CreateValidQuery(sortBy: sortBy);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }
}