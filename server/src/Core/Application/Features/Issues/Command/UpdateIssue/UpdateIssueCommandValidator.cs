using Domain.Entities.Enums;

using FluentValidation;

namespace Application.Features.Issues.Command.UpdateIssue;

public class UpdateIssueCommandValidator : AbstractValidator<UpdateIssueCommand>
{
    public UpdateIssueCommandValidator()
    {
        RuleFor(p => p.IssueID)
            .NotEmpty()
            .WithMessage("Issue ID is required");

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

        When(p => p.Status == IssueStatusEnum.RESOLVED, () =>
        {
            RuleFor(p => p.ResolutionNotes)
                .NotEmpty()
                .WithMessage("Resolution notes are required when the issue is resolved");

            RuleFor(p => p.ResolvedDate)
                .NotEmpty()
                .WithMessage("Resolved date is required when the issue is resolved");

            RuleFor(p => p.ResolvedByUserID)
                .NotEmpty()
                .WithMessage("Resolved by user ID is required when the issue is resolved");
        });
    }
}