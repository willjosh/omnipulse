using Application.Features.InspectionForms.Command.UpdateInspectionForm;

using Domain.Entities;

namespace Application.Test.InspectionForms.CommandTest.UpdateInspectionForm;

public class UpdateInspectionFormCommandValidatorTest
{
    private readonly UpdateInspectionFormCommandValidator _validator = new();

    // Constants matching the validator
    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 1000;

    private static UpdateInspectionFormCommand CreateValidCommand(
        int inspectionFormID = 1,
        string title = $"Test {nameof(InspectionForm)} Title",
        string? description = $"Test {nameof(InspectionForm)} Description",
        bool isActive = true) => new(inspectionFormID, title, description, isActive);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_InspectionFormID_Is_Invalid(int invalidInspectionFormID)
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormID = invalidInspectionFormID };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormCommand.InspectionFormID));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_InspectionFormID_Is_Positive()
    {
        // Arrange
        var command = CreateValidCommand() with { InspectionFormID = 123 };

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
    public async Task Validator_Should_Fail_When_Title_Is_Empty_Or_Whitespace(string invalidTitle)
    {
        // Arrange
        var command = CreateValidCommand() with { Title = invalidTitle };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormCommand.Title));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Title_Exceeds_MaxLength()
    {
        // Arrange
        var longTitle = new string('x', TitleMaxLength + 1); // Exceeds max length
        var command = CreateValidCommand() with { Title = longTitle };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormCommand.Title));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Title_Is_At_MaxLength()
    {
        // Arrange
        var maxLengthTitle = new string('x', TitleMaxLength); // Exactly at max length
        var command = CreateValidCommand() with { Title = maxLengthTitle };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand() with { Description = null };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand() with { Description = "" };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength()
    {
        // Arrange
        var longDescription = new string('x', DescriptionMaxLength + 1); // Exceeds max length
        var command = CreateValidCommand() with { Description = longDescription };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormCommand.Description));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_At_MaxLength()
    {
        // Arrange
        var maxLengthDescription = new string('x', DescriptionMaxLength); // Exactly at max length
        var command = CreateValidCommand() with { Description = maxLengthDescription };

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validator_Should_Pass_When_IsActive_Is_Valid_Boolean(bool isActive)
    {
        // Arrange
        var command = CreateValidCommand() with { IsActive = isActive };

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
            inspectionFormID: 1,
            title: "Valid Title",
            description: "Valid description",
            isActive: true);

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
            inspectionFormID: -1, // Invalid ID
            title: "", // Empty title
            description: new string('x', DescriptionMaxLength + 1), // Too long description
            isActive: true);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 2); // At least ID and title errors
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormCommand.InspectionFormID));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormCommand.Title));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateInspectionFormCommand.Description));
    }
}