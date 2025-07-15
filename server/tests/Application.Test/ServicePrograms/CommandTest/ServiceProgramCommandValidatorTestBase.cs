using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Test.ServicePrograms.CommandTest.CreateServiceProgram;
// using Application.Test.ServicePrograms.CommandTest.UpdateServiceProgram; // TODO: Not implemented yet

using FluentValidation;

using Xunit;
// using Application.Test.ServicePrograms.CommandTest.UpdateServiceProgram; // TODO: Not implemented yet

namespace Application.Test.ServicePrograms.CommandTest;

/// <summary>
/// Base validator test for:
/// <list type="bullet">
///   <item><see cref="CreateServiceProgramCommandValidatorTest"/></item>
///   <item><see cref="UpdateServiceProgramCommandValidatorTest"/></item>
/// </list>
/// </summary>
/// <typeparam name="TCommand">The type of command to test.</typeparam>
/// <typeparam name="TValidator">The type of validator to test. Must be <see cref="IValidator{TCommand}"/>.</typeparam>
public abstract class ServiceProgramCommandValidatorTestBase<TCommand, TValidator>
    where TValidator : IValidator<TCommand>
{
    // Constants
    protected const int NameMaxLength = 200;
    protected const int DescriptionMaxLength = 500;

    protected abstract TValidator Validator { get; }

    protected abstract TCommand CreateValidCommand(
        string name = "Service Program Name",
        string? description = "Service Program Description",
        bool isActive = true);

    [Fact]
    public void Validator_Should_Not_Be_Null()
    {
        Assert.NotNull(Validator);
    }

    [Fact]
    public void Should_PassValidation_WithValidData()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void Should_PassValidation_WhenNameAtMaxLength()
    {
        // Arrange
        var name = new string('N', NameMaxLength);
        var command = CreateValidCommand(name: name);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void Should_FailValidation_WhenNameIsMissingOrWhitespace(string? name)
    {
        // Arrange
        var command = CreateValidCommand(name: name!);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateServiceProgramCommand.Name));
    }

    [Theory]
    [InlineData(NameMaxLength + 1)]
    public void Should_FailValidation_WhenNameTooLong(int nameLength)
    {
        // Arrange
        var name = new string('N', nameLength);
        var command = CreateValidCommand(name: name);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateServiceProgramCommand.Name));
    }

    [Fact]
    public void Should_PassValidation_WithNullDescription()
    {
        // Arrange
        var command = CreateValidCommand(description: null);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CreateServiceProgramCommand.Description));
    }

    [Fact]
    public void Should_PassValidation_WithEmptyDescription()
    {
        // Arrange
        var command = CreateValidCommand(description: string.Empty);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CreateServiceProgramCommand.Description));
    }

    [Theory]
    [InlineData(DescriptionMaxLength + 1)]
    public void Should_FailValidation_WhenDescriptionTooLong(int descLength)
    {
        // Arrange
        var description = new string('D', descLength);
        var command = CreateValidCommand(description: description);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateServiceProgramCommand.Description));
    }

    [Fact]
    public void Should_PassValidation_WhenDescriptionAtMaxLength()
    {
        // Arrange
        var description = new string('D', DescriptionMaxLength);
        var command = CreateValidCommand(description: description);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public void Should_PassValidation_WhenDescriptionIsWhitespace(string whitespace)
    {
        // Arrange
        var command = CreateValidCommand(description: whitespace);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CreateServiceProgramCommand.Description));
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public void Should_PassValidation_WithIsActive(bool isActive)
    {
        // Arrange
        var command = CreateValidCommand(isActive: isActive);

        // Act
        var result = Validator.Validate(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == nameof(CreateServiceProgramCommand.IsActive));
    }
}