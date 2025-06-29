using Application.Contracts.Persistence;
using Application.Contracts.Logger;
using MediatR;
using AutoMapper;

namespace Application.Features.Vehicles.Command.DeactivateVehicle;

public class DeactivateVehicleCommandHandler : IRequestHandler<DeactivateVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAppLogger<DeactivateVehicleCommandHandler> _logger;
    private readonly IMapper _mapper;

    public DeactivateVehicleCommandHandler(IVehicleRepository vehicleRepository, IAppLogger<DeactivateVehicleCommandHandler> logger, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public Task<int> Handle(DeactivateVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
