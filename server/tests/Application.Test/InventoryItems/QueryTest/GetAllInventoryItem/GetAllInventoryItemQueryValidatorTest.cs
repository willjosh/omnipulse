using System;

using Application.Features.InventoryItems.Query.GetAllInventoryItem;
using Application.Models;
using Application.Models.PaginationModels;

namespace Application.Test.InventoryItems.QueryTest.GetAllInventoryItem;

public class GetAllInventoryItemQueryValidatorTest
{
    private readonly GetAllInventoryItemQueryValidator _validator;

    public GetAllInventoryItemQueryValidatorTest()
    {
        _validator = new GetAllInventoryItemQueryValidator();
    }

    private GetAllInventoryItemQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "itemnumber",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllInventoryItemQuery(
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
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parameters_Is_Null()
    {
        // Given
        var query = new GetAllInventoryItemQuery(null!);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    // INVENTORY ITEM SORT FIELD VALIDATION TESTS
    [Theory]
    [InlineData("itemnumber")]
    [InlineData("itemname")]
    [InlineData("description")]
    [InlineData("category")]
    [InlineData("manufacturer")]
    [InlineData("manufacturerpartnumber")]
    [InlineData("universalproductcode")]
    [InlineData("supplier")]
    [InlineData("weightkg")]
    [InlineData("unitcost")]
    [InlineData("unitcostmeasurementunit")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_Inventory_Item_Field(string validSortField)
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
    [InlineData("email")]
    [InlineData("username")]
    [InlineData("invalidfield")]
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_Inventory_Item_Field(string invalidSortField)
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