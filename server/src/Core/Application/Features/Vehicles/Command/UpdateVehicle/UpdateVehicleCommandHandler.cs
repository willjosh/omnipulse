using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Vehicles.Command.UpdateVehicle;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Vehicles.Command.UpdateVehicle;

public class UpdateVehicleCommandHandler : IRequestHandler<UpdateVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IUserRepository _userRepository;

    private readonly IValidator<UpdateVehicleCommand> _validator;
    private readonly IAppLogger<UpdateVehicleCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateVehicleCommandHandler(
        IVehicleRepository vehicleRepository,
        IVehicleGroupRepository vehicleGroupRepository,
        IUserRepository userRepository,
        IValidator<UpdateVehicleCommand> validator,
        IAppLogger<UpdateVehicleCommandHandler> logger,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _vehicleGroupRepository = vehicleGroupRepository;
        _userRepository = userRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateVehicleCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"UpdateVehicleCommandHandler - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Check if vehicle exists
        var existingVehicle = await _vehicleRepository.GetByIdAsync(request.VehicleID);
        if (existingVehicle == null)
        {
            var errorMessage = $"UpdateVehicleCommandHandler - Vehicle ID not found: {request.VehicleID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "VehicleID", request.VehicleID.ToString());
        }

        // Validate business rules
        await ValidateBusinessRulesAsync(request, existingVehicle);

        // Map request to vehicle entity (this will update the existing vehicle properties)
        _mapper.Map(request, existingVehicle);

        // Update vehicle
        _vehicleRepository.Update(existingVehicle);

        // Save changes
        await _vehicleRepository.SaveChangesAsync();

        _logger.LogInformation($"UpdateVehicleCommandHandler - Successfully updated vehicle with ID: {request.VehicleID}");

        // Return vehicleID
        return existingVehicle.ID;
    }

    private async Task ValidateBusinessRulesAsync(UpdateVehicleCommand request, Vehicle existingVehicle)
    {
        // Check for duplicate VIN only if VIN is being changed
        if (request.VIN != existingVehicle.VIN && await _vehicleRepository.VinExistAsync(request.VIN))
        {
            var errorMessage = $"UpdateVehicleCommandHandler - Vehicle VIN already exists: {request.VIN}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(Vehicle).ToString(), "VIN", request.VIN);
        }

        // Validate VehicleGroupID exists
        if (!await _vehicleGroupRepository.ExistsAsync(request.VehicleGroupID))
        {
            var errorMessage = $"UpdateVehicleCommandHandler - VehicleGroup ID not found: {request.VehicleGroupID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(VehicleGroup).ToString(), "VehicleGroupID", request.VehicleGroupID.ToString());
        }

        // Validate AssignedTechnicianID if provided (note: AssignedTechnicianID is not in UpdateVehicleCommand but exists in Vehicle entity)
        // Since AssignedTechnicianID is not part of UpdateVehicleCommand, we'll preserve the existing value
        if (!string.IsNullOrEmpty(existingVehicle.AssignedTechnicianID) && !await _userRepository.ExistsAsync(existingVehicle.AssignedTechnicianID))
        {
            var errorMessage = $"UpdateVehicleCommandHandler - Assigned technician ID not found: {existingVehicle.AssignedTechnicianID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(User).ToString(), "AssignedTechnicianID", existingVehicle.AssignedTechnicianID);
        }
    }
}
