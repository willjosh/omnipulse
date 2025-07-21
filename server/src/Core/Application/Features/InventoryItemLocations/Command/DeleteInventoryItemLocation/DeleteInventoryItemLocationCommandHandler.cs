
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;

using MediatR;

namespace Application.Features.InventoryItemLocations.Command.DeleteInventoryItemLocation;

public class DeleteInventoryItemLocationCommandHandler : IRequestHandler<DeleteInventoryItemLocationCommand, int>
{
    private readonly IInventoryItemLocationRepository _inventoryItemLocationRepository;
    private readonly IAppLogger<DeleteInventoryItemLocationCommandHandler> _logger;

    public DeleteInventoryItemLocationCommandHandler(IInventoryItemLocationRepository inventoryItemLocationRepository, IAppLogger<DeleteInventoryItemLocationCommandHandler> logger)
    {
        _inventoryItemLocationRepository = inventoryItemLocationRepository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteInventoryItemLocationCommand request, CancellationToken cancellationToken)
    {
        // Validate InventoryItemLocationID
        _logger.LogInformation($"Validating InventoryItemLocation with ID: {request.InventoryItemLocationID}");
        var inventoryItemLocation = await _inventoryItemLocationRepository.GetByIdAsync(request.InventoryItemLocationID);
        if (inventoryItemLocation == null)
        {
            _logger.LogError($"InventoryItemLocation with ID {request.InventoryItemLocationID} not found.");
            throw new EntityNotFoundException(typeof(InventoryItemLocation).ToString(), "ID", request.InventoryItemLocationID.ToString());
        }

        // Delete InventoryItemLocation
        _inventoryItemLocationRepository.Delete(inventoryItemLocation);

        // Save Changes
        await _inventoryItemLocationRepository.SaveChangesAsync();
        _logger.LogInformation($"InventoryItemLocation with ID: {request.InventoryItemLocationID} deleted");

        return request.InventoryItemLocationID;
    }
}