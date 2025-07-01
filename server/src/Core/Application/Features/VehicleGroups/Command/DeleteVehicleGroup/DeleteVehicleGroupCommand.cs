using MediatR;

namespace Application.Features.VehicleGroups.Command.DeleteVehicleGroup;

public record DeleteVehicleGroupCommand(
    int VehicleGroupID
) : IRequest<int>;