using MediatR;

namespace Application.Features.ServicePrograms.Command.UpdateServiceProgram;

/// <summary>
/// Command for updating an existing ServiceProgram.
/// </summary>
/// <param name="ServiceProgramID">The ID of the service program to update.</param>
/// <param name="Name">The new name of the service program.</param>
/// <param name="Description">The new description (optional).</param>
/// <param name="IsActive">Whether the service program is active.</param>
/// <returns>The ID of the updated service program.</returns>
public record UpdateServiceProgramCommand(
    int ServiceProgramID,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<int>;