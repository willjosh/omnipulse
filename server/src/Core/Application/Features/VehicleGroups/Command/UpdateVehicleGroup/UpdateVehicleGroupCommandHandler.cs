using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
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

    public async Task<int> Handle(UpdateVehicleGroupCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateVehicleGroupCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if vehicle group exists
        var vehicleGroup = await _vehicleGroupRepository.GetByIdAsync(request.VehicleGroupId);
        if (vehicleGroup == null)
        {
            var errorMessage = $"Vehicle group ID not found: {request.VehicleGroupId}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(VehicleGroup).ToString(), "VehicleGroupId", request.VehicleGroupId.ToString());
        }

        // Map request to vehicle group entity (this will update the existing vehicle group properties)
        _mapper.Map(request, vehicleGroup);

        // Update vehicle group
        _vehicleGroupRepository.Update(vehicleGroup);

        // Save changes
        await _vehicleGroupRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated vehicle group with ID: {request.VehicleGroupId}");

        // Return vehicle group ID
        return vehicleGroup.ID;
    }
}
