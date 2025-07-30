using Application.Features.ServiceSchedules.Query.GetAllServiceSchedule;
using Application.Models.PaginationModels;

namespace Application.Test.ServiceSchedules.QueryTest.GetAllServiceSchedule;

public class GetAllServiceScheduleQueryValidatorTest
{
    private readonly GetAllServiceScheduleQueryValidator _queryValidator = new();

    private GetAllServiceScheduleQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "name",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllServiceScheduleQuery(
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
        // Arrange
        var query = CreateValidQuery();

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parameters_Is_Null()
    {
        // Arrange
        var query = new GetAllServiceScheduleQuery(null!);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    [Theory]
    [InlineData("name")]
    [InlineData("isactive")]
    [InlineData("createdat")]
    public async Task Validator_Should_Pass_With_Valid_SortBy(string sortBy)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: sortBy);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData("invalidfield")]
    [InlineData("model")]
    [InlineData("purchaseprice")]
    public async Task Validator_Should_Fail_With_Invalid_SortBy(string sortBy)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: sortBy);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Null_Or_Empty(string? sortBy)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: sortBy);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }
}