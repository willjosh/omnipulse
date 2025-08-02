using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.Inspections.Query.GetAllInspection;

/// <summary>
/// Validates <see cref="GetAllInspectionQuery"/>
/// </summary>
public class GetAllInspectionQueryValidator : AbstractValidator<GetAllInspectionQuery>
{
    public GetAllInspectionQueryValidator()
    {
        string[] inspectionSortFields = [
            "id", "inspectionformid", "vehicleid", "technicianid",
            "inspectionstarttime", "inspectionendtime", "odometerreading",
            "vehiclecondition", "snapshotformtitle", "createdat", "updatedat"
        ];

        RuleFor(x => x.Parameters)
            .NotNull()
            .WithMessage($"{nameof(GetAllInspectionQuery.Parameters)} is required")
            .SetValidator(new PaginationValidator<Inspection>(inspectionSortFields));
    }
}