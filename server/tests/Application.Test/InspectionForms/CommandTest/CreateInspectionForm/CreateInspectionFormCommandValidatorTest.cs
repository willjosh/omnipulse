using Application.Features.InspectionForms.Command.CreateInspectionForm;

namespace Application.Test.InspectionForms.CommandTest.CreateInspectionForm;

public class CreateInspectionFormCommandValidatorTest
{
    private readonly CreateInspectionFormCommandValidator _validator;

    public CreateInspectionFormCommandValidatorTest()
    {
        _validator = new CreateInspectionFormCommandValidator();
    }

    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 1000;

    private static CreateInspectionFormCommand CreateValidCommand(
        string title = "Test Inspection Title",
        string? description = "Test Inspection Description",
        bool isActive = true) => new(title, description, isActive);

    [Fact]
    public void Validate_Should_Pass_For_Valid_Command()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void Validate_Should_Fail_When_Title_Is_Empty_Or_Null(string? invalidTitle)
    {
        // Arrange
        var command = CreateValidCommand(title: invalidTitle!);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormCommand.Title));
    }

    [Fact]
    public void Validate_Should_Fail_When_Title_Exceeds_Maximum_Length()
    {
        // Arrange
        var longTitle = new string('a', TitleMaxLength + 1); // 201 characters (exceeds 200 limit)
        var command = CreateValidCommand(title: longTitle);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormCommand.Title));
    }

    [Fact]
    public void Validate_Should_Pass_When_Title_Is_At_Maximum_Length()
    {
        // Arrange
        var titleAtMaxLength = new string('a', TitleMaxLength);
        var command = CreateValidCommand(title: titleAtMaxLength);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Pass_When_Description_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand(description: null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Pass_When_Description_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand(description: "");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Fail_When_Description_Exceeds_Maximum_Length()
    {
        // Arrange
        var longDescription = new string('a', DescriptionMaxLength + 1);
        var command = CreateValidCommand(description: longDescription);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormCommand.Description));
    }

    [Fact]
    public void Validate_Should_Pass_When_Description_Is_At_Maximum_Length()
    {
        // Arrange
        var descriptionAtMaxLength = new string('a', DescriptionMaxLength); // Exactly 1000 characters
        var command = CreateValidCommand(description: descriptionAtMaxLength);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Validate_Should_Pass_For_Any_IsActive_Value(bool isActive)
    {
        // Arrange
        var command = CreateValidCommand(isActive: isActive);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Return_Multiple_Errors_For_Multiple_Invalid_Fields()
    {
        // Arrange
        var command = CreateValidCommand(
            title: "", // Invalid - empty
            description: new string('a', DescriptionMaxLength + 1) // Invalid - too long
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormCommand.Title));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionFormCommand.Description));
    }
}