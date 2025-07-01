using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using AutoMapper;
using FluentValidation;
using MediatR;

namespace Application.Features.VehicleGroups.Command.UpdateVehicleGroup;

public class UpdateVehicleGroupCommandHandler : IRequestHandler<UpdateVehicleGroupCommand, int>
{
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<UpdateVehicleGroupCommandHandler> _logger;
    private readonly IValidator<UpdateVehicleGroupCommand> _validator;

    public UpdateVehicleGroupCommandHandler(IVehicleGroupRepository vehicleGroupRepository, IMapper mapper, IAppLogger<UpdateVehicleGroupCommandHandler> logger, IValidator<UpdateVehicleGroupCommand> validator)
    {
        _vehicleGroupRepository = vehicleGroupRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public Task<int> Handle(UpdateVehicleGroupCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
