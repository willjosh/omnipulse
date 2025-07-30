using FluentValidation;

namespace Application.Features.InventoryItemLocations.Command.CreateInventoryItemLocation;

public class CreateInventoryItemLocationCommandValidator : AbstractValidator<CreateInventoryItemLocationCommand>
{
    public CreateInventoryItemLocationCommandValidator()
    {
        RuleFor(p => p.LocationName)
            .NotEmpty()
            .WithMessage("Location name is required")
            .MinimumLength(1)
            .MaximumLength(100)
            .WithMessage("Location name must be between 1 and 100 characters");

        RuleFor(p => p.Address)
            .NotEmpty()
            .WithMessage("Address is required")
            .MinimumLength(1)
            .MaximumLength(255)
            .WithMessage("Address must be between 1 and 255 characters");

        RuleFor(p => p.Longitude)
            .NotEmpty()
            .WithMessage("Longitude is required")
            .GreaterThanOrEqualTo(-180)
            .LessThanOrEqualTo(180)
            .WithMessage("Longitude must be between -180 and 180");

        RuleFor(p => p.Latitude)
            .NotEmpty()
            .WithMessage("Latitude is required")
            .GreaterThanOrEqualTo(-90)
            .LessThanOrEqualTo(90)
            .WithMessage("Latitude must be between -90 and 90");

        RuleFor(p => p.Capacity)
            .NotEmpty()
            .WithMessage("Capacity is required")
            .GreaterThan(0)
            .WithMessage("Capacity must be greater than 0");
    }
}