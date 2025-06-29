using System;
using MediatR;

namespace Application.Features.Vehicles.Command.DeleteVehicle;

public record DeleteVehicleCommand(
    int VehicleID
) : IRequest<int>;
