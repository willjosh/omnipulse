using System;

using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;

using Xunit;

namespace Application.Test.ServiceSchedules.CommandTest.CreateServiceSchedule;

public class CreateServiceScheduleCommandValidatorTest
{
    private readonly CreateServiceScheduleCommandValidator _validator = new();

    private static CreateServiceScheduleCommand CreateValidCommand(
        int serviceProgramId = 1,
        string name = "5000 km / 6 month service",
        int intervalMileage = 5000,
        int intervalDays = 180,
        int intervalHours = 0,
        int bufferMileage = 250,
        int bufferDays = 7,
        bool isActive = true) => new(
            ServiceProgramID: serviceProgramId,
            Name: name,
            IntervalMileage: intervalMileage,
            IntervalDays: intervalDays,
            IntervalHours: intervalHours,
            BufferMileage: bufferMileage,
            BufferDays: bufferDays,
            IsActive: isActive);

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        var command = CreateValidCommand();
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_ServiceProgramID_Invalid(int invalidId)
    {
        var command = CreateValidCommand(serviceProgramId: invalidId);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceProgramID");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_Name_Empty_Or_Whitespace(string invalidName)
    {
        var command = CreateValidCommand(name: invalidName);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Name");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_MeterInterval_Provided_And_TimeInterval_Zero()
    {
        var command = CreateValidCommand(intervalMileage: 5000, intervalDays: 0, intervalHours: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_TimeInterval_Provided_And_MeterIntervals_Zero()
    {
        var command = CreateValidCommand(intervalMileage: 0, intervalDays: 180, intervalHours: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_All_Intervals_Zero()
    {
        var command = CreateValidCommand(intervalMileage: 0, intervalDays: 0, intervalHours: 0);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.ErrorMessage.Contains("meter interval") && e.ErrorMessage.Contains("time interval"));
    }

    [Theory]
    [InlineData(-5)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_BufferMileage_Negative(int invalid)
    {
        var command = CreateValidCommand(bufferMileage: invalid);
        var result = await _validator.ValidateAsync(command);
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "BufferMileage");
    }
}