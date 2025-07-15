using Domain.Entities;

using MediatR;

namespace Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;

/// <summary>
/// Command for adding an existing <see cref="Vehicle"/> to an existing <see cref="ServiceProgram"/>.
/// </summary>
/// <param name="ServiceProgramID">The ID of the existing Service Program.</param>
/// <param name="VehicleID">The ID of the existing Vehicle to be added.</param>
/// <returns>A tuple containing the ID of the <see cref="ServiceProgram"/> and the ID of the <see cref="Vehicle"/>.</returns>
public record AddVehicleToServiceProgramCommand(
    int ServiceProgramID,
    int VehicleID
    // string AddedByUserID // TODO
) : IRequest<(int ServiceProgramID, int VehicleID)>;