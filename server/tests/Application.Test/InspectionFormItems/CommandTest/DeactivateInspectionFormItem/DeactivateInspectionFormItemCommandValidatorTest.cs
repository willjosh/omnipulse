using Application.Features.InspectionFormItems.Command.DeactivateInspectionFormItem;

namespace Application.Test.InspectionFormItems.CommandTest.DeactivateInspectionFormItem;

public class DeactivateInspectionFormItemCommandValidatorTest
{
    private readonly DeactivateInspectionFormItemCommandValidator _validator = new();

    private static DeactivateInspectionFormItemCommand CreateValidCommand(int inspectionFormItemID = 1) => new(inspectionFormItemID);

    [Fact]
    public async Task Validator_Should_Pass_When_Command_Is_Valid()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_InspectionFormItemID_Is_Invalid(int invalidInspectionFormItemID)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormItemID = invalidInspectionFormItemID };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeactivateInspectionFormItemCommand.InspectionFormItemID));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Validator_Should_Pass_When_InspectionFormItemID_Is_Valid(int validInspectionFormItemID)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormItemID = validInspectionFormItemID };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Have_Correct_Error_Message_For_Invalid_InspectionFormItemID()
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormItemID = 0 };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        var error = Assert.Single(result.Errors);
        Assert.Equal(nameof(DeactivateInspectionFormItemCommand.InspectionFormItemID), error.PropertyName);
    }
}