using MediatR;

namespace Application.Features.VehicleGroups.Command.UpdateVehicleGroup;

public record UpdateVehicleGroupCommand(
    int VehicleGroupId,
    string Name,
    string? Description,
    bool IsActive
) : IRequest<int>;