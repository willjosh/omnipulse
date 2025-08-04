using Application.Features.InspectionForms.Query.GetAllInspectionForm;
using Application.Models.PaginationModels;

namespace Application.Test.InspectionForms.QueryTest.GetAllInspectionForm;

public class GetAllInspectionFormQueryValidatorTest
{
    private readonly GetAllInspectionFormQueryValidator _validator = new();

    private static GetAllInspectionFormQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            SortDescending = sortDescending
        };
        return new GetAllInspectionFormQuery(parameters);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Parameters_Are_Valid()
    {
        // Arrange
        var query = CreateValidQuery();

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parameters_Are_Null()
    {
        // Arrange
        var query = new GetAllInspectionFormQuery(null!);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetAllInspectionFormQuery.Parameters));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10)]
    public async Task Validator_Should_Fail_When_PageNumber_Is_Invalid(int invalidPageNumber)
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: invalidPageNumber);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains(nameof(PaginationParameters.PageNumber)));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_PageSize_Is_Invalid(int invalidPageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageSize: invalidPageSize);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains(nameof(PaginationParameters.PageSize)));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(25)]
    [InlineData(100)]
    [InlineData(250)] // Max allowed page size
    public async Task Validator_Should_Pass_When_PageSize_Is_Valid(int validPageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageSize: validPageSize);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("title")]
    [InlineData("description")]
    [InlineData("isactive")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid(string validSortBy)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: validSortBy);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("invalidfield")]
    [InlineData("name")] // ServiceProgram field, not InspectionForm
    [InlineData("id")]
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid(string invalidSortBy)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: invalidSortBy);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains(nameof(PaginationParameters.SortBy)));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_SortBy_Is_Null()
    {
        // Arrange
        var query = CreateValidQuery(sortBy: null);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_SortBy_Is_Empty_String()
    {
        // Arrange
        var query = CreateValidQuery(sortBy: "");

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validator_Should_Pass_When_SortDescending_Is_Valid(bool sortDescending)
    {
        // Arrange
        var query = CreateValidQuery(sortDescending: sortDescending);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("safety")]
    [InlineData("vehicle inspection")]
    [InlineData("a")]
    [InlineData(null)]
    public async Task Validator_Should_Pass_When_Search_Is_Valid(string? search)
    {
        // Arrange
        var query = CreateValidQuery(search: search);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_All_Valid_Parameters()
    {
        // Arrange
        var query = CreateValidQuery(
            pageNumber: 2,
            pageSize: 25,
            search: "safety inspection",
            sortBy: "title",
            sortDescending: true);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Return_Multiple_Errors_When_Multiple_Properties_Invalid()
    {
        // Arrange
        var query = CreateValidQuery(
            pageNumber: -1, // Invalid
            pageSize: 0, // Invalid
            sortBy: "invalidfield"); // Invalid

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 3); // At least 3 errors
        Assert.Contains(result.Errors, e => e.PropertyName.Contains(nameof(PaginationParameters.PageNumber)));
        Assert.Contains(result.Errors, e => e.PropertyName.Contains(nameof(PaginationParameters.PageSize)));
        Assert.Contains(result.Errors, e => e.PropertyName.Contains(nameof(PaginationParameters.SortBy)));
    }

    [Theory]
    [InlineData(1, 10)]
    [InlineData(5, 50)]
    [InlineData(10, 100)]
    public async Task Validator_Should_Pass_With_Different_Valid_Pagination_Combinations(int pageNumber, int pageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: pageNumber, pageSize: pageSize);

        // Act
        var result = await _validator.ValidateAsync(query);

        // Assert
        Assert.True(result.IsValid);
    }
}