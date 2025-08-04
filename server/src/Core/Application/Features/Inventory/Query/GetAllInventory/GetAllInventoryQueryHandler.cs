using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Inventory.Query;

public class GetAllInventoryQueryHandler : IRequestHandler<GetAllInventoryQuery, PagedResult<InventoryDetailDTO>>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllInventoryQueryHandler> _logger;
    private readonly IValidator<GetAllInventoryQuery> _validator;
    public GetAllInventoryQueryHandler(IInventoryRepository inventoryRepository, IMapper mapper, IAppLogger<GetAllInventoryQueryHandler> logger, IValidator<GetAllInventoryQuery> validator)
    {
        _inventoryRepository = inventoryRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<InventoryDetailDTO>> Handle(GetAllInventoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(GetAllInventoryQuery)}");

        // validate the request
        var validationResult = await _validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"{nameof(GetAllInventoryQuery)} - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // Get paginated inventory
        var pagedInventory = await _inventoryRepository.GetAllInventoriesPagedAsync(request.Parameters);

        // Map to DTOs
        var inventoryDtos = _mapper.Map<List<InventoryDetailDTO>>(pagedInventory.Items);

        var result = new PagedResult<InventoryDetailDTO>
        {
            Items = inventoryDtos,
            TotalCount = pagedInventory.TotalCount,
            PageNumber = pagedInventory.PageNumber,
            PageSize = pagedInventory.PageSize
        };

        _logger.LogInformation($"Retrieved {result.Items.Count} inventory items from page {result.PageNumber}");
        return result;
    }
}