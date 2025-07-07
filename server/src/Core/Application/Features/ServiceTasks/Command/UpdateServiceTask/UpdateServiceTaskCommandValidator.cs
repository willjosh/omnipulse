using Domain.Entities.Enums;

using FluentValidation;

namespace Application.Features.ServiceTasks.Command.UpdateServiceTask;

/// <summary>
/// Validates <see cref="UpdateServiceTaskCommand"/> using <see href="https://docs.fluentvalidation.net/en/latest/">FluentValidation</see> rules.
/// </summary>
public class UpdateServiceTaskCommandValidator : AbstractValidator<UpdateServiceTaskCommand>
{
    public UpdateServiceTaskCommandValidator()
    {
        RuleFor(p => p.ServiceTaskID)
            .NotEmpty()
            .WithMessage("Service Task ID is required")
            .GreaterThan(0)
            .WithMessage("Service Task ID must be a positive number");

        RuleFor(p => p.Name)
            .NotEmpty()
            .WithMessage("Service task name is required")
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Service task name cannot be whitespace only")
            .MinimumLength(1)
            .MaximumLength(100)
            .WithMessage("Service task name must be between 1 and 100 characters");

        RuleFor(p => p.Description)
            .MaximumLength(500)
            .WithMessage("Service task description cannot exceed 500 characters");

        RuleFor(p => p.EstimatedLabourHours)
            .GreaterThan(0)
            .WithMessage("Estimated labour hours must be greater than 0")
            .LessThanOrEqualTo(1000)
            .WithMessage("Estimated labour hours cannot exceed 1000 hours");

        RuleFor(p => p.EstimatedCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Estimated cost cannot be negative")
            .LessThanOrEqualTo(999999.99m)
            .WithMessage("Estimated cost cannot exceed $999,999.99");

        RuleFor(p => p.Category)
            .IsInEnum()
            .WithMessage("Invalid service task category selected");
    }
}