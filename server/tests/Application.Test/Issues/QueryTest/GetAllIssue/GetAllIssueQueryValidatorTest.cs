using Application.Features.Issues.Query.GetAllIssue;
using Application.Models.PaginationModels;

namespace Application.Test.Issues.QueryTest.GetAllIssue;

public class GetAllIssueQueryValidatorTest
{
    private readonly GetAllIssueQueryValidator _validator;

    public GetAllIssueQueryValidatorTest()
    {
        _validator = new GetAllIssueQueryValidator();
    }

    private GetAllIssueQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "title",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllIssueQuery(
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
        var query = new GetAllIssueQuery(Parameters: null!);
        var result = await _validator.ValidateAsync(query);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    [Theory]
    [InlineData("title")]
    [InlineData("status")]
    [InlineData("prioritylevel")]
    [InlineData("category")]
    [InlineData("reporteddate")]
    [InlineData("resolveddate")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_Issue_Field(string validSortField)
    {
        var query = CreateValidQuery(sortBy: validSortField);
        var result = await _validator.ValidateAsync(query);
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData("name")]
    [InlineData("make")]
    [InlineData("model")]
    [InlineData("purchaseprice")]
    [InlineData("mileage")]
    [InlineData("enginehours")]
    [InlineData("purchasedate")]
    [InlineData("fuelcapacity")]
    [InlineData("location")]
    [InlineData("invalidfield")]
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_Issue_Field(string invalidSortField)
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