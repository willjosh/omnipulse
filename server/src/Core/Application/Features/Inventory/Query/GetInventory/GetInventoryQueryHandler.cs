using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using MediatR;

namespace Application.Features.Inventory.Query.GetInventory;

public class GetInventoryQueryHandler : IRequestHandler<GetInventoryQuery, InventoryDetailDTO>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly IAppLogger<GetInventoryQueryHandler> _logger;
    private readonly IMapper _mapper;

    public GetInventoryQueryHandler(
        IInventoryRepository inventoryRepository,
        IAppLogger<GetInventoryQueryHandler> logger,
        IMapper mapper
    )
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<InventoryDetailDTO> Handle(GetInventoryQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"{nameof(Handle)} - Handling {nameof(GetInventoryQuery)}({request.InventoryID})");

        // Validate input
        if (request.InventoryID <= 0)
        {
            var errorMessage = $"Invalid inventory ID: {request.InventoryID}";
            _logger.LogError(errorMessage);
            throw new BadRequestException(errorMessage);
        }

        // Get inventory with all related data
        var inventory = await _inventoryRepository.GetInventoryWithDetailsAsync(request.InventoryID);

        // Check if inventory exists
        if (inventory == null)
        {
            _logger.LogError($"Inventory with ID {request.InventoryID} not found.");
            throw new EntityNotFoundException(typeof(Domain.Entities.Inventory).ToString(), nameof(Domain.Entities.Inventory.ID), request.InventoryID.ToString());
        }

        // Map to DTO
        var inventoryDto = _mapper.Map<InventoryDetailDTO>(inventory);

        _logger.LogInformation($"Returning InventoryDetailDTO for InventoryID: {request.InventoryID}");
        return inventoryDto;
    }
}