using System;
using Application.Features.Shared;
using Application.Features.WorkOrders.Query.GetAllWorkOrder;
using Application.Models;
using Xunit;

namespace Application.Test.WorkOrders.QueryTest.GetAllWorkOrder;

public class GetAllWorkOrderQueryValidatorTest
{
    private readonly GetAllWorkOrderQueryValidator _validator;

    public GetAllWorkOrderQueryValidatorTest()
    {
        _validator = new GetAllWorkOrderQueryValidator();
    }

    private GetAllWorkOrderQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? sortBy = "id",
        bool sortDescending = false,
        string? search = null
        )
    {
        return new GetAllWorkOrderQuery(
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
        var query = new GetAllWorkOrderQuery(Parameters: null!);

        // When
        var result = await _validator.ValidateAsync(query);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Parameters");
    }

    // WorkOrder-SPECIFIC SORT FIELD VALIDATION TESTS
    [Theory]
    [InlineData("id")]
    [InlineData("status")]
    [InlineData("workOrderType")]
    [InlineData("priority")]
    [InlineData("startOdometer")]
    [InlineData("scheduledStartDate")]
    [InlineData("actualStartDate")]
    public async Task Validator_Should_Pass_When_SortBy_Is_Valid_WorkOrder_Field(string validSortField)
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
    [InlineData("email")]           // Valid for users, not workOrder
    [InlineData("username")]        // Valid for users, not workOrder
    [InlineData("description")]     // Not a vehicle field
    [InlineData("category")]        // Not a vehicle field
    [InlineData("price")]           // Should be "purchaseprice"
    [InlineData("invalidfield")]    // Completely invalid
    public async Task Validator_Should_Fail_When_SortBy_Is_Invalid_WorkOrderField(string invalidSortField)
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