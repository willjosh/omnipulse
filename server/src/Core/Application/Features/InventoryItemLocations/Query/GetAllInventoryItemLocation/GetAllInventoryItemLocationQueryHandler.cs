using Application.Models;
using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

public class GetAllInventoryItemLocationQueryHandler : IRequestHandler<GetAllInventoryItemLocationQuery, PagedResult<GetAllInventoryItemLocationDTO>>
{
    public Task<PagedResult<GetAllInventoryItemLocationDTO>> Handle(GetAllInventoryItemLocationQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}