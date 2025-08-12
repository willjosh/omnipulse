using FluentValidation;

namespace Application.Features.FuelLogging.Command.UpdateFuelPurchase;

public sealed class UpdateFuelPurchaseCommandValidator : AbstractValidator<UpdateFuelPurchaseCommand>
{
    public UpdateFuelPurchaseCommandValidator()
    {
        RuleFor(p => p.FuelPurchaseId)
            .GreaterThan(0)
            .WithMessage("FuelPurchaseId must be a positive number");

        RuleFor(p => p.VehicleId)
            .GreaterThan(0)
            .WithMessage("VehicleId must be a positive number");

        RuleFor(p => p.PurchasedByUserId)
            .NotEmpty()
            .WithMessage("PurchasedByUserId is required")
            .Must(id => Guid.TryParse(id, out _))
            .WithMessage("PurchasedByUserId must be a valid GUID");

        RuleFor(p => p.PurchaseDate)
            .NotEmpty()
            .WithMessage("Purchase date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Purchase date cannot be in the future");

        RuleFor(p => p.OdometerReading)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Odometer reading cannot be negative");

        RuleFor(p => p.Volume)
            .GreaterThan(0)
            .WithMessage("Fuel volume must be greater than 0");

        RuleFor(p => p.PricePerUnit)
            .GreaterThan(0)
            .WithMessage("Price per unit must be greater than 0");

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