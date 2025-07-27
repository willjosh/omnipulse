using Application.Features.ServiceTasks.Query.GetAllServiceTask;
using Application.Models.PaginationModels;

using FluentValidation.TestHelper;

namespace Application.Test.ServiceTasks.QueryTest.GetAllServiceTask;

public class GetAllServiceTaskQueryValidatorTest
{
    private readonly GetAllServiceTaskQueryValidator _validator;

    public GetAllServiceTaskQueryValidatorTest()
    {
        _validator = new GetAllServiceTaskQueryValidator();
    }

    [Fact]
    public void Validator_Should_Pass_For_Valid_Parameters()
    {
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = "name",
            SortDescending = false,
            Search = ""
        };
        var query = new GetAllServiceTaskQuery(parameters);
        var result = _validator.TestValidate(query);
        result.ShouldNotHaveAnyValidationErrors();
    }

    [Theory]
    [InlineData(0, 10)] // Invalid PageNumber
    [InlineData(1, 0)]  // Invalid PageSize
    public void Validator_Should_Fail_For_Invalid_Pagination(int pageNumber, int pageSize)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            SortBy = "name"
        };
        var query = new GetAllServiceTaskQuery(parameters);
        var result = _validator.TestValidate(query);
        if (pageNumber <= 0)
        {
            result.ShouldHaveValidationErrorFor(x => x.Parameters.PageNumber);
        }
        else if (pageSize <= 0)
        {
            result.ShouldHaveValidationErrorFor(x => x.Parameters.PageSize);
        }
    }

    [Theory]
    [InlineData("invalidfield")]
    [InlineData("notarealcolumn")]
    public void Validator_Should_Fail_For_Invalid_SortBy(string sortBy)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = sortBy
        };
        var query = new GetAllServiceTaskQuery(parameters);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Parameters.SortBy);
    }

    [Fact]
    public void Validator_Should_Fail_When_Parameters_Null()
    {
        var query = new GetAllServiceTaskQuery(null!);
        var result = _validator.TestValidate(query);
        result.ShouldHaveValidationErrorFor(x => x.Parameters);
    }
}