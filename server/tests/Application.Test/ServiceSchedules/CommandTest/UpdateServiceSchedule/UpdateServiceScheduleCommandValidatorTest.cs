using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;

using Domain.Entities.Enums;

namespace Application.Test.ServiceSchedules.CommandTest.UpdateServiceSchedule;

public class UpdateServiceScheduleCommandValidatorTest
    : ServiceScheduleCommandValidatorTestBase<UpdateServiceScheduleCommand, UpdateServiceScheduleCommandValidator>
{
    protected override UpdateServiceScheduleCommandValidator Validator { get; } = new();

    protected override UpdateServiceScheduleCommand CreateValidCommand(
        int serviceProgramId = 1,
        string name = "5000 km / 6 week service",
        List<int>? serviceTaskIDs = null,
        int? timeIntervalValue = 6,
        TimeUnitEnum? timeIntervalUnit = TimeUnitEnum.Weeks,
        int? timeBufferValue = 1,
        TimeUnitEnum? timeBufferUnit = TimeUnitEnum.Days,
        int? mileageInterval = null,
        int? mileageBuffer = null,
        DateTime? firstServiceDate = null,
        int? firstServiceMileage = null,
        bool isActive = true) => new(
            ServiceScheduleID: 1,
            ServiceProgramID: serviceProgramId,
            Name: name,
            ServiceTaskIDs: serviceTaskIDs ?? [1],
            TimeIntervalValue: timeIntervalValue,
            TimeIntervalUnit: timeIntervalUnit,
            TimeBufferValue: timeBufferValue,
            TimeBufferUnit: timeBufferUnit,
            MileageInterval: mileageInterval,
            MileageBuffer: mileageBuffer,
            FirstServiceDate: firstServiceDate,
            FirstServiceMileage: firstServiceMileage,
            IsActive: isActive);

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-100)]
    public async Task Validator_Should_Fail_When_ServiceScheduleID_Is_Invalid(int invalidServiceScheduleID)
    {
        // Arrange
        var command = CreateValidCommand() with { ServiceScheduleID = invalidServiceScheduleID };

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ServiceScheduleID");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_ServiceScheduleID_Is_Positive()
    {
        // Arrange
        var command = CreateValidCommand() with { ServiceScheduleID = 123 };

        // Act
        var result = await Validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }
}