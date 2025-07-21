using Application.Features.Users.Query.GetAllTechnician;
using Application.Models.PaginationModels;

namespace Application.Test.Users.QueryTest.GetAllTechnician;

public class GetAllTechnicianQueryValidatorTest
{
    private readonly GetAllTechnicianQueryValidator _validator;

    public GetAllTechnicianQueryValidatorTest()
    {
        _validator = new GetAllTechnicianQueryValidator();
    }

    private GetAllTechnicianQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "firstname",
        bool sortDescending = false,
        string? search = null
    )
    {
        return new GetAllTechnicianQuery(new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = sortBy,
            SortDescending = sortDescending,
            Search = search
        });
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
        var query = new GetAllTechnicianQuery(Parameters: null!);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    [Theory]
    [InlineData("firstname")]
    [InlineData("lastname")]
    [InlineData("email")]
    [InlineData("hiredate")]
    public async Task Validator_Should_Pass_With_Valid_SortBy(string sortBy)
    {
        // Given
        var query = CreateValidQuery(sortBy: sortBy);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("invalidsort")]
    [InlineData("123")]
    public async Task Validator_Should_Fail_With_Invalid_SortBy(string sortBy)
    {
        // Given
        var query = CreateValidQuery(sortBy: sortBy);

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