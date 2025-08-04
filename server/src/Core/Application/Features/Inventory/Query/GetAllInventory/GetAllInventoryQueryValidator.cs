using Application.Features.Shared;

using FluentValidation;

namespace Application.Features.Inventory.Query;

public class GetAllInventoryQueryValidator : AbstractValidator<GetAllInventoryQuery>
{
    public GetAllInventoryQueryValidator()
    {
        var inventorySortFields = new[] {
            "id",
            "quantity",
            "minstocklevel",
            "maxstocklevel",
            "location",
            "createdat",
            "updatedat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<Domain.Entities.Inventory>(inventorySortFields));
    }
}