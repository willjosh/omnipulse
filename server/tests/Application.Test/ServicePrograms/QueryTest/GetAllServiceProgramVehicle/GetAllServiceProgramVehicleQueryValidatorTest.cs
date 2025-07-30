using Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;
using Application.Models.PaginationModels;

namespace Application.Test.ServicePrograms.QueryTest.GetAllServiceProgramVehicle;

public class GetAllServiceProgramVehicleQueryValidatorTest
{
    private readonly GetAllServiceProgramVehicleQueryValidator _queryValidator = new();

    [Fact]
    public void Validator_Should_Pass_With_Valid_ServiceProgramID_And_Parameters()
    {
        // Arrange
        var query = new GetAllServiceProgramVehicleQuery(
            ServiceProgramID: 1,
            Parameters: new PaginationParameters
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = "vehiclename",
                SortDescending = false
            }
        );

        // Act
        var result = _queryValidator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public void Validator_Should_Fail_With_Invalid_ServiceProgramID(int invalidId)
    {
        // Arrange
        var query = new GetAllServiceProgramVehicleQuery(
            ServiceProgramID: invalidId,
            Parameters: new PaginationParameters()
        );

        // Act
        var result = _queryValidator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetAllServiceProgramVehicleQuery.ServiceProgramID));
    }

    [Fact]
    public void Validator_Should_Fail_With_Null_Parameters()
    {
        // Arrange
        var query = new GetAllServiceProgramVehicleQuery(
            ServiceProgramID: 1,
            Parameters: null!
        );

        // Act
        var result = _queryValidator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(GetAllServiceProgramVehicleQuery.Parameters));
    }

    [Theory]
    [InlineData("vehiclename")]
    [InlineData("addedat")]
    public void Validator_Should_Pass_With_Valid_SortBy_Fields(string sortBy)
    {
        // Arrange
        var query = new GetAllServiceProgramVehicleQuery(
            ServiceProgramID: 1,
            Parameters: new PaginationParameters
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = sortBy
            }
        );

        // Act
        var result = _queryValidator.Validate(query);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("invalidfield")]
    public void Validator_Should_Fail_With_Invalid_SortBy_Fields(string invalidSortBy)
    {
        // Arrange
        var query = new GetAllServiceProgramVehicleQuery(
            ServiceProgramID: 1,
            Parameters: new PaginationParameters
            {
                PageNumber = 1,
                PageSize = 10,
                SortBy = invalidSortBy
            }
        );

        // Act
        var result = _queryValidator.Validate(query);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName.Contains(nameof(PaginationParameters.SortBy)));
    }
}