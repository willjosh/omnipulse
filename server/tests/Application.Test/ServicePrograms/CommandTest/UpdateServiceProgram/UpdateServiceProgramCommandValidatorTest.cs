using Application.Features.ServicePrograms.Command.UpdateServiceProgram;

namespace Application.Test.ServicePrograms.CommandTest.UpdateServiceProgram;

public class UpdateServiceProgramCommandValidatorTest : ServiceProgramCommandValidatorTestBase<UpdateServiceProgramCommand, UpdateServiceProgramCommandValidator>
{
    protected override UpdateServiceProgramCommandValidator Validator { get; } = new();

    protected override UpdateServiceProgramCommand CreateValidCommand(
        string name = "Service Program Name",
        string? description = "Service Program Description",
        bool isActive = true)
        => new(ServiceProgramID: 1, name, description, isActive);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_ServiceProgramID_Is_Invalid(int invalidServiceProgramID)
    {
        // Arrange
        var command = CreateValidCommand() with { ServiceProgramID = invalidServiceProgramID };

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(UpdateServiceProgramCommand.ServiceProgramID));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ServiceProgramID_Is_Positive()
    {
        // Arrange
        var command = CreateValidCommand() with { ServiceProgramID = 123 };

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }
}