using System;

using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.VehicleGroups.Query.GetAllVehicleGroup;

public class GetAllVehicleGroupQueryValidator : AbstractValidator<GetAllVehicleGroupQuery>
{
    public GetAllVehicleGroupQueryValidator()
    {
        var vehicleGroupSortFields = new[] {
            "name",
            "description",
            "isactive",
            "createdat",
            "updatedat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<VehicleGroup>(vehicleGroupSortFields));
    }
}