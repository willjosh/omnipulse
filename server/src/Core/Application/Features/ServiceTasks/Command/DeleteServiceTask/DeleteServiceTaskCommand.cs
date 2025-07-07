using MediatR;

namespace Application.Features.ServiceTasks.Command.DeleteServiceTask;

/// <param name="ServiceTaskID">The ID of the service task to delete.</param>
/// <returns>The ID of the deleted service task.</returns>
/// <exception cref="Application.Exceptions.EntityNotFoundException">
/// Thrown when the service task with the specified ID does not exist.
/// </exception>
public record DeleteServiceTaskCommand(
    int ServiceTaskID
) : IRequest<int>;