using System;
using Application.Features.VehicleGroups.Command.UpdateVehicleGroup;
using Xunit;

namespace Application.Test.VehicleGroups.CommandTest.UpdateVehicleGroup;

public class UpdateVehicleGroupValidatorTest
{
    private readonly UpdateVehicleGroupCommandValidator _validator;

    public UpdateVehicleGroupValidatorTest()
    {
        _validator = new UpdateVehicleGroupCommandValidator();
    }
    
    private UpdateVehicleGroupCommand CreateValidCommand(
        int vehicleGroupId = 1,
        string name = "Test Group",
        string? description = "Test Description",
        bool isActive = true
    )
    {
        return new UpdateVehicleGroupCommand(vehicleGroupId, name, description, isActive);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Given
        var command = CreateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}
