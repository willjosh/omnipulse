using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;

using Domain.Entities.Enums;

using Xunit;

namespace Application.Test.ServiceSchedules.CommandTest.CreateServiceSchedule;

public class CreateServiceScheduleCommandValidatorTest
    : ServiceScheduleCommandValidatorTestBase<CreateServiceScheduleCommand, CreateServiceScheduleCommandValidator>
{
    protected override CreateServiceScheduleCommandValidator Validator { get; } = new();

    protected override CreateServiceScheduleCommand CreateValidCommand(
        int serviceProgramId = 1,
        string name = "5000 km / 6 week service",
        List<int>? serviceTaskIDs = null,
        int? timeIntervalValue = 6,
        TimeUnitEnum? timeIntervalUnit = TimeUnitEnum.Weeks,
        int? timeBufferValue = 1,
        TimeUnitEnum? timeBufferUnit = TimeUnitEnum.Days,
        int? mileageInterval = 5000,
        int? mileageBuffer = 250,
        int? firstServiceTimeValue = null,
        TimeUnitEnum? firstServiceTimeUnit = null,
        int? firstServiceMileage = null,
        bool isActive = true) => new(
            ServiceProgramID: serviceProgramId,
            Name: name,
            ServiceTaskIDs: serviceTaskIDs ?? [1],
            TimeIntervalValue: timeIntervalValue,
            TimeIntervalUnit: timeIntervalUnit,
            TimeBufferValue: timeBufferValue,
            TimeBufferUnit: timeBufferUnit,
            MileageInterval: mileageInterval,
            MileageBuffer: mileageBuffer,
            FirstServiceTimeValue: firstServiceTimeValue,
            FirstServiceTimeUnit: firstServiceTimeUnit,
            FirstServiceMileage: firstServiceMileage,
            IsActive: isActive);
}