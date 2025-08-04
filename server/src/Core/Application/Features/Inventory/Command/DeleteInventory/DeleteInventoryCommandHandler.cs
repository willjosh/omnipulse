using System;

using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using MediatR;

using Microsoft.Extensions.Logging;

namespace Application.Features.Inventory.Command.DeleteInventory;

public class DeleteInventoryCommandHandler : IRequestHandler<DeleteInventoryCommand, int>
{
    private readonly IInventoryRepository _inventoryRepository;
    private readonly ILogger<DeleteInventoryCommandHandler> _logger;

    public DeleteInventoryCommandHandler(IInventoryRepository inventoryRepository, ILogger<DeleteInventoryCommandHandler> logger)
    {
        _inventoryRepository = inventoryRepository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteInventoryCommand request, CancellationToken cancellationToken)
    {
        // validate InventoryID
        _logger.LogInformation($"Validating Inventory with ID: {request.InventoryID}");
        var inventory = _inventoryRepository.GetByIdAsync(request.InventoryID).Result;
        if (inventory == null)
        {
            _logger.LogError($"Inventory with ID {request.InventoryID} not found.");
            throw new EntityNotFoundException(typeof(Domain.Entities.Inventory).ToString(), "ID", request.InventoryID.ToString());
        }

        // delete Inventory
        _inventoryRepository.Delete(inventory);

        // save changes
        await _inventoryRepository.SaveChangesAsync();
        _logger.LogInformation($"Inventory with ID: {request.InventoryID} deleted");

        return request.InventoryID;
    }
}