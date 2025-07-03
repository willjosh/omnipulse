using System;

using Application.Contracts.Persistence;
using Application.Features.FuelLogging.Command.CreateFuelPurchase;

using FluentValidation;

namespace Application.Features.FuelLogging.Command.CreateFuelPurchase;

public class CreateFuelPurchaseCommandValidator : AbstractValidator<CreateFuelPurchaseCommand>
{
    public CreateFuelPurchaseCommandValidator()
    {
        RuleFor(p => p.VehicleId)
            .NotEmpty()
            .WithMessage("Vehicle id is required");

        RuleFor(p => p.PurchasedByUserId)
            .NotEmpty()
            .WithMessage("User purchase id is required");

        RuleFor(p => p.PurchaseDate)
            .NotEmpty()
            .WithMessage("Purchase date is required");

        RuleFor(p => p.OdometerReading)
            .NotEmpty()
            .WithMessage("Odometer reading is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("OdometerReading cannot be less than 0 after being fuelled");

        RuleFor(p => p.Volume)
            .NotEmpty()
            .WithMessage("Fuel volume is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Fuel volume cannot be less than 0");

        RuleFor(p => p.PricePerUnit)
            .NotEmpty()
            .WithMessage("Price per unit is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("Price per unit cannot be less than 0");

        RuleFor(p => p.TotalCost)
            .NotEmpty()
            .WithMessage("Total cost is required")
            .GreaterThanOrEqualTo(0)
            .WithMessage("TotalCost cannot be less than 0")
            .Must((command, TotalCost) => TotalCost >= command.PricePerUnit)
            .WithMessage("Total Cost cannot be less than price pre unit");
        ;

        RuleFor(p => p.FuelStation)
            .NotEmpty()
            .WithMessage("Name of fuel station is required");

        RuleFor(p => p.ReceiptNumber)
            .NotEmpty()
            .WithMessage("Receipt number is required");
    }
}