using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.FuelPurchases.Query.GetAllFuelPurchases;

public class GetAllFuelPurchasesQueryHandler : IRequestHandler<GetAllFuelPurchasesQuery, PagedResult<GetAllFuelPurchasesDTO>>
{
    private readonly IFuelPurchaseRepository _fuelPurchasesRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllFuelPurchasesQueryHandler> _logger;
    private readonly IValidator<GetAllFuelPurchasesQuery> _validator;

    public GetAllFuelPurchasesQueryHandler(IFuelPurchaseRepository fuelPurchasesRepository, IMapper mapper, IAppLogger<GetAllFuelPurchasesQueryHandler> logger, IValidator<GetAllFuelPurchasesQuery> validator)
    {
        _fuelPurchasesRepository = fuelPurchasesRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllFuelPurchasesDTO>> Handle(GetAllFuelPurchasesQuery request, CancellationToken cancellationToken)
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
        var fuelPurchasesDTOs = _mapper.Map<List<GetAllFuelPurchasesDTO>>(result.Items);

        var pagedResult = new PagedResult<GetAllFuelPurchasesDTO>
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