using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;

using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;

/// <summary>
/// Command for updating an existing service schedule.
/// </summary>
/// <remarks>
/// Validation rules mirror <see cref="CreateServiceScheduleCommand"/> with the additional requirement of a positive <see cref="ServiceScheduleID"/>.
/// <list type="bullet">
/// <item>Exactly one of the following recurrence options must be provided: time-based (TimeIntervalValue &amp; TimeIntervalUnit) OR mileage-based (MileageInterval).</item>
/// <item>Buffer values are optional but, if provided, cannot be greater than or equal to their corresponding interval.</item>
/// <item>First service properties are optional. If provided, they require their related interval fields to be set.</item>
/// </list>
/// </remarks>
/// <param name="ServiceScheduleID">The ID of the service schedule to update.</param>
/// <param name="ServiceProgramID">The ID of the service program this schedule belongs to.</param>
/// <param name="Name">The name of the service schedule.</param>
/// <param name="ServiceTaskIDs">The list of service task IDs to associate with this schedule.</param>
/// <param name="TimeIntervalValue">The time interval value for the schedule (requires TimeIntervalUnit).</param>
/// <param name="TimeIntervalUnit">The time interval unit for the schedule (requires TimeIntervalValue).</param>
/// <param name="TimeBufferValue">The time buffer value for the schedule (optional).</param>
/// <param name="TimeBufferUnit">The time buffer unit for the schedule (optional).</param>
/// <param name="MileageInterval">The mileage interval for the schedule in kilometres.</param>
/// <param name="MileageBuffer">The mileage buffer for the schedule in kilometres.</param>
/// <param name="FirstServiceDate">The absolute date for the first service (requires TimeIntervalValue and TimeIntervalUnit).</param>
/// <param name="FirstServiceMileage">The absolute mileage for the first service (requires MileageInterval).</param>
/// <param name="IsActive">Whether the schedule is active.</param>
public record UpdateServiceScheduleCommand(
    int ServiceScheduleID,
    int ServiceProgramID,
    string Name,
    List<int> ServiceTaskIDs,
    int? TimeIntervalValue,
    TimeUnitEnum? TimeIntervalUnit,
    int? TimeBufferValue,
    TimeUnitEnum? TimeBufferUnit,
    int? MileageInterval,
    int? MileageBuffer,
    DateTime? FirstServiceDate,
    int? FirstServiceMileage,
    bool IsActive
) : IRequest<int>;