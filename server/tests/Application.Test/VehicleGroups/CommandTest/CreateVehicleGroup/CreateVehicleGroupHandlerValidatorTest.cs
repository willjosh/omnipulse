using System;

using Application.Features.VehicleGroups.Command.CreateVehicleGroup;

using Xunit;

namespace Application.Test.VehicleGroups.CommandTest.CreateVehicleGroup;

public class CreateVehicleGroupHandlerValidatorTest
{
    private readonly CreateVehicleGroupCommandValidator _validator;

    public CreateVehicleGroupHandlerValidatorTest()
    {
        _validator = new CreateVehicleGroupCommandValidator();
    }

    private CreateVehicleGroupCommand CreateValidCommand(
        string name = "Test Group",
        string? description = "Test Description",
        bool isActive = true
    )
    {
        return new CreateVehicleGroupCommand(name, description, isActive);
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
    [InlineData(101)]  // Exceeds 100 limit
    [InlineData(200)]  // Way over limit
    public async Task Validator_Should_Fail_When_Name_Exceeds_MaxLength(int nameLength)
    {
        // Given
        var command = CreateValidCommand(name: new string('A', nameLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name" && e.ErrorMessage.Contains("100 characters"));
    }

    // DESCRIPTION VALIDATION TESTS
    [Theory]
    [InlineData(301)]  // Exceeds 300 limit
    [InlineData(500)]  // Way over limit
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength(int descriptionLength)
    {
        // Given
        var command = CreateValidCommand(description: new string('A', descriptionLength));

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description" && e.ErrorMessage.Contains("300 characters"));
    }

    [Theory]
    [InlineData("")]
    [InlineData(null)]
    public async Task Validator_Should_Pass_When_Description_Is_Empty(string? emptyDescription)
    {
        // Given
        var command = CreateValidCommand(description: emptyDescription);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.DoesNotContain(result.Errors, e => e.PropertyName == "Description");
    }
}