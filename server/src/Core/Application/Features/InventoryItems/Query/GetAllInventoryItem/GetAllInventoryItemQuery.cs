using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.InventoryItems.Query.GetAllInventoryItem;

public record GetAllInventoryItemQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllInventoryItemDTO>>;