using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using MediatR;

namespace Application.Features.FuelPurchases.Query.GetFuelPurchase;

public sealed class GetFuelPurchaseQueryHandler : IRequestHandler<GetFuelPurchaseQuery, FuelPurchaseDTO>
{
    private readonly IFuelPurchaseRepository _fuelPurchaseRepository;
    private readonly IValidator<GetFuelPurchaseQuery> _validator;
    private readonly IAppLogger<GetFuelPurchaseQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetFuelPurchaseQueryHandler(
        IFuelPurchaseRepository fuelPurchaseRepository,
        IValidator<GetFuelPurchaseQuery> validator,
        IAppLogger<GetFuelPurchaseQueryHandler> logger,
        IMapper mapper)
    {
        _fuelPurchaseRepository = fuelPurchaseRepository;
        _validator = validator;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<FuelPurchaseDTO> Handle(GetFuelPurchaseQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(GetFuelPurchaseQuery)}({request.FuelPurchaseID})");

        // Validate request
        var validationResult = await _validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(GetFuelPurchaseQuery)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        var entity = await _fuelPurchaseRepository.GetByIdAsync(request.FuelPurchaseID);
        if (entity == null)
        {
            _logger.LogError($"{nameof(FuelPurchase)} with ID {request.FuelPurchaseID} not found.");
            throw new EntityNotFoundException(nameof(FuelPurchase), nameof(FuelPurchase.ID), request.FuelPurchaseID.ToString());
        }

        var dto = _mapper.Map<FuelPurchaseDTO>(entity);
        return dto;
    }
}