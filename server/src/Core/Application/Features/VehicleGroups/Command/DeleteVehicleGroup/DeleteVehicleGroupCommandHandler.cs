using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using AutoMapper;
using MediatR;

namespace Application.Features.VehicleGroups.Command.DeleteVehicleGroup;

public class DeleteVehicleGroupCommandHandler : IRequestHandler<DeleteVehicleGroupCommand, int>
{
    private readonly IAppLogger<DeleteVehicleGroupCommandHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteVehicleGroupCommandHandler(IVehicleGroupRepository vehicleGroupRepository, IAppLogger<DeleteVehicleGroupCommandHandler> logger, IMapper mapper)
    {
        _logger = logger;
        _mapper = mapper;
    }

    public Task<int> Handle(DeleteVehicleGroupCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
