using Application.Features.ServicePrograms.Command.DeleteServiceProgram;

namespace Application.Test.ServicePrograms.CommandTest.DeleteServiceProgram;

public class DeleteServiceProgramCommandValidatorTest
{
    private readonly DeleteServiceProgramCommandValidator _validator = new();

    private static DeleteServiceProgramCommand CreateCommand(int serviceProgramID) => new(serviceProgramID);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_ServiceProgramID_Is_Invalid(int invalidServiceProgramID)
    {
        // Arrange
        var command = CreateCommand(invalidServiceProgramID);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(DeleteServiceProgramCommand.ServiceProgramID));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ServiceProgramID_Is_Positive()
    {
        // Arrange
        var command = CreateCommand(123);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}