using System.ComponentModel.DataAnnotations;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.VehicleGroups.Command.CreateVehicleGroup;

public class CreateVehicleGroupHandler : IRequestHandler<CreateVehicleGroupCommand, int>
{
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateVehicleGroupHandler> _logger;
    private readonly IValidator<CreateVehicleGroupCommand> _validator;

    public CreateVehicleGroupHandler(IVehicleGroupRepository vehicleGroupRepository, IMapper mapper, IAppLogger<CreateVehicleGroupHandler> logger, IValidator<CreateVehicleGroupCommand> validator)
    {
        _vehicleGroupRepository = vehicleGroupRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public Task<int> Handle(CreateVehicleGroupCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
