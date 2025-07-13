
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

    public Task<int> Handle(DeleteInventoryItemLocationCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}