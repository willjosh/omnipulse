using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.InventoryItems.Query.GetAllInventoryItem;

public class GetAllInventoryItemQueryValidator : AbstractValidator<GetAllInventoryItemQuery>
{
    public GetAllInventoryItemQueryValidator()
    {
        var inventoryItemSortFields = new[] {
            "itemnumber",
            "itemname",
            "description",
            "category",
            "manufacturer",
            "manufacturerpartnumber",
            "universalproductcode",
            "supplier",
            "weightkg",
            "unitcost",
            "unitcostmeasurementunit",
            "createdat",
            "updatedat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<InventoryItem>(inventoryItemSortFields));
    }
}