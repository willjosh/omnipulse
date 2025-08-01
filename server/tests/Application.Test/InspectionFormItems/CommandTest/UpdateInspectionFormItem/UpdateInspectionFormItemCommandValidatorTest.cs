using Application.Features.InspectionFormItems.Command.UpdateInspectionFormItem;

namespace Application.Test.InspectionFormItems.CommandTest.UpdateInspectionFormItem;

public class UpdateInspectionFormItemCommandValidatorTest
{
    private readonly UpdateInspectionFormItemCommandValidator _validator = new();

    // Constants
    private const int ItemLabelMaxLength = 200;
    private const int ItemDescriptionMaxLength = 500;
    private const int ItemInstructionsMaxLength = 4000;

    private static UpdateInspectionFormItemCommand CreateValidCommand(
        int inspectionFormItemID = 1,
        string itemLabel = "Updated Engine Oil Check",
        string? itemDescription = "Updated description",
        string? itemInstructions = "Updated instructions",
        bool isRequired = true) => new(inspectionFormItemID, itemLabel, itemDescription, itemInstructions, isRequired);

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
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.InspectionFormItemID));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public async Task Validator_Should_Pass_When_InspectionFormItemID_Is_Valid(int validID)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormItemID = validID };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("")]
    [InlineData(" ")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task Validator_Should_Fail_When_ItemLabel_Is_Empty_Or_Whitespace(string invalidItemLabel)
    {
        // Arrange
        var command = CreateValidCommand() with { ItemLabel = invalidItemLabel };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.ItemLabel));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ItemLabel_Exceeds_MaxLength()
    {
        // Arrange
        var longItemLabel = new string('x', ItemLabelMaxLength + 1);
        var command = CreateValidCommand() with { ItemLabel = longItemLabel };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.ItemLabel));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ItemLabel_Is_At_MaxLength()
    {
        // Arrange
        var maxLengthItemLabel = new string('x', ItemLabelMaxLength);
        var command = CreateValidCommand() with { ItemLabel = maxLengthItemLabel };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ItemDescription_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand() with { ItemDescription = null };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ItemDescription_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand() with { ItemDescription = "" };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ItemDescription_Exceeds_MaxLength()
    {
        // Arrange
        var longItemDescription = new string('x', ItemDescriptionMaxLength + 1);
        var command = CreateValidCommand() with { ItemDescription = longItemDescription };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.ItemDescription));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ItemDescription_Is_At_MaxLength()
    {
        // Arrange
        var maxLengthItemDescription = new string('x', ItemDescriptionMaxLength);
        var command = CreateValidCommand() with { ItemDescription = maxLengthItemDescription };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ItemInstructions_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand() with { ItemInstructions = null };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ItemInstructions_Exceeds_MaxLength()
    {
        // Arrange
        var longItemInstructions = new string('x', ItemInstructionsMaxLength + 1);
        var command = CreateValidCommand() with { ItemInstructions = longItemInstructions };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.ItemInstructions));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ItemInstructions_Is_At_MaxLength()
    {
        // Arrange
        var maxLengthItemInstructions = new string('x', ItemInstructionsMaxLength);
        var command = CreateValidCommand() with { ItemInstructions = maxLengthItemInstructions };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validator_Should_Pass_When_IsRequired_Is_Valid_Boolean(bool isRequired)
    {
        // Arrange
        var command = CreateValidCommand() with { IsRequired = isRequired };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_All_Properties_Are_Valid()
    {
        // Arrange
        var command = CreateValidCommand(
            inspectionFormItemID: 1,
            itemLabel: "Updated Brake Check",
            itemDescription: "Updated brake fluid level check",
            itemInstructions: "Updated instructions for checking brake fluid",
            isRequired: false);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Return_Multiple_Errors_When_Multiple_Properties_Invalid()
    {
        // Arrange
        var command = CreateValidCommand(
            inspectionFormItemID: -1, // Invalid ID
            itemLabel: "", // Empty label
            itemDescription: new string('x', ItemDescriptionMaxLength + 1), // Too long description
            itemInstructions: new string('x', ItemInstructionsMaxLength + 1)); // Too long instructions

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 4); // At least 4 errors
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.InspectionFormItemID));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.ItemLabel));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.ItemDescription));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormItemCommand.ItemInstructions));
    }
}