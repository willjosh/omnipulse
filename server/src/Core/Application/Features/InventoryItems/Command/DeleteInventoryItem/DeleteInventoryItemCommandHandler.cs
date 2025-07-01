using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.InventoryItems.Command.DeleteInventoryItem;

public class DeleteInventoryItemCommandHandler : IRequestHandler<DeleteInventoryItemCommand, int>
{
    private readonly IInventoryItemRepository _inventoryItemRepository;
    private readonly IAppLogger<DeleteInventoryItemCommandHandler> _logger;
    private readonly IMapper _mapper;

    public DeleteInventoryItemCommandHandler(IInventoryItemRepository inventoryItemRepository, IAppLogger<DeleteInventoryItemCommandHandler> logger, IMapper mapper)
    {
        _inventoryItemRepository = inventoryItemRepository;
        _logger = logger;
        _mapper = mapper;
    }

    public async Task<int> Handle(DeleteInventoryItemCommand request, CancellationToken cancellationToken)
    {
        // Validate InventoryItemID
        _logger.LogInformation($"Validating InventoryItem with ID: {request.InventoryItemID}");
        var inventoryItem = await _inventoryItemRepository.GetByIdAsync(request.InventoryItemID);
        if (inventoryItem == null)
        {
            _logger.LogError($"InventoryItem with ID {request.InventoryItemID} not found.");
            throw new EntityNotFoundException(typeof(InventoryItem).ToString(), "ID", request.InventoryItemID.ToString());
        }

        // Delete InventoryItem
        _inventoryItemRepository.Delete(inventoryItem);

        // Save Changes
        await _inventoryItemRepository.SaveChangesAsync();
        _logger.LogInformation($"InventoryItem with ID: {request.InventoryItemID} deleted");

        return request.InventoryItemID;
    }
}