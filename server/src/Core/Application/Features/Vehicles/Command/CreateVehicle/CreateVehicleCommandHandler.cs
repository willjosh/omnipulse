using System.ComponentModel.DataAnnotations;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Vehicles.Command.CreateVehicle;

public class CreateVehicleCommandHandler : IRequestHandler<CreateVehicleCommand, int>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IVehicleGroupRepository _vehicleGroupRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateVehicleCommandHandler> _logger;
    private readonly IValidator<CreateVehicleCommand> _validator;

    public CreateVehicleCommandHandler(IVehicleRepository vehicleRepository, IUserRepository userRepository, IVehicleGroupRepository vehicleGroupRepository, IMapper mapper, IAppLogger<CreateVehicleCommandHandler> logger, IValidator<CreateVehicleCommand> validator)
    {
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _vehicleGroupRepository = vehicleGroupRepository;
        _validator = validator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<int> Handle(CreateVehicleCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateVehicleCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to vehicle domain entity
        var vehicle = _mapper.Map<Vehicle>(request);

        // validate business rules
        await ValidateBusinessRuleAsync(vehicle);

        // add new vehicle
        var newVehicle = await _vehicleRepository.AddAsync(vehicle);

        // save changes
        await _vehicleRepository.SaveChangesAsync();

        // return vehicleID
        return newVehicle.ID;
    }

    private async Task ValidateBusinessRuleAsync(Vehicle vehicle)
    {
        // check for duplicate VIN in the db
        if (await _vehicleRepository.VinExistAsync(vehicle.VIN))
        {
            var errorMessage = $"Vehicle VIN already exists: {vehicle.VIN}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(Vehicle).ToString(), "VIN", vehicle.VIN);
        }

        // Check for duplicate License Plate
        if (await _vehicleRepository.LicensePlateExistAsync(vehicle.LicensePlate))
        {
            var errorMessage = $"License plate already exists: {vehicle.LicensePlate}";
            _logger.LogError(errorMessage);
            throw new DuplicateEntityException(typeof(Vehicle).ToString(), "LicensePlate", vehicle.LicensePlate);
        }

        // validate groupID
        if (!await _vehicleGroupRepository.ExistsAsync(vehicle.VehicleGroupID))
        {
            var errorMessage = $"VehicleGroup ID not found: {vehicle.VehicleGroupID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "VehicleGroupID", vehicle.VehicleGroupID.ToString());
        }

        // validate technician if given
        if (!string.IsNullOrEmpty(vehicle.AssignedTechnicianID) && !await _userRepository.ExistsAsync(vehicle.AssignedTechnicianID))
        {
            var errorMessage = $"Technician ID not found: {vehicle.AssignedTechnicianID}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(typeof(Vehicle).ToString(), "AssignedTechnicianID", vehicle.AssignedTechnicianID);
        }
    }
}
