using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.Vehicles.Command.UpdateVehicle;
using AutoMapper;
using MediatR;

namespace Application.Features.Vehicles.Command.UpdateVehicle;

public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UpdateVehicleCommandHandler> _logger;

    public UpdateVehicleCommandHandler(IVehicleRepository VehicleRepository, IMapper Mapper, IAppLogger<UpdateVehicleCommandHandler> Logger)
    {
        _vehicleRepository = VehicleRepository;
        _mapper = Mapper;
        _logger = Logger;
    }

    public Task<int> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
