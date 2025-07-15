using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

public class GetAllInventoryItemLocationQueryValidator : AbstractValidator<GetAllInventoryItemLocationQuery>
{
    public GetAllInventoryItemLocationQueryValidator()
    {
        var inventoryItemLocationSortFields = new[] {
            "locationname",
            "address",
            "longitude",
            "latitude",
            "capacity",
            "isactive",
            "createdat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<InventoryItemLocation>(inventoryItemLocationSortFields));
    }
}