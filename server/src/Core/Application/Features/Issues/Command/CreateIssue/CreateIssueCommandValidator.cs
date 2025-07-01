using System;
using FluentValidation;

namespace Application.Features.Issues.Command.CreateIssue;

public class CreateIssueCommandValidator : AbstractValidator<CreateIssueCommand>
{
    public CreateIssueCommandValidator()
    {
        RuleFor(p => p.VehicleID)
            .NotEmpty()
            .WithMessage("Vehicle ID is required");

        RuleFor(p => p.ReportedByUserID)
            .NotEmpty()
            .WithMessage("Reported by user ID is required");

        RuleFor(p => p.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title must be less than 200 characters");

        RuleFor(p => p.Description)
            .MaximumLength(1000)
            .WithMessage("Description must be less than 1000 characters");

        RuleFor(p => p.Category)
            .NotEmpty()
            .WithMessage("Category is required")
            .IsInEnum()
            .WithMessage("Invalid category selected");

        RuleFor(p => p.PriorityLevel)
            .NotEmpty()
            .WithMessage("Priority level is required")
            .IsInEnum()
            .WithMessage("Invalid priority level selected");

        RuleFor(p => p.Status)
            .NotEmpty()
            .WithMessage("Status is required")
            .IsInEnum()
            .WithMessage("Invalid status selected");
    }
}
