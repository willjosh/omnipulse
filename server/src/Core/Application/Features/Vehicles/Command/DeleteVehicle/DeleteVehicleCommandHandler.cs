using Application.Contracts.Persistence;
using Application.Contracts.Logger;
using MediatR;
using AutoMapper;

namespace Application.Features.Vehicles.Command.DeleteVehicle;

public class DeleteVehicleCommandHandler : IRequestHandler<DeleteVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAppLogger<DeleteVehicleCommandHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteVehicleCommandHandler(IVehicleRepository vehicleRepository, IAppLogger<DeleteVehicleCommandHandler> logger, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public Task<int> Handle(DeleteVehicleCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
