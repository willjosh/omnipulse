using Application.Features.Inventory.Query;
using Application.Models.PaginationModels;

namespace Application.Test.Inventories.QueryTest.GetAllInventoryTest;

public class GetAllInventoryQueryValidatorTest
{
    private readonly GetAllInventoryQueryValidator _validator;

    public GetAllInventoryQueryValidatorTest()
    {
        _validator = new GetAllInventoryQueryValidator();
    }

    private GetAllInventoryQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "location",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllInventoryQuery(
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
        var query = CreateValidQuery();
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parameters_Is_Null()
    {
        var query = new GetAllInventoryQuery(Parameters: null!);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    // INVENTORY-SPECIFIC SORT FIELD VALIDATION TESTS
    [Theory]
    [InlineData("id")]
    [InlineData("quantity")]
    [InlineData("minstocklevel")]
    [InlineData("maxstocklevel")]
    [InlineData("location")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_Inventory_Field(string validSortField)
    {
        var query = CreateValidQuery(sortBy: validSortField);
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData("email")]           // Valid for users, not inventory
    [InlineData("username")]        // Valid for users, not inventory
    [InlineData("model")]           // Valid for vehicles, not inventory
    [InlineData("price")]           // Should be "unitcost" or "totalcost"
    [InlineData("invalidfield")]    // Completely invalid
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_Inventory_Field(string invalidSortField)
    {
        var query = CreateValidQuery(sortBy: invalidSortField);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Null_Or_Empty(string? sortBy)
    {
        var query = CreateValidQuery(sortBy: sortBy);
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }
}