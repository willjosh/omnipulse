using MediatR;

namespace Application.Features.ServicePrograms.Command.DeleteServiceProgram;

/// <summary>
/// Command to delete a Service Program by ID.
/// </summary>
/// <param name="ServiceProgramID">The ID of the Service Program to delete.</param>
public record DeleteServiceProgramCommand(int ServiceProgramID) : IRequest<int>;