using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;

public class GetAllInspectionFormItemQueryValidator : AbstractValidator<GetAllInspectionFormItemQuery>
{
    public GetAllInspectionFormItemQueryValidator()
    {
        var inspectionFormItemSortFields = new[] {
            "itemlabel",
            "itemdescription",
            "itemtypeenum",
            "isrequired",
            "createdat",
            "updatedat"
        };

        RuleFor(x => x.InspectionFormID)
            .GreaterThan(0)
            .WithMessage($"{nameof(GetAllInspectionFormItemQuery.InspectionFormID)} must be a positive integer");

        RuleFor(x => x.Parameters)
            .NotNull()
            .WithMessage($"{nameof(GetAllInspectionFormItemQuery.Parameters)} cannot be null.")
            .SetValidator(new PaginationValidator<InspectionFormItem>(inspectionFormItemSortFields));
    }
}