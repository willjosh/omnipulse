using Application.Features.ServiceTasks.Command.UpdateServiceTask;

using Domain.Entities.Enums;

namespace Application.Test.ServiceTasks.CommandTest.UpdateServiceTask;

public class UpdateServiceTaskCommandValidatorTest
{
    private readonly UpdateServiceTaskCommandValidator _validator;

    public UpdateServiceTaskCommandValidatorTest()
    {
        _validator = new UpdateServiceTaskCommandValidator();
    }

    private static UpdateServiceTaskCommand CreateValidCommand(
        int serviceTaskID = 123,
        string name = "Updated Service Task",
        string? description = "Updated test description",
        double estimatedLabourHours = 2.5,
        decimal estimatedCost = 150.00m,
        ServiceTaskCategoryEnum category = ServiceTaskCategoryEnum.PREVENTIVE,
        bool isActive = true)
    {
        return new UpdateServiceTaskCommand(
            ServiceTaskID: serviceTaskID,
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
        // Given
        var command = CreateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    // SERVICE TASK ID VALIDATION TESTS
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_ServiceTaskID_Is_Invalid(int invalidServiceTaskID)
    {
        // Given
        var command = CreateValidCommand(serviceTaskID: invalidServiceTaskID);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceTaskID");
    }

    // NAME VALIDATION TESTS
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Name_Is_Empty(string invalidName)
    {
        // Given
        var command = CreateValidCommand(name: invalidName);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Name_Is_Whitespace_Only()
    {
        // Given
        var command = CreateValidCommand(name: "   ");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(101)]  // Exceeds 100 limit
    [InlineData(200)]  // Way over limit
    public async Task Validator_Should_Fail_When_Name_Exceeds_MaxLength(int nameLength)
    {
        // Given
        var command = CreateValidCommand(name: new string('A', nameLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    // DESCRIPTION VALIDATION TESTS
    [Theory]
    [InlineData(501)]  // Exceeds 500 limit
    [InlineData(1000)] // Way over limit
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength(int descriptionLength)
    {
        // Given
        var command = CreateValidCommand(description: new string('A', descriptionLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_Null()
    {
        // Given
        var command = CreateValidCommand(description: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
    }

    // ESTIMATED LABOUR HOURS VALIDATION TESTS
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_EstimatedLabourHours_Is_Zero_Or_Negative(double invalidHours)
    {
        // Given
        var command = CreateValidCommand(estimatedLabourHours: invalidHours);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedLabourHours");
    }

    [Theory]
    [InlineData(1001)]  // Exceeds 1000 limit
    [InlineData(2000)]  // Way over limit
    public async Task Validator_Should_Fail_When_EstimatedLabourHours_Exceeds_Limit(double invalidHours)
    {
        // Given
        var command = CreateValidCommand(estimatedLabourHours: invalidHours);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedLabourHours");
    }

    // ESTIMATED COST VALIDATION TESTS
    [Theory]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_EstimatedCost_Is_Negative(decimal invalidCost)
    {
        // Given
        var command = CreateValidCommand(estimatedCost: invalidCost);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedCost");
    }

    [Theory]
    [InlineData(1000000.00)]  // Exceeds 999,999.99 limit
    [InlineData(2000000.00)]  // Way over limit
    public async Task Validator_Should_Fail_When_EstimatedCost_Exceeds_Limit(decimal invalidCost)
    {
        // Given
        var command = CreateValidCommand(estimatedCost: invalidCost);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "EstimatedCost");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_EstimatedCost_Is_Zero()
    {
        // Given
        var command = CreateValidCommand(estimatedCost: 0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
    }

    // CATEGORY VALIDATION TESTS
    [Fact]
    public async Task Validator_Should_Fail_When_Category_Is_Invalid_Enum()
    {
        // Given - using invalid enum value
        var command = CreateValidCommand(category: (ServiceTaskCategoryEnum)999);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Category");
    }

    [Fact]
    public async Task Validator_Should_Pass_With_All_Valid_Categories()
    {
        // Given - test all valid categories
        var categories = Enum.GetValues<ServiceTaskCategoryEnum>();

        foreach (var category in categories)
        {
            var command = CreateValidCommand(category: category);

            // When
            var result = await _validator.ValidateAsync(command);

            // Then
            Assert.True(result.IsValid, $"Category {category} should be valid");
        }
    }
}