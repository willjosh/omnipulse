using System;
using Application.Features.Shared;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.Vehicles.Query.GetAllVehicle;

public class GetAllVehicleQueryValidator : AbstractValidator<GetAllVehicleQuery>
{
    public GetAllVehicleQueryValidator()
    {
        var vehicleSortFields = new[] {
            "name",
            "make",
            "model",
            "year",
            "status",
            "purchaseprice",
            "mileage",
            "enginehours",
            "createdat",
            "purchasedate",
            "fuelcapacity",
            "location"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<Vehicle>(vehicleSortFields));
    }
}
