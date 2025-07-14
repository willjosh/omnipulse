using MediatR;
namespace Application.Features.InventoryItemLocations.Command.DeleteInventoryItemLocation;

public record DeleteInventoryItemLocationCommand
(
    int InventoryItemLocationID
) : IRequest<int>;