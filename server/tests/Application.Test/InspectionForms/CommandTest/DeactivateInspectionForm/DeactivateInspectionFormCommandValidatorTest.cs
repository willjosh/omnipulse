using Application.Features.InspectionForms.Command.DeactivateInspectionForm;

namespace Application.Test.InspectionForms.CommandTest.DeactivateInspectionForm;

public class DeactivateInspectionFormCommandValidatorTest
{
    private readonly DeactivateInspectionFormCommandValidator _validator = new();

    private static DeactivateInspectionFormCommand CreateValidCommand(int inspectionFormID = 1) => new(inspectionFormID);

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
    public async Task Validator_Should_Fail_When_InspectionFormID_Is_Invalid(int invalidInspectionFormID)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormID = invalidInspectionFormID };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeactivateInspectionFormCommand.InspectionFormID));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Validator_Should_Pass_When_InspectionFormID_Is_Valid(int validInspectionFormID)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormID = validInspectionFormID };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}