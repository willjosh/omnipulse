using Domain.Entities;

using MediatR;

namespace Application.Features.ServicePrograms.Command.CreateServiceProgram;

/// <summary>
/// Command for creating a new <see cref="ServiceProgram"/>.
/// </summary>
/// <param name="Name">The name of the service program.</param>
/// <param name="Description">The description of the service program (optional).</param>
/// <param name="IsActive">Whether the service program is active (default true).</param>
/// <returns>The ID of the newly created <see cref="ServiceProgram"/>.</returns>
public record CreateServiceProgramCommand(
    string Name,
    string? Description,
    bool IsActive = true
) : IRequest<int>;