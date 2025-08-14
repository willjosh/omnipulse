using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.FuelPurchases.Command.UpdateFuelPurchase;

public sealed class UpdateFuelPurchaseCommandHandler : IRequestHandler<UpdateFuelPurchaseCommand, int>
{
    private readonly IFuelPurchaseRepository _fuelPurchaseRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IValidator<UpdateFuelPurchaseCommand> _validator;
    private readonly IAppLogger<UpdateFuelPurchaseCommandHandler> _logger;
    private readonly IMapper _mapper;

    public UpdateFuelPurchaseCommandHandler(
        IFuelPurchaseRepository fuelPurchaseRepository,
        IVehicleRepository vehicleRepository,
        IUserRepository userRepository,
        IValidator<UpdateFuelPurchaseCommand> validator,
        IAppLogger<UpdateFuelPurchaseCommandHandler> logger,
        IMapper mapper)
    {
        _fuelPurchaseRepository = fuelPurchaseRepository;
        _vehicleRepository = vehicleRepository;
        _userRepository = userRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(UpdateFuelPurchaseCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(UpdateFuelPurchaseCommand)} for ID: {request.FuelPurchaseId}");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(UpdateFuelPurchaseCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Load existing entity
        var existing = await _fuelPurchaseRepository.GetByIdAsync(request.FuelPurchaseId);
        if (existing is null)
        {
            var errorMessage = $"{nameof(FuelPurchase)} {nameof(FuelPurchase.ID)} not found: {request.FuelPurchaseId}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(FuelPurchase), nameof(FuelPurchase.ID), request.FuelPurchaseId.ToString());
        }

        // Validate business rules against request
        await ValidateBusinessRulesAsync(request, existing, cancellationToken);

        // Map request to existing entity
        _mapper.Map(request, existing);

        // Recalculate derived values if needed
        existing.CalculateTotalCost();

        // Update vehicle mileage if odometer reading is equal or higher
        var vehicle = await _vehicleRepository.GetByIdAsync(existing.VehicleId);
        if (vehicle is not null && existing.OdometerReading >= vehicle.Mileage)
        {
            vehicle.Mileage = existing.OdometerReading;
            _vehicleRepository.Update(vehicle);
        }

        // Persist
        _fuelPurchaseRepository.Update(existing);
        await _fuelPurchaseRepository.SaveChangesAsync();

        _logger.LogInformation($"Successfully updated {nameof(FuelPurchase)} with {nameof(FuelPurchase.ID)}: {existing.ID}");

        return existing.ID;
    }

    private async Task ValidateBusinessRulesAsync(UpdateFuelPurchaseCommand request, FuelPurchase existing, CancellationToken cancellationToken)
    {
        // Validate user exists
        if (!await _userRepository.ExistsAsync(request.PurchasedByUserId))
        {
            var errorMessage = $"User ID not found: {request.PurchasedByUserId}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(User), "PurchasedByUserId", request.PurchasedByUserId);
        }

        // Validate vehicle exists
        if (!await _vehicleRepository.ExistsAsync(request.VehicleId))
        {
            var errorMessage = $"Vehicle ID not found: {request.VehicleId}";
            _logger.LogError(errorMessage);
            throw new EntityNotFoundException(nameof(Vehicle), "VehicleId", request.VehicleId.ToString());
        }

        // If receipt number changed, ensure uniqueness
        if (!string.Equals(existing.ReceiptNumber, request.ReceiptNumber, StringComparison.Ordinal))
        {
            if (!await _fuelPurchaseRepository.IsReceiptNumberUniqueAsync(request.ReceiptNumber))
            {
                var errorMessage = $"Receipt number already exists: {request.ReceiptNumber}";
                _logger.LogError(errorMessage);
                throw new DuplicateEntityException(nameof(FuelPurchase), "ReceiptNumber", request.ReceiptNumber);
            }
        }

        // Validate odometer reading is >= last recorded for the vehicle
        if (!await _fuelPurchaseRepository.IsValidOdometerReading(request.VehicleId, request.OdometerReading))
        {
            var errorMessage = "New odometer reading must be greater than last recorded reading.";
            _logger.LogWarning($"{nameof(UpdateFuelPurchaseCommand)} - Odometer validation failed: {errorMessage}");
            throw new BadRequestException(errorMessage);
        }
    }
}