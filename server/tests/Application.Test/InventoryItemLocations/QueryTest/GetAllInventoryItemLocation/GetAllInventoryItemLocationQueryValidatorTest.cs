using System;
using System.Threading.Tasks;

using Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;
using Application.Models;

using Xunit;

namespace Application.Test.InventoryItemLocations.QueryTest.GetAllInventoryItemLocation;

public class GetAllInventoryItemLocationQueryValidatorTest
{
    private readonly GetAllInventoryItemLocationQueryValidator _validator;

    public GetAllInventoryItemLocationQueryValidatorTest()
    {
        _validator = new GetAllInventoryItemLocationQueryValidator();
    }

    private GetAllInventoryItemLocationQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "locationname",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllInventoryItemLocationQuery(
            new PaginationParameters
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
        var query = new GetAllInventoryItemLocationQuery(Parameters: null!);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    [Theory]
    [InlineData("locationname")]
    [InlineData("address")]
    [InlineData("longitude")]
    [InlineData("latitude")]
    [InlineData("capacity")]
    [InlineData("isactive")]
    [InlineData("createdat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_Field(string validSortField)
    {
        var query = CreateValidQuery(sortBy: validSortField);
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData("title")]
    [InlineData("status")]
    [InlineData("prioritylevel")]
    [InlineData("category")]
    [InlineData("reporteddate")]
    [InlineData("resolveddate")]
    [InlineData("updatedat")]
    [InlineData("invalidfield")]
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_Field(string invalidSortField)
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