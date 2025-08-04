using Domain.Entities.Enums;

using FluentValidation;

namespace Application.Features.Inspections.Command.CreateInspection;

/// <summary>
/// Validates <see cref="CreateInspectionCommand"/>
/// </summary>
public class CreateInspectionCommandValidator : AbstractValidator<CreateInspectionCommand>
{
    public CreateInspectionCommandValidator()
    {
        RuleFor(x => x.InspectionFormID)
            .GreaterThan(0)
            .WithMessage($"{nameof(CreateInspectionCommand.InspectionFormID)} must be greater than 0");

        RuleFor(x => x.VehicleID)
            .GreaterThan(0)
            .WithMessage($"{nameof(CreateInspectionCommand.VehicleID)} must be greater than 0");

        RuleFor(x => x.TechnicianID)
            .NotEmpty()
            .WithMessage($"{nameof(CreateInspectionCommand.TechnicianID)} is required")
            .MaximumLength(450)
            .WithMessage($"{nameof(CreateInspectionCommand.TechnicianID)} must not exceed 450 characters");

        RuleFor(x => x.InspectionStartTime)
            .NotEmpty()
            .WithMessage($"{nameof(CreateInspectionCommand.InspectionStartTime)} is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage($"{nameof(CreateInspectionCommand.InspectionStartTime)} cannot be in the future");

        RuleFor(x => x.InspectionEndTime)
            .NotEmpty()
            .WithMessage($"{nameof(CreateInspectionCommand.InspectionEndTime)} is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage($"{nameof(CreateInspectionCommand.InspectionEndTime)} cannot be in the future");

        RuleFor(x => x)
            .Must(x => x.InspectionEndTime >= x.InspectionStartTime)
            .WithMessage("InspectionEndTime must be greater than or equal to InspectionStartTime")
            .When(x => x.InspectionStartTime != default && x.InspectionEndTime != default);

        RuleFor(x => x.OdometerReading)
            .GreaterThanOrEqualTo(0)
            .WithMessage($"{nameof(CreateInspectionCommand.OdometerReading)} must be greater than or equal to 0")
            .When(x => x.OdometerReading.HasValue);

        RuleFor(x => x.VehicleCondition)
            .IsInEnum()
            .WithMessage($"{nameof(CreateInspectionCommand.VehicleCondition)} must be a valid vehicle condition");

        RuleFor(x => x.Notes)
            .MaximumLength(2000)
            .WithMessage($"{nameof(CreateInspectionCommand.Notes)} must not exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Notes));

        RuleFor(x => x.InspectionItems)
            .NotNull()
            .WithMessage($"{nameof(CreateInspectionCommand.InspectionItems)} is required")
            .NotEmpty()
            .WithMessage($"{nameof(CreateInspectionCommand.InspectionItems)} must contain at least one item");

        RuleForEach(x => x.InspectionItems)
            .SetValidator(new CreateInspectionPassFailItemCommandValidator());
    }
}