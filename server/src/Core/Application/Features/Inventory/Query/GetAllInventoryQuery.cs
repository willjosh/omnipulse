using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.Inventory.Query;

public record GetAllInventoryQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllInventoryDTO>> { }