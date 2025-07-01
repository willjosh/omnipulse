using MediatR;

namespace Application.Features.InventoryItems.Query.GetInventoryItem;

public record GetInventoryItemQuery(int InventoryItemID) : IRequest<GetInventoryItemDTO> {}