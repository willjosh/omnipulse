using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.InventoryItems.Query.GetAllInventoryItem;

public class GetAllInventoryItemQueryHandler : IRequestHandler<GetAllInventoryItemQuery, PagedResult<GetAllInventoryItemDTO>>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllInventoryItemQueryHandler> _logger;
    private readonly IValidator<GetAllInventoryItemQuery> _validator;

    public GetAllInventoryItemQueryHandler(IInventoryItemRepository inventoryItemRepository, IMapper mapper, IAppLogger<GetAllInventoryItemQueryHandler> logger, IValidator<GetAllInventoryItemQuery> validator)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllInventoryItemDTO>> Handle(GetAllInventoryItemQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllInventoryItemQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllInventoryItem - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all inventory items from the repository
        var result = await _inventoryItemRepository.GetAllInventoryItemsPagedAsync(request.Parameters);

        // map the inventory items to DTOs
        var inventoryItemDTOs = _mapper.Map<List<GetAllInventoryItemDTO>>(result.Items);

        var pagedResult = new PagedResult<GetAllInventoryItemDTO>
        {
            Items = inventoryItemDTOs,
            TotalCount = result.TotalCount,
            PageNumber = result.PageNumber,
            PageSize = result.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} inventory items for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}