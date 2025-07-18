using MediatR;

namespace Application.Features.ServiceTasks.Query.GetServiceTask;

/// <summary>
/// Query for retrieving a single service task by its ID.
/// </summary>
/// <param name="ServiceTaskID">The unique identifier of the service task.</param>
public record GetServiceTaskQuery(int ServiceTaskID) : IRequest<ServiceTaskDTO>;