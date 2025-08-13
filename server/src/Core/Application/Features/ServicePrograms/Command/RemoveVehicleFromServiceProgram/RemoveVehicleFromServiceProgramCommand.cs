using Domain.Entities;

using MediatR;

namespace Application.Features.ServicePrograms.Command.RemoveVehicleFromServiceProgram;

/// <summary>
/// Command for removing a <see cref="Vehicle"/> from a <see cref="ServiceProgram"/>.
/// </summary>
/// <param name="ServiceProgramID">The ID of the Service Program.</param>
/// <param name="VehicleID">The ID of the Vehicle to remove.</param>
public record RemoveVehicleFromServiceProgramCommand(
    int ServiceProgramID,
    int VehicleID
) : IRequest<(int ServiceProgramID, int VehicleID)>;