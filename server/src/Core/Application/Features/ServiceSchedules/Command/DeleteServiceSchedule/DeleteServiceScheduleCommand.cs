using MediatR;

namespace Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;

/// <param name="ServiceScheduleID">The ID of the service schedule to delete.</param>
/// <returns>The ID of the deleted service schedule.</returns>
/// <exception cref="Application.Exceptions.EntityNotFoundException">
/// Thrown when the service schedule with the specified ID does not exist.
/// </exception>
public record DeleteServiceScheduleCommand(
    int ServiceScheduleID
) : IRequest<int>;