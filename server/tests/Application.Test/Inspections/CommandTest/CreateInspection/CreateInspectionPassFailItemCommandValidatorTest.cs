using Application.Features.Inspections.Command.CreateInspection;

namespace Application.Test.Inspections.CommandTest.CreateInspection;

public class CreateInspectionPassFailItemCommandValidatorTest
{
    private readonly CreateInspectionPassFailItemCommandValidator _validator;

    public CreateInspectionPassFailItemCommandValidatorTest()
    {
        _validator = new CreateInspectionPassFailItemCommandValidator();
    }

    private const int CommentMaxLength = 1000;

    private static CreateInspectionPassFailItemCommand CreateValidCommand(
        int inspectionFormItemID = 1,
        bool passed = true,
        string? comment = "Test comment") => new(inspectionFormItemID, passed, comment);

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
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public void Validate_Should_Fail_When_InspectionFormItemID_Is_Invalid(int invalidId)
    {
        // Arrange
        var command = CreateValidCommand(inspectionFormItemID: invalidId);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionPassFailItemCommand.InspectionFormItemID));
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(9999)]
    public void Validate_Should_Pass_When_InspectionFormItemID_Is_Valid(int validId)
    {
        // Arrange
        var command = CreateValidCommand(inspectionFormItemID: validId);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Validate_Should_Pass_When_Passed_Is_Valid_Boolean(bool passed)
    {
        // Arrange
        var command = CreateValidCommand(passed: passed);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Pass_When_Comment_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand(comment: null);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Pass_When_Comment_Is_Empty()
    {
        // Arrange
        var command = CreateValidCommand(comment: "");

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public void Validate_Should_Fail_When_Comment_Exceeds_Maximum_Length()
    {
        // Arrange
        var longComment = new string('a', CommentMaxLength + 1);
        var command = CreateValidCommand(comment: longComment);

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionPassFailItemCommand.Comment));
    }

    [Fact]
    public void Validate_Should_Pass_When_Comment_Is_At_Maximum_Length()
    {
        // Arrange
        var commentAtMaxLength = new string('a', CommentMaxLength);
        var command = CreateValidCommand(comment: commentAtMaxLength);

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
            inspectionFormItemID: 0, // Invalid
            comment: new string('a', CommentMaxLength + 1) // Invalid - too long
        );

        // Act
        var result = _validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionPassFailItemCommand.InspectionFormItemID));
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateInspectionPassFailItemCommand.Comment));
    }
}