using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.FuelLogging.Command.CreateFuelPurchase;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.FuelLogging.Command.CreateFuelPurchase;

public class CreateFuelPurchaseCommandHandler : IRequestHandler<CreateFuelPurchaseCommand, int>
{
    private readonly IFuelPurchaseRepository _fuelPurchaseRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateFuelPurchaseCommandHandler> _logger;
    private readonly IValidator<CreateFuelPurchaseCommand> _validator;

    public CreateFuelPurchaseCommandHandler(IFuelPurchaseRepository fuelPurchaseRepository, IVehicleRepository vehicleRepository, IUserRepository userRepository, IMapper mapper, IAppLogger<CreateFuelPurchaseCommandHandler> logger, IValidator<CreateFuelPurchaseCommand> validator)
    {
        _fuelPurchaseRepository = fuelPurchaseRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<int> Handle(CreateFuelPurchaseCommand request, CancellationToken cancellationToken)
    {
        // validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"CreateFuelPurchaseCommand - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // map request to fuel purchase domain entity
        var fuelPurchase = _mapper.Map<FuelPurchase>(request);

        // validate business rules
        await ValidateBusinessRuleAsync(fuelPurchase);

        // add new fuel purchase
        var newFuelPurchase = await _fuelPurchaseRepository.AddAsync(fuelPurchase);

        // save changes
        await _fuelPurchaseRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully created fuel purchase with ID: {newFuelPurchase.ID}");

        return newFuelPurchase.ID;
    }

    private async Task ValidateBusinessRuleAsync(FuelPurchase fuelPurchase)
    {
        // validate user who logged fuel
        if (!await _userRepository.ExistsAsync(fuelPurchase.PurchasedByUserId))
        {
            var errorMessage = $"User ID not found: {fuelPurchase.PurchasedByUserId}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(User), "PurchasedByUserId", fuelPurchase.PurchasedByUserId);
        }

        // validate vehicle exists
        if (!await _vehicleRepository.ExistsAsync(fuelPurchase.VehicleId))
        {
            var errorMessage = $"Vehicle ID not found: {fuelPurchase.VehicleId}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(Vehicle), "VehicleId", fuelPurchase.VehicleId.ToString());
        }

        // Validate odometer reading is greater than previous reading
        if (!await _fuelPurchaseRepository.IsValidOdometerReading(fuelPurchase.VehicleId, fuelPurchase.OdometerReading))
        {
            var errorMessage = "New odometer reading must be greater than last recorded reading.";
            _logger.LogWarning($"CreateFuelPurchaseCommand - Odometer validation failed: {errorMessage}");
            throw new BadRequestException(errorMessage);
        }
    }
}