using FluentValidation;

namespace Application.Features.FuelPurchases.Command.DeleteFuelPurchase;

public sealed class DeleteFuelPurchaseCommandValidator : AbstractValidator<DeleteFuelPurchaseCommand>
{
    public DeleteFuelPurchaseCommandValidator()
    {
        RuleFor(x => x.FuelPurchaseID)
            .GreaterThan(0)
            .WithMessage($"{nameof(DeleteFuelPurchaseCommand.FuelPurchaseID)} must be a positive integer");
    }
}