using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.Vehicle.Command.CreateVehicle;
using AutoMapper;
using MediatR;

namespace Application.Features.Vehicles.Command.CreateVehicle;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateVehicleCommandHandler> _logger;

    public CreateVehicleCommandHandler(IVehicleRepository VehicleRepository, IMapper Mapper, IAppLogger<CreateVehicleCommandHandler> Logger)
    {
        _vehicleRepository = VehicleRepository;
        _mapper = Mapper;
        _logger = Logger;
    }

    public Task<int> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
