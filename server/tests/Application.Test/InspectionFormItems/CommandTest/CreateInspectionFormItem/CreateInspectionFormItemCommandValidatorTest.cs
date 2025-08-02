using Application.Features.InspectionFormItems.Command.CreateInspectionFormItem;

using Domain.Entities.Enums;

namespace Application.Test.InspectionFormItems.CommandTest.CreateInspectionFormItem;

public class CreateInspectionFormItemCommandValidatorTest
{
    private readonly CreateInspectionFormItemCommandValidator _validator = new();

    // Constants matching the validator
    private const int ItemLabelMaxLength = 200;
    private const int ItemDescriptionMaxLength = 500;
    private const int ItemInstructionsMaxLength = 4000;

    private static CreateInspectionFormItemCommand CreateValidCommand(
        int inspectionFormID = 1,
        string itemLabel = "Test Inspection Item Label",
        string? itemDescription = "Verify engine oil level and quality",
        string? itemInstructions = "Remove dipstick, check oil level between min/max marks",
        InspectionFormItemTypeEnum itemType = InspectionFormItemTypeEnum.PassFail,
        bool isRequired = true) => new(inspectionFormID, itemLabel, itemDescription, itemInstructions, itemType, isRequired);

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
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.InspectionFormID));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public async Task Validator_Should_Pass_When_InspectionFormID_Is_Valid(int validInspectionFormID)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormID = validInspectionFormID };

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
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.ItemLabel));
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
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.ItemLabel));
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
        Assert.Empty(result.Errors);
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
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.ItemDescription));
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
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.ItemInstructions));
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
    [InlineData(InspectionFormItemTypeEnum.PassFail)]
    public async Task Validator_Should_Pass_When_InspectionFormItemTypeEnum_Is_Valid(InspectionFormItemTypeEnum validItemType)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormItemTypeEnum = validItemType };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_InspectionFormItemTypeEnum_Is_Invalid()
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormItemTypeEnum = (InspectionFormItemTypeEnum)9999 };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.InspectionFormItemTypeEnum));
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
    public async Task Validator_Should_Return_Multiple_Errors_When_Multiple_Properties_Invalid()
    {
        // Arrange
        var command = CreateValidCommand(
            inspectionFormID: -1, // Invalid ID
            itemLabel: "", // Empty label
            itemDescription: new string('x', ItemDescriptionMaxLength + 1), // Too long description
            itemInstructions: new string('x', ItemInstructionsMaxLength + 1), // Too long instructions
            itemType: (InspectionFormItemTypeEnum)9999); // Invalid enum

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 5); // At least 5 errors
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.InspectionFormID));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.ItemLabel));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.ItemDescription));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.ItemInstructions));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormItemCommand.InspectionFormItemTypeEnum));
    }
}