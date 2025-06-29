using System;
using Application.Contracts.Persistence;
using Application.Features.Vehicle.Command.CreateVehicle;
using MediatR;

namespace Application.Features.Vehicles.Command.CreateVehicle;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;

    public CreateVehicleCommandHandler(IVehicleRepository VehicleRepository)
    {
        _vehicleRepository = VehicleRepository;
    }

    public Task<int> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
