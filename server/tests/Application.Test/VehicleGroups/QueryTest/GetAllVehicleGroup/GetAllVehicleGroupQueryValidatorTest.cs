using System;
using Application.Features.VehicleGroups.Query.GetAllVehicleGroup;
using Application.Features.Shared;
using Application.Models;
using Xunit;

namespace Application.Test.VehicleGroups.QueryTest.GetAllVehicleGroup;

public class GetAllVehicleGroupQueryValidatorTest
{
    private readonly GetAllVehicleGroupQueryValidator _validator;

    public GetAllVehicleGroupQueryValidatorTest()
    {
        _validator = new GetAllVehicleGroupQueryValidator();
    }

    private GetAllVehicleGroupQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "name",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllVehicleGroupQuery(
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
        var query = new GetAllVehicleGroupQuery(Parameters: null!);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }


    // VEHICLE GROUP SORT FIELD VALIDATION TESTS
    [Theory]
    [InlineData("name")]
    [InlineData("description")]
    [InlineData("isactive")]
    [InlineData("createdat")]
    [InlineData("updatedat")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_VehicleGroup_Field(string validSortField)
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
    [InlineData("category")]
    [InlineData("price")]
    [InlineData("id")]
    [InlineData("invalidfield")]
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_VehicleGroup_Field(string invalidSortField)
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
