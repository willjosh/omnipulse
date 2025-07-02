using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.InventoryItems.Query.GetInventoryItem;

public class GetInventoryItemQueryHandler : IRequestHandler<GetInventoryItemQuery, GetInventoryItemDTO>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IAppLogger<GetInventoryItemQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetInventoryItemQueryHandler(
        IInventoryItemRepository inventoryItemRepository,
        IAppLogger<GetInventoryItemQueryHandler> logger,
        IMapper mapper)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<GetInventoryItemDTO> Handle(GetInventoryItemQuery request, CancellationToken cancellationToken)
    {
        // Get inventory item by id
        _logger.LogInformation($"GetInventoryItemQuery for InventoryItemID: {request.InventoryItemID}");
        var inventoryItem = await _inventoryItemRepository.GetByIdAsync(request.InventoryItemID);

        // Check if inventory item exists
        if (inventoryItem == null)
        {
            _logger.LogError($"InventoryItem with ID {request.InventoryItemID} not found.");
            throw new EntityNotFoundException(typeof(InventoryItem).ToString(), "InventoryItemID", request.InventoryItemID.ToString());
        }

        // Map to GetInventoryItemDTO
        var inventoryItemDto = _mapper.Map<GetInventoryItemDTO>(inventoryItem);

        // Return GetInventoryItemDTO
        _logger.LogInformation($"Returning InventoryItemDTO for InventoryItemID: {request.InventoryItemID}");
        return inventoryItemDto;
    }
}