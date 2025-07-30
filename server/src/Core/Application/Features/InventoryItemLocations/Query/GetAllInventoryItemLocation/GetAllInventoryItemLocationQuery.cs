using MediatR;

namespace Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

public record GetAllInventoryItemLocationQuery() : IRequest<List<GetAllInventoryItemLocationDTO>>;