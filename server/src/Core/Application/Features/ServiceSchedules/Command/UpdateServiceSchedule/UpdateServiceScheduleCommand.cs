using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;

/// <summary>
/// Command for updating an existing service schedule.
/// </summary>
/// <remarks>
/// Validation rules mirror <see cref="CreateServiceScheduleCommand"/> with the additional requirement of a positive <see cref="ServiceScheduleID"/>.
/// </remarks>
public record UpdateServiceScheduleCommand(
    int ServiceScheduleID,
    int ServiceProgramID,
    string Name,
    int? TimeIntervalValue,
    TimeUnitEnum? TimeIntervalUnit,
    int? TimeBufferValue,
    TimeUnitEnum? TimeBufferUnit,
    int? MileageInterval,
    int? MileageBuffer,
    int? FirstServiceTimeValue,
    TimeUnitEnum? FirstServiceTimeUnit,
    int? FirstServiceMileage,
    bool IsActive
) : IRequest<int>;