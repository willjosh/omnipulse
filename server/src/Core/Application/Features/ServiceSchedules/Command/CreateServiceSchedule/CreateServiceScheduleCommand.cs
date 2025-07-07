using Domain.Entities;

using MediatR;

namespace Application.Features.ServiceSchedules.Command.CreateServiceSchedule;

/// <summary>
/// Command for creating a new <see cref="ServiceSchedule"/> entity.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Either a meter interval or a time interval must be provided.</item>
/// </list>
/// </remarks>
/// <param name="ServiceProgramID">The ID of the service program this schedule belongs to.</param>
/// <param name="Name">The name of the service schedule.</param>
/// <param name="IntervalMileage">The mileage interval for the schedule.</param>
/// <param name="IntervalDays">The days interval for the schedule.</param>
/// <param name="IntervalHours">The hours interval for the schedule.</param>
/// <param name="BufferMileage">The mileage buffer for the schedule.</param>
/// <param name="BufferDays">The days buffer for the schedule.</param>
/// <param name="IsActive">Whether the schedule is active.</param>
/// <returns>The ID of the newly created service schedule.</returns>
public record CreateServiceScheduleCommand(
    int ServiceProgramID,
    string Name,
    int IntervalMileage,
    int IntervalDays,
    int IntervalHours,
    int BufferMileage,
    int BufferDays,
    bool IsActive = true
) : IRequest<int>;