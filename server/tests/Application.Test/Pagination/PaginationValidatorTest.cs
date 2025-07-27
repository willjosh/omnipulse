using Application.Features.Shared;
using Application.Models.PaginationModels;

namespace Application.Test.Pagination;

public class PaginationValidatorTest
{
    private readonly PaginationValidator<object> _validator;
    private readonly string[] _validSortFields = { "name", "email", "createdat", "status" };

    public PaginationValidatorTest()
    {
        _validator = new PaginationValidator<object>(_validSortFields);
    }

    [Fact]
    public async Task Should_Have_Error_When_PageNumber_Is_Zero()
    {
        // Given
        var parameters = new PaginationParameters { PageNumber = 0 };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public async Task Should_Have_Error_When_PageNumber_Is_Negative()
    {
        // Given
        var parameters = new PaginationParameters { PageNumber = -1 };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber" &&
                                           e.ErrorMessage.Contains("greater than 0"));
    }


    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(100)]
    [InlineData(1000)]
    [InlineData(10000)]
    public async Task Should_Not_Have_Error_When_PageNumber_Is_Valid(int pageNumber)
    {
        // Given
        var parameters = new PaginationParameters { PageNumber = pageNumber };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "PageNumber");
    }

    [Fact]
    public async Task Should_Have_Error_When_PageSize_Is_Zero()
    {
        // Given
        var parameters = new PaginationParameters { PageSize = 0 };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public async Task Should_Have_Error_When_PageSize_Is_Negative()
    {
        // Given
        var parameters = new PaginationParameters { PageSize = -5 };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize" &&
                                           e.ErrorMessage.Contains("greater than 0"));
    }


    [Theory]
    [InlineData(1)]
    [InlineData(10)]
    [InlineData(25)]
    [InlineData(50)]
    [InlineData(100)]
    public async Task Should_Not_Have_Error_When_PageSize_Is_Valid(int pageSize)
    {
        // Given
        var parameters = new PaginationParameters { PageSize = pageSize };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "PageSize");
    }

    [Fact]
    public async Task Should_Have_Error_When_Search_Exceeds_Maximum_Length()
    {
        // Given
        var longSearchTerm = new string('a', 101); // 101 characters
        var parameters = new PaginationParameters { Search = longSearchTerm };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Search" &&
                                           e.ErrorMessage.Contains("100 characters"));
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("  ")]
    [InlineData("toyota")]
    [InlineData("Toyota Corolla")]
    public async Task Should_Not_Have_Error_When_Search_Is_Valid(string? searchTerm)
    {
        // Given
        var parameters = new PaginationParameters { Search = searchTerm };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Search");
    }

    [Fact]
    public async Task Should_Not_Have_Error_When_Search_Is_Exactly_100_Characters()
    {
        // Given
        var searchTerm = new string('a', 100); // Exactly 100 characters
        var parameters = new PaginationParameters { Search = searchTerm };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Search");
    }

    [Theory]
    [InlineData("name")]
    [InlineData("email")]
    [InlineData("createdat")]
    [InlineData("status")]
    public async Task Should_Not_Have_Error_When_SortBy_Is_Valid(string sortBy)
    {
        // Given
        var parameters = new PaginationParameters { SortBy = sortBy };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "SortBy");
    }

    [Theory]
    [InlineData("NAME")]     // Test case insensitive
    [InlineData("Email")]    // Test mixed case
    [InlineData("CREATEDAT")] // Test uppercase
    public async Task Should_Not_Have_Error_When_SortBy_Is_Valid_Different_Case(string sortBy)
    {
        // Given
        var parameters = new PaginationParameters { SortBy = sortBy };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "SortBy");
    }

    [Theory]
    [InlineData("invalid_field")]
    [InlineData("nonexistent")]
    [InlineData("random")]
    [InlineData("id")] // Not in our valid fields list
    public async Task Should_Have_Error_When_SortBy_Is_Invalid(string sortBy)
    {
        // Given
        var parameters = new PaginationParameters { SortBy = sortBy };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SortBy");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Should_Not_Have_Error_When_SortBy_Is_Null_Or_Empty(string? sortBy)
    {
        // Given
        var parameters = new PaginationParameters { SortBy = sortBy };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "SortBy");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_Not_Have_Error_When_SortDescending_Is_Valid(bool sortDescending)
    {
        // Given
        var parameters = new PaginationParameters { SortDescending = sortDescending };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "SortDescending");
    }

    [Fact]
    public async Task Should_Pass_Validation_With_All_Valid_Parameters()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "toyota",
            SortBy = "name",
            SortDescending = false
        };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Should_Pass_Validation_With_Minimal_Valid_Parameters()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
            // Search, SortBy are null - should be valid
        };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Should_Have_Multiple_Errors_When_Multiple_Fields_Invalid()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 0,           // Invalid
            PageSize = -5,            // Invalid
            Search = new string('a', 101), // Invalid - too long
            SortBy = "invalid_field"  // Invalid
        };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PageNumber");
        Assert.Contains(result.Errors, e => e.PropertyName == "PageSize");
        Assert.Contains(result.Errors, e => e.PropertyName == "Search");
        Assert.Contains(result.Errors, e => e.PropertyName == "SortBy");
    }

    [Fact]
    public async Task Should_Pass_Validation_With_Maximum_Valid_Values()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 10000,       // Maximum allowed
            PageSize = 100,           // Maximum allowed
            Search = new string('a', 100), // Maximum allowed length
            SortBy = "name",
            SortDescending = true
        };

        // When
        var result = await _validator.ValidateAsync(parameters);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Should_Work_With_Different_Sort_Fields()
    {
        // Given
        var vehicleSortFields = new[] { "make", "model", "year", "vin" };
        var vehicleValidator = new PaginationValidator<object>(vehicleSortFields);
        var parameters = new PaginationParameters { SortBy = "make" };

        // When
        var result = await vehicleValidator.ValidateAsync(parameters);

        // Then
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "SortBy");
    }

    [Fact]
    public async Task Should_Reject_Invalid_Sort_Field_For_Different_Entity()
    {
        // Given
        var vehicleSortFields = new[] { "make", "model", "year" };
        var vehicleValidator = new PaginationValidator<object>(vehicleSortFields);
        var parameters = new PaginationParameters { SortBy = "email" }; // Valid for user, not vehicle

        // When
        var result = await vehicleValidator.ValidateAsync(parameters);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "SortBy");
    }
}