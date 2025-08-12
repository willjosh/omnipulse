using FluentValidation;

namespace Application.Features.FuelPurchases.Command.CreateFuelPurchase;

public class CreateFuelPurchaseCommandValidator : AbstractValidator<CreateFuelPurchaseCommand>
{
    public CreateFuelPurchaseCommandValidator()
    {
        RuleFor(p => p.VehicleId)
            .NotEmpty()
            .WithMessage("Vehicle ID is required")
            .GreaterThan(0)
            .WithMessage("Vehicle ID must be a positive number");

        RuleFor(p => p.PurchasedByUserId)
            .NotEmpty()
            .WithMessage("User ID is required")
            .MinimumLength(1)
            .MaximumLength(450)
            .WithMessage("User ID must be between 1 and 450 characters");

        RuleFor(p => p.PurchaseDate)
            .NotEmpty()
            .WithMessage("Purchase date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Purchase date cannot be in the future");

        RuleFor(p => p.OdometerReading)
            .NotEmpty()
            .WithMessage("Odometer reading is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Odometer reading cannot be negative");

        RuleFor(p => p.Volume)
            .NotEmpty()
            .WithMessage("Fuel volume is required")
            .GreaterThan(0)
            .WithMessage("Fuel volume must be greater than 0");

        RuleFor(p => p.PricePerUnit)
            .NotEmpty()
            .WithMessage("Price per unit is required")
            .GreaterThan(0)
            .WithMessage("Price per unit must be greater than 0");

        RuleFor(p => p.TotalCost)
            .NotEmpty()
            .WithMessage("Total cost is required")
            .GreaterThan(0)
            .WithMessage("Total cost must be greater than 0")
            .Must((command, totalCost) =>
            {
                var expectedCost = (decimal)command.Volume * command.PricePerUnit;
                var tolerance = expectedCost * 0.05m; // 5% tolerance
                var difference = Math.Abs(totalCost - expectedCost);
                return difference <= tolerance;
            })
            .WithMessage("Total cost must be approximately equal to volume * price per unit (within 5% tolerance)");

        RuleFor(p => p.FuelStation)
            .NotEmpty()
            .WithMessage("Fuel station name is required")
            .MinimumLength(1)
            .MaximumLength(200)
            .WithMessage("Fuel station name must be between 1 and 200 characters");

        RuleFor(p => p.ReceiptNumber)
            .NotEmpty()
            .WithMessage("Receipt number is required")
            .MinimumLength(1)
            .MaximumLength(100)
            .WithMessage("Receipt number must be between 1 and 100 characters");

        RuleFor(p => p.Notes)
            .MaximumLength(500)
            .WithMessage("Notes cannot exceed 500 characters");
    }
}