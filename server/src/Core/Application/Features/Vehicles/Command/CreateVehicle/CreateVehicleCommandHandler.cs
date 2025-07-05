using System.ComponentModel.DataAnnotations;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.Vehicles.Command.CreateVehicle;

/// <summary>
/// Handles the <see cref="CreateVehicleCommand"/> by validating the request, enforcing business rules, mapping it to a <see cref="Vehicle"/>, persisting the entity and returning its identifier.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Implements the <see cref="IRequestHandler{TRequest,TResponse}"/> interface from the MediatR library, enabling the request/response mediator pattern (<see href="https://github.com/LuckyPennySoftware/MediatR?tab=readme-ov-file#registering-with-iservicecollection">AddMediatR registration</see>).
///     <list type="bullet">
///     <item><see href="https://refactoring.guru/design-patterns/mediator">Mediator Pattern</see></item>
///     </list>
/// </item>
/// <item>Performs asynchronous validation using FluentValidation's <c>ValidateAsync</c> method (<see href="https://github.com/FluentValidation/FluentValidation/blob/main/docs/async.md">Asynchronous validation example</see>).</item>
/// <item>Uses <see href="https://docs.automapper.io/en/latest/">AutoMapper</see> to transform the command into the domain <see cref="Vehicle"/> entity.</item>
/// <item>Logs diagnostic information via the injected <see cref="IAppLogger{T}"/> and throws rich application exceptions on failure.</item>
/// </list>
/// </remarks>
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

    /// <summary>
    /// Executes the command: validates input, checks domain/business constraints, persists the new vehicle and returns its ID.
    /// </summary>
    /// <param name="request">The <see cref="CreateVehicleCommand"/> containing vehicle details.</param>
    /// <param name="cancellationToken">Token to observe while awaiting async operations.</param>
    /// <returns>The ID of the newly created <see cref="Vehicle"/>.</returns>
    /// <exception cref="BadRequestException">Thrown when validation fails.</exception>
    /// <exception cref="DuplicateEntityException">Thrown when a VIN or licence plate already exists.</exception>
    /// <exception cref="EntityNotFoundException">Thrown when a referenced <see cref="VehicleGroup"/> or <see cref="User"/> is not found.</exception>
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

    /// <summary>
    /// Validates domain-specific uniqueness and foreign-key constraints before persistence.
    /// </summary>
    /// <param name="vehicle">The <see cref="Vehicle"/> entity to validate.</param>
    /// <exception cref="DuplicateEntityException">Thrown when VIN or licence plate duplicates are detected.</exception>
    /// <exception cref="EntityNotFoundException">Thrown when related entities (<see cref="VehicleGroup"/>, <see cref="User"/>) are not found.</exception>
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