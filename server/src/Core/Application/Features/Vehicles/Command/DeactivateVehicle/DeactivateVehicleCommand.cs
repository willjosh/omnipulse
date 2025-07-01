using System;

using MediatR;

namespace Application.Features.Vehicles.Command.DeactivateVehicle;

public record DeactivateVehicleCommand(
    int VehicleID
) : IRequest<int>;