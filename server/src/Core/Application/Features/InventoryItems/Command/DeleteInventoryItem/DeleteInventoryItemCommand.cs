using MediatR;

namespace Application.Features.InventoryItems.Command.DeleteInventoryItem;

public record DeleteInventoryItemCommand(
    int InventoryItemID
) : IRequest<int>;
