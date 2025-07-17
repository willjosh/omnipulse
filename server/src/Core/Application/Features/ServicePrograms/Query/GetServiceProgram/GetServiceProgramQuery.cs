using MediatR;

namespace Application.Features.ServicePrograms.Query.GetServiceProgram;

/// <summary>
/// Query for retrieving a single service program by its ID.
/// </summary>
/// <param name="ServiceProgramID">The ID of the Service Program.</param>
public record GetServiceProgramQuery(int ServiceProgramID) : IRequest<ServiceProgramDTO>;