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

    // NAME VALIDATION TESTS
    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Name_Is_Empty(string invalidName)
    {
        // Given
        var command = CreateValidCommand(name: invalidName);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    public async Task Validator_Should_Fail_When_Name_Exceeds_MaxLength(int nameLength)
    {
        // Given
        var command = CreateValidCommand(name: new string('a', nameLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100 characters"));
    }

    // DESCRIPTION VALIDATION TESTS
    [Theory]
    [InlineData(301)]
    [InlineData(500)]
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength(int descriptionLength)
    {
        // Given
        var command = CreateValidCommand(description: new string('a', descriptionLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description" && e.ErrorMessage.Contains("300 characters"));
    }
}
