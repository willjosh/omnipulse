using FluentValidation;

namespace Application.Features.VehicleGroups.Command.UpdateVehicleGroup;

public class UpdateVehicleGroupCommandValidator : AbstractValidator<UpdateVehicleGroupCommand>
{
    public UpdateVehicleGroupCommandValidator()
    {
        RuleFor(p => p.VehicleGroupId)
            .NotEmpty()
            .WithMessage("Vehicle group id is required");

        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Vehicle group name is required")
            .MinimumLength(1)
            .MaximumLength(100)
            .WithMessage("Vehicle group name must be between 1 and 100 characters");

        RuleFor(p => p.Description)
            .MaximumLength(300)
            .WithMessage("Vehicle group description must be less than 300 characters");

        RuleFor(p => p.IsActive)
            .NotNull()
            .WithMessage("Vehicle group is active is required");
    }
}