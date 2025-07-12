using System;
using System.Threading.Tasks;

using Application.Features.InventoryItemLocations.Command;
using Application.Features.InventoryItemLocations.Command.CreateInventoryItemLocation;

using Xunit;

namespace Application.Test.InventoryItemLocations.CommandTest;

public class CreateInventoryItemLocationCommandValidatorTest
{
    private readonly CreateInventoryItemLocationCommandValidator _validator;

    public CreateInventoryItemLocationCommandValidatorTest()
    {
        _validator = new CreateInventoryItemLocationCommandValidator();
    }

    private CreateInventoryItemLocationCommand CreateValidCommand(
        string locationName = "Warehouse A",
        string address = "123 Main St",
        double longitude = 100.0,
        double latitude = 10.0,
        int capacity = 50)
    {
        return new CreateInventoryItemLocationCommand(
            locationName,
            address,
            longitude,
            latitude,
            capacity
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_LocationName_Is_Empty(string invalidName)
    {
        var command = CreateValidCommand(locationName: invalidName);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LocationName");
    }

    [Theory]
    [InlineData(101)]
    [InlineData(200)]
    public async Task Validator_Should_Fail_When_LocationName_Exceeds_MaxLength(int nameLength)
    {
        var command = CreateValidCommand(locationName: new string('A', nameLength));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "LocationName");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Address_Is_Empty(string invalidAddress)
    {
        var command = CreateValidCommand(address: invalidAddress);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Address");
    }

    [Theory]
    [InlineData(256)]
    [InlineData(300)]
    public async Task Validator_Should_Fail_When_Address_Exceeds_MaxLength(int addressLength)
    {
        var command = CreateValidCommand(address: new string('A', addressLength));
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Address");
    }

    [Theory]
    [InlineData(-181)]
    [InlineData(181)]
    public async Task Validator_Should_Fail_When_Longitude_Is_Out_Of_Range(double longitude)
    {
        var command = CreateValidCommand(longitude: longitude);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Longitude");
    }

    [Theory]
    [InlineData(-91)]
    [InlineData(91)]
    public async Task Validator_Should_Fail_When_Latitude_Is_Out_Of_Range(double latitude)
    {
        var command = CreateValidCommand(latitude: latitude);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Latitude");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_Capacity_Is_Zero_Or_Negative(int capacity)
    {
        var command = CreateValidCommand(capacity: capacity);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Capacity");
    }
}