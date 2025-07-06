using Application.Features.ServiceTasks.Command.CreateServiceTask;

using Domain.Entities.Enums;

using Xunit;

namespace Application.Test.ServiceTasks.CommandTest.CreateServiceTask;

public class CreateServiceTaskCommandValidatorTest
{
    private readonly CreateServiceTaskCommandValidator _validator;

    public CreateServiceTaskCommandValidatorTest()
    {
        _validator = new CreateServiceTaskCommandValidator();
    }

    private CreateServiceTaskCommand CreateValidCommand(
        string name = "Engine Oil Change",
        string? description = "Replace engine oil and filter",
        double estimatedLabourHours = 1.5,
        decimal estimatedCost = 85.50m,
        ServiceTaskCategoryEnum category = ServiceTaskCategoryEnum.PREVENTIVE,
        bool isActive = true)
    {
        return new CreateServiceTaskCommand(
            Name: name,
            Description: description,
            EstimatedLabourHours: estimatedLabourHours,
            EstimatedCost: estimatedCost,
            Category: category,
            IsActive: isActive
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Name_Is_Empty(string invalidName)
    {
        var command = CreateValidCommand(name: invalidName);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_EstimatedLabourHours_Is_Non_Positive()
    {
        var command = CreateValidCommand(estimatedLabourHours: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedLabourHours");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_EstimatedCost_Is_Negative()
    {
        var command = CreateValidCommand(estimatedCost: -1);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedCost");
    }

    [Theory]
    [InlineData(ServiceTaskCategoryEnum.PREVENTIVE)]
    [InlineData(ServiceTaskCategoryEnum.CORRECTIVE)]
    [InlineData(ServiceTaskCategoryEnum.EMERGENCY)]
    [InlineData(ServiceTaskCategoryEnum.INSPECTION)]
    [InlineData(ServiceTaskCategoryEnum.WARRANTY)]
    public async Task Validator_Should_Pass_With_All_Valid_Categories(ServiceTaskCategoryEnum category)
    {
        var command = CreateValidCommand(category: category);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Null_Description()
    {
        var command = CreateValidCommand(description: null);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }
}