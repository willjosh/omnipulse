using Application.Features.ServicePrograms.Query.GetAllServiceProgram;
using Application.Models.PaginationModels;

namespace Application.Test.ServicePrograms.QueryTest.GetAllServiceProgram;

public class GetAllServiceProgramQueryValidatorTest
{
    private readonly GetAllServiceProgramQueryValidator _queryValidator = new();

    private static GetAllServiceProgramQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "name",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllServiceProgramQuery(
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
        var query = new GetAllServiceProgramQuery(Parameters: null!);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    [Theory]
    [InlineData("name")]
    [InlineData("description")]
    [InlineData("isactive")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_ServiceProgram_Field(string validSortField)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: validSortField);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortBy");
    }

    [Theory]
    [InlineData("email")]           // Valid for users, not service programs
    [InlineData("username")]        // Valid for users, not service programs
    [InlineData("make")]            // Valid for vehicles, not service programs
    [InlineData("model")]           // Valid for vehicles, not service programs
    [InlineData("year")]            // Valid for vehicles, not service programs
    [InlineData("status")]          // Valid for vehicles, not service programs
    [InlineData("purchaseprice")]   // Valid for vehicles, not service programs
    [InlineData("mileage")]         // Valid for vehicles, not service programs
    [InlineData("purchasedate")]    // Valid for vehicles, not service programs
    [InlineData("fuelcapacity")]    // Valid for vehicles, not service programs
    [InlineData("location")]        // Valid for vehicles, not service programs
    [InlineData("category")]        // Not a service program field
    [InlineData("price")]           // Not a service program field
    [InlineData("id")]              // Not in service program sort fields
    [InlineData("invalidfield")]    // Completely invalid
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_ServiceProgram_Field(string invalidSortField)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: invalidSortField);

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

    [Theory]
    [InlineData(1, 10)]
    [InlineData(1, 25)]
    [InlineData(2, 50)]
    [InlineData(5, 100)]
    public async Task Validator_Should_Pass_With_Valid_Pagination_Parameters(int pageNumber, int pageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: pageNumber, pageSize: pageSize);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName.Contains("PageNumber") || e.PropertyName.Contains("PageSize"));
    }

    [Theory]
    [InlineData(0, 10)]      // Invalid page number
    [InlineData(-1, 10)]     // Invalid page number
    [InlineData(1, 0)]       // Invalid page size
    [InlineData(1, -1)]      // Invalid page size
    public async Task Validator_Should_Fail_With_Invalid_Pagination_Parameters(int pageNumber, int pageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: pageNumber, pageSize: pageSize);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains("PageNumber") || e.PropertyName.Contains("PageSize"));
    }

    [Theory]
    [InlineData("test")]
    [InlineData("maintenance")]
    [InlineData("fleet")]
    [InlineData("program")]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validator_Should_Pass_With_Valid_Search_Terms(string? search)
    {
        // Arrange
        var query = CreateValidQuery(search: search);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.Search");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validator_Should_Pass_With_Valid_SortDescending_Values(bool sortDescending)
    {
        // Arrange
        var query = CreateValidQuery(sortDescending: sortDescending);

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Parameters.SortDescending");
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Complex_Valid_Query()
    {
        // Arrange
        var query = CreateValidQuery(
            pageNumber: 3,
            pageSize: 25,
            sortBy: "createdat",
            sortDescending: true,
            search: "maintenance program"
        );

        // Act
        var result = await _queryValidator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}