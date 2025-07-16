using System;

using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.FuelPurchases.Query.GetAllFuelPurchases;

public class GetAllFuelPurchasesQueryValidator : AbstractValidator<GetAllFuelPurchasesQuery>
{
    public GetAllFuelPurchasesQueryValidator()
    {
        var fuelPurchasesSortFields = new[] {
            "vehicleid",
            "purchasedbyuserid",
            "purchasedate",
            "odometerreading",
            "volume",
            "priceperunit",
            "totalcost",
            "fuelstation",
            "receiptnumber"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<FuelPurchase>(fuelPurchasesSortFields));
    }
}