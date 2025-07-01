using System;

using Domain.Entities.Enums;

using FluentValidation;

namespace Application.Features.Vehicles.Command.CreateVehicle;

public class CreateVehicleCommandValidator : AbstractValidator<CreateVehicleCommand>
{
    public CreateVehicleCommandValidator()
    {
        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Vehicle name is required")
            .MinimumLength(1)
            .MaximumLength(100)
            .WithMessage("Vehicle name must be between 1 and 100 characters");

        RuleFor(p => p.Make)
            .NotEmpty()
            .WithMessage("Vehicle make is required")
            .MinimumLength(1)
            .MaximumLength(30)
            .WithMessage("Vehicle make must be between 1 and 30 characters");

        RuleFor(p => p.Model)
            .NotEmpty()
            .WithMessage("Vehicle model is required")
            .MinimumLength(1)
            .MaximumLength(50)
            .WithMessage("Vehicle model must be between 1 and 50 characters");

        RuleFor(p => p.Year)
            .NotEmpty()
            .WithMessage("Vehicle year is required")
            .GreaterThan(1885)
            .LessThanOrEqualTo(2100)
            .WithMessage("Vehicle year must be between 1886 and 2100");

        RuleFor(p => p.VIN)
            .NotEmpty()
            .WithMessage("VIN is required")
            .Length(17)
            .WithMessage("VIN must be exactly 17 characters");

        RuleFor(p => p.LicensePlate)
            .NotEmpty()
            .WithMessage("License plate is required")
            .MinimumLength(1)
            .MaximumLength(20)
            .WithMessage("License plate must be between 1 and 20 characters");

        RuleFor(p => p.LicensePlateExpirationDate)
            .NotEmpty()
            .WithMessage("License plate expiration date is required")
            .Must((command, expirationDate) => expirationDate > command.PurchaseDate)
            .WithMessage("License plate expiration date must be after purchase date");

        RuleFor(p => p.VehicleType)
            .NotEmpty()
            .WithMessage("Vehicle type is required")
            .IsInEnum()
            .WithMessage("Invalid vehicle type selected");

        RuleFor(p => p.VehicleGroupID)
            .NotEmpty()
            .WithMessage("Vehicle group is required")
            .GreaterThan(0)
            .WithMessage("Vehicle group ID must be a positive number");

        RuleFor(p => p.Trim)
            .NotEmpty()
            .WithMessage("Vehicle trim is required")
            .MinimumLength(1)
            .MaximumLength(50)
            .WithMessage("Vehicle trim must be between 1 and 50 characters");

        RuleFor(p => p.Mileage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Mileage cannot be negative");

        RuleFor(p => p.EngineHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Engine hours cannot be negative");

        RuleFor(p => p.FuelCapacity)
            .GreaterThan(0)
            .WithMessage("Fuel capacity must be greater than 0");

        // Fuel Type validation
        RuleFor(p => p.FuelType)
            .NotEmpty()
            .WithMessage("Fuel type is required")
            .IsInEnum()
            .WithMessage("Invalid fuel type selected");

        RuleFor(p => p.PurchaseDate)
            .NotEmpty()
            .WithMessage("Purchase date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Purchase date cannot be in the future");

        RuleFor(p => p.PurchasePrice)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Purchase price cannot be negative");

        RuleFor(p => p.Status)
            .NotEmpty()
            .WithMessage("Vehicle status is required")
            .IsInEnum()
            .WithMessage("Invalid vehicle status selected");

        RuleFor(p => p.Location)
            .NotEmpty()
            .WithMessage("Vehicle location is required")
            .MinimumLength(1)
            .MaximumLength(100)
            .WithMessage("Vehicle location must be between 1 and 100 characters");

    }
}