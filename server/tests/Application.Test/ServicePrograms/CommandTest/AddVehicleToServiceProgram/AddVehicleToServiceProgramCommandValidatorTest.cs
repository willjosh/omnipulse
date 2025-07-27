using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;

namespace Application.Test.ServicePrograms.CommandTest.AddVehicleToServiceProgram;

public class AddVehicleToServiceProgramCommandValidatorTest
{
    private readonly AddVehicleToServiceProgramCommandValidator _validator = new();

    private static AddVehicleToServiceProgramCommand CreateValidCommand(int serviceProgramID = 1, int vehicleID = 1)
        => new(serviceProgramID, vehicleID);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_ServiceProgramID_Is_Invalid(int invalidServiceProgramID)
    {
        // Arrange
        var command = CreateValidCommand(serviceProgramID: invalidServiceProgramID);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(AddVehicleToServiceProgramCommand.ServiceProgramID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_VehicleID_Is_Invalid(int invalidVehicleID)
    {
        // Arrange
        var command = CreateValidCommand(vehicleID: invalidVehicleID);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(AddVehicleToServiceProgramCommand.VehicleID));
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Ids_Are_Positive()
    {
        // Arrange
        var command = CreateValidCommand(serviceProgramID: 123, vehicleID: 456);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}