using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.FuelPurchases.Command.DeleteFuelPurchase;

public class DeleteFuelPurchaseCommandHandler : IRequestHandler<DeleteFuelPurchaseCommand, int>
{
    private readonly IFuelPurchaseRepository _fuelPurchaseRepository;
    private readonly IValidator<DeleteFuelPurchaseCommand> _validator;
    private readonly IAppLogger<DeleteFuelPurchaseCommandHandler> _logger;

    public DeleteFuelPurchaseCommandHandler(
        IFuelPurchaseRepository fuelPurchaseRepository,
        IValidator<DeleteFuelPurchaseCommand> validator,
        IAppLogger<DeleteFuelPurchaseCommandHandler> logger)
    {
        _fuelPurchaseRepository = fuelPurchaseRepository;
        _validator = validator;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteFuelPurchaseCommand request, CancellationToken cancellationToken)
    {
        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(DeleteFuelPurchaseCommand)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get FuelPurchase by ID
        var targetFuelPurchase = await _fuelPurchaseRepository.GetByIdAsync(request.FuelPurchaseID);
        if (targetFuelPurchase == null)
        {
            _logger.LogError($"{nameof(FuelPurchase)} with {nameof(FuelPurchase.ID)} {request.FuelPurchaseID} not found.");
            throw new EntityNotFoundException(nameof(FuelPurchase), nameof(FuelPurchase.ID), request.FuelPurchaseID.ToString());
        }

        // Delete
        _fuelPurchaseRepository.Delete(targetFuelPurchase);

        // Save changes
        await _fuelPurchaseRepository.SaveChangesAsync();

        // Return deleted ID
        return request.FuelPurchaseID;
    }
}