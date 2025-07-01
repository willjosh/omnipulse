using System;

using FluentValidation;

namespace Application.Features.InventoryItems.Command.UpdateInventoryItem;

public class UpdateInventoryItemCommandValidator : AbstractValidator<UpdateInventoryItemCommand>
{
    public UpdateInventoryItemCommandValidator()
    {
        RuleFor(p => p.InventoryItemID)
            .NotEmpty()
            .WithMessage("Inventory Item ID is required")
            .GreaterThan(0)
            .WithMessage("Inventory Item ID must be a positive number");

        RuleFor(x => x.ItemNumber)
            .NotEmpty().WithMessage("Item number is required")
            .MaximumLength(250).WithMessage("Item number must not exceed 250 characters");

        RuleFor(x => x.ItemName)
            .NotEmpty().WithMessage("Item name is required")
            .MaximumLength(250).WithMessage("Item name must not exceed 250 characters");

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Category)
            .IsInEnum().WithMessage("Valid category is required")
            .When(x => x.Category.HasValue);

        RuleFor(x => x.Manufacturer)
            .MaximumLength(100).WithMessage("Manufacturer must not exceed 100 characters");

        RuleFor(x => x.ManufacturerPartNumber)
            .MaximumLength(100).WithMessage("Manufacturer part number must not exceed 100 characters");

        RuleFor(x => x.UniversalProductCode)
            .Length(12).WithMessage("Universal Product Code must be exactly 12 characters")
            .When(x => !string.IsNullOrEmpty(x.UniversalProductCode));

        RuleFor(x => x.Supplier)
            .MaximumLength(100).WithMessage("Supplier must not exceed 100 characters");

        RuleFor(x => x.UnitCost)
            .GreaterThanOrEqualTo(0).WithMessage("Unit cost must be greater than or equal to 0")
            .When(x => x.UnitCost.HasValue);

        RuleFor(x => x.UnitCostMeasurementUnit)
            .IsInEnum().WithMessage("Valid unit cost measurement unit is required")
            .When(x => x.UnitCostMeasurementUnit.HasValue);

        RuleFor(x => x.WeightKG)
            .GreaterThanOrEqualTo(0).WithMessage("Weight must be greater than or equal to 0")
            .When(x => x.WeightKG.HasValue);
    }
}