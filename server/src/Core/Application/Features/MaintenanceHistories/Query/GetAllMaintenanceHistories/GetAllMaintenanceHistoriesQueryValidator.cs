using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoriesQueryValidator : AbstractValidator<GetAllMaintenanceHistoriesQuery>
{
    public GetAllMaintenanceHistoriesQueryValidator()
    {
        var maintenanceHistorySortFields = new[] {
            "servicedate",
            "vehicleid",
            "workorderid",
            "servicetaskid",
            "technicianid",
            "mileageatservice",
            "cost",
            "labourhours",
            "createdat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<MaintenanceHistory>(maintenanceHistorySortFields));
    }
}