using FluentValidation;

namespace Application.Features.FuelPurchases.Query.GetFuelPurchase;

public sealed class GetFuelPurchaseQueryValidator : AbstractValidator<GetFuelPurchaseQuery>
{
    public GetFuelPurchaseQueryValidator()
    {
        RuleFor(x => x.FuelPurchaseID)
            .GreaterThan(0)
            .WithMessage($"{nameof(GetFuelPurchaseQuery.FuelPurchaseID)} must be a positive integer");
    }
}