using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.FuelPurchases.Query.GetAllFuelPurchase;

public class GetAllFuelPurchaseQueryHandler : IRequestHandler<GetAllFuelPurchaseQuery, PagedResult<FuelPurchaseDTO>>
{
    private readonly IFuelPurchaseRepository _fuelPurchasesRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllFuelPurchaseQueryHandler> _logger;
    private readonly IValidator<GetAllFuelPurchaseQuery> _validator;

    public GetAllFuelPurchaseQueryHandler(IFuelPurchaseRepository fuelPurchasesRepository, IMapper mapper, IAppLogger<GetAllFuelPurchaseQueryHandler> logger, IValidator<GetAllFuelPurchaseQuery> validator)
    {
        _fuelPurchasesRepository = fuelPurchasesRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<FuelPurchaseDTO>> Handle(GetAllFuelPurchaseQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllFuelPurchasesQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllFuelPurchases - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all FuelPurchasess from the repository
        var result = await _fuelPurchasesRepository.GetAllFuelPurchasesPagedAsync(request.Parameters);

        // map the FuelPurchasess to DTOs
        var fuelPurchasesDTOs = _mapper.Map<List<FuelPurchaseDTO>>(result.Items);

        var pagedResult = new PagedResult<FuelPurchaseDTO>
        {
            Items = fuelPurchasesDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} FuelPurchasess for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}