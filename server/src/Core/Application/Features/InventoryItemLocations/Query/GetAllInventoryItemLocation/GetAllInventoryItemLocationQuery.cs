using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

public record GetAllInventoryItemLocationQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllInventoryItemLocationDTO>>;