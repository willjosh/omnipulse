using System;

using FluentValidation;

namespace Application.Features.Inventory.Command.UpdateInventory;

public class UpdateInventoryCommandValidator : AbstractValidator<UpdateInventoryCommand>
{
    public UpdateInventoryCommandValidator()
    {
        RuleFor(p => p.InventoryID)
             .GreaterThan(0) // Changed from NotEmpty to GreaterThan(0)
             .WithMessage("Inventory ID is required");

        RuleFor(p => p.QuantityOnHand)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Quantity on hand must be zero or greater");

        RuleFor(p => p.UnitCost)
            .GreaterThan(0)
            .WithMessage("Unit cost must be greater than zero");

        RuleFor(p => p.MinStockLevel)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Minimum stock level must be zero or greater");

        RuleFor(p => p.MaxStockLevel)
            .GreaterThanOrEqualTo(p => p.MinStockLevel)
            .WithMessage("Maximum stock level must be greater than or equal to minimum stock level");

        RuleFor(p => p.IsAdjustment)
            .NotNull()
            .WithMessage("IsAdjustment flag is required");

        RuleFor(p => p.PerformedByUserID)
            .NotEmpty()
            .WithMessage("Performed by user ID is required");
    }
}