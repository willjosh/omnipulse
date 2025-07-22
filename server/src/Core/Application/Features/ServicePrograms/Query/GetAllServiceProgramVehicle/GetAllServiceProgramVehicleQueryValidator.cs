using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;

public class GetAllServiceProgramVehicleQueryValidator : AbstractValidator<GetAllServiceProgramVehicleQuery>
{
    private static readonly string[] ValidSortFields = [
        "vehiclename",
        "addedat",
    ];

    public GetAllServiceProgramVehicleQueryValidator()
    {
        RuleFor(x => x.ServiceProgramID)
            .GreaterThan(0)
            .WithMessage($"{nameof(GetAllServiceProgramVehicleQuery.ServiceProgramID)} must be greater than 0.");

        RuleFor(x => x.Parameters)
            .NotNull()
            .WithMessage($"{nameof(GetAllServiceProgramVehicleQuery.Parameters)} cannot be null.")
            .SetValidator(new PaginationValidator<Vehicle>(ValidSortFields));
    }
}