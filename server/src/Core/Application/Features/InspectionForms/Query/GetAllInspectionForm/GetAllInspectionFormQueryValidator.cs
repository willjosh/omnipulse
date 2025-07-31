using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.InspectionForms.Query.GetAllInspectionForm;

public class GetAllInspectionFormQueryValidator : AbstractValidator<GetAllInspectionFormQuery>
{
    public GetAllInspectionFormQueryValidator()
    {
        var inspectionFormSortFields = new[] {
            "title",
            "description",
            "isactive",
            "createdat",
            "updatedat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .WithMessage($"{nameof(GetAllInspectionFormQuery.Parameters)} cannot be null.")
            .SetValidator(new PaginationValidator<InspectionForm>(inspectionFormSortFields));
    }
}