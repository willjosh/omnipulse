using System;
using Application.Features.Vehicles.Query.GetAllVehicle;
using Application.Features.Shared;
using Xunit;
using Application.Models;

namespace Application.Test.Vehicles.QueryTest.GetAllVehicle;

public class GetAllVehicleQueryValidatorTest
{
    private readonly GetAllVehicleQueryValidator _validator;

    public GetAllVehicleQueryValidatorTest()
    {
        _validator = new GetAllVehicleQueryValidator();
    }

    private GetAllVehicleQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "name",
        bool sortDescending = false,
        string? search = null)
    {
        return new GetAllVehicleQuery(
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
        var query = new GetAllVehicleQuery(Parameters: null!);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    // VEHICLE-SPECIFIC SORT FIELD VALIDATION TESTS
    [Theory]
    [InlineData("name")]
    [InlineData("make")]
    [InlineData("model")]
    [InlineData("year")]
    [InlineData("status")]
    [InlineData("purchaseprice")]
    [InlineData("mileage")]
    [InlineData("enginehours")]
    [InlineData("createdat")]
    [InlineData("purchasedate")]
    [InlineData("fuelcapacity")]
    [InlineData("location")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_Vehicle_Field(string validSortField)
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
    [InlineData("email")]           // Valid for users, not vehicles
    [InlineData("username")]        // Valid for users, not vehicles
    [InlineData("description")]     // Not a vehicle field
    [InlineData("category")]        // Not a vehicle field
    [InlineData("price")]           // Should be "purchaseprice"
    [InlineData("id")]              // Not in vehicle sort fields
    [InlineData("invalidfield")]    // Completely invalid
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_Vehicle_Field(string invalidSortField)
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