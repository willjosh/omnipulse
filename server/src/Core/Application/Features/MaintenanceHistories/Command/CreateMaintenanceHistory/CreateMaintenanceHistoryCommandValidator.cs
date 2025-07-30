using FluentValidation;

namespace Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;

public class CreateMaintenanceHistoryCommandValidator : AbstractValidator<CreateMaintenanceHistoryCommand>
{
    public CreateMaintenanceHistoryCommandValidator()
    {
        RuleFor(p => p.WorkOrderID)
            .NotEmpty()
            .WithMessage("Work Order ID is required");

        RuleFor(p => p.ServiceDate)
            .NotEmpty()
            .WithMessage("Service date is required")
            .LessThanOrEqualTo(_ => DateTime.UtcNow)
            .WithMessage("Service date cannot be in the future");

        RuleFor(p => p.MileageAtService)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Mileage at service must be non-negative");

        RuleFor(p => p.Description)
            .MaximumLength(1000)
            .WithMessage("Description must be less than 1000 characters");

        RuleFor(p => p.Cost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Cost must be non-negative");

        RuleFor(p => p.LabourHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Labour hours must be non-negative");

        RuleFor(p => p.Notes)
            .MaximumLength(1000)
            .WithMessage("Notes must be less than 1000 characters");
    }
}