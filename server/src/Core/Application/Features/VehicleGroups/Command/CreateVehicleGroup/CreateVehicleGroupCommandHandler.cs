using System.ComponentModel.DataAnnotations;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.VehicleGroups.Command.CreateVehicleGroup;

public class CreateVehicleGroupCommandHandler : IRequestHandler<CreateVehicleGroupCommand, int>
{
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateVehicleGroupCommandHandler> _logger;
    private readonly IValidator<CreateVehicleGroupCommand> _validator;

    public CreateVehicleGroupCommandHandler(IVehicleGroupRepository vehicleGroupRepository, IMapper mapper, IAppLogger<CreateVehicleGroupCommandHandler> logger, IValidator<CreateVehicleGroupCommand> validator)
    {
        _vehicleGroupRepository = vehicleGroupRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(CreateVehicleGroupCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateVehicleGroupCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to vehicle group domain entity
        var vehicleGroup = _mapper.Map<VehicleGroup>(request);

        // add new vehicle group
        var newVehicleGroup = await _vehicleGroupRepository.AddAsync(vehicleGroup);

        // save changes
        await _vehicleGroupRepository.SaveChangesAsync();

        return newVehicleGroup.ID;
    }
}
