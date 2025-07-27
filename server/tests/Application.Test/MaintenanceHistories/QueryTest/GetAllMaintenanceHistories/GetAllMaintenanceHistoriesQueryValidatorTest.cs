using Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;
using Application.Models.PaginationModels;

namespace Application.Test.MaintenanceHistories.QueryTest.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoriesQueryValidatorTest
{
    private readonly GetAllMaintenanceHistoriesQueryValidator _validator;

    public GetAllMaintenanceHistoriesQueryValidatorTest()
    {
        _validator = new GetAllMaintenanceHistoriesQueryValidator();
    }

    private GetAllMaintenanceHistoriesQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "servicedate",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllMaintenanceHistoriesQuery(
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
        var query = new GetAllMaintenanceHistoriesQuery(Parameters: null!);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    [Theory]
    [InlineData("servicedate")]
    [InlineData("vehicleid")]
    [InlineData("workorderid")]
    [InlineData("servicetaskid")]
    [InlineData("technicianid")]
    [InlineData("mileageatservice")]
    [InlineData("cost")]
    [InlineData("labourhours")]
    [InlineData("createdat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_Field(string validSortField)
    {
        var query = CreateValidQuery(sortBy: validSortField);
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData("name")]
    [InlineData("model")]
    [InlineData("purchaseprice")]
    [InlineData("id")]
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