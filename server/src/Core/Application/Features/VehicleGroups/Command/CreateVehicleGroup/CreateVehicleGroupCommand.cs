using MediatR;

namespace Application.Features.VehicleGroups.Command.CreateVehicleGroup;

public record CreateVehicleGroupCommand(
    string Name,
    string? Description,
    bool IsActive
) : IRequest<int>;
