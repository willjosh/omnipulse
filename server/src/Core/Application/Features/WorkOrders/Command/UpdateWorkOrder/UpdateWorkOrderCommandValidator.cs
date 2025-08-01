using System;

using FluentValidation;

namespace Application.Features.WorkOrders.Command.UpdateWorkOrder;

public class UpdateWorkOrderCommandValidator : AbstractValidator<UpdateWorkOrderCommand>
{

    public UpdateWorkOrderCommandValidator()
    {
        RuleFor(x => x.WorkOrderID)
            .GreaterThan(0)
            .WithMessage("Work Order ID must be greater than 0");

        RuleFor(x => x.VehicleID)
             .GreaterThan(0)
             .WithMessage("Vehicle ID must be greater than 0");

        RuleFor(x => x.AssignedToUserID)
            .NotEmpty()
            .WithMessage("Assigned To User ID is required");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage("Description cannot exceed 2000 characters");

        RuleFor(x => x.StartOdometer)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Start odometer must be greater than or equal to 0");

        RuleFor(x => x.EndOdometer)
            .GreaterThanOrEqualTo(x => x.StartOdometer)
            .When(x => x.EndOdometer.HasValue)
            .WithMessage("End odometer must be greater than or equal to start odometer");

        RuleFor(x => x.ScheduledStartDate)
            .GreaterThanOrEqualTo(DateTime.UtcNow.AddMinutes(-5))
            .When(x => x.ScheduledStartDate.HasValue)
            .WithMessage("Scheduled Start Date cannot be in the past");

        RuleFor(x => x.ActualStartDate)
            .GreaterThanOrEqualTo(x => x.ScheduledStartDate)
            .When(x => x.ActualStartDate.HasValue && x.ScheduledStartDate.HasValue)
            .WithMessage("Actual Start Date must be greater than or equal to Scheduled Start Date");

        RuleFor(x => x.ScheduledCompletionDate)
            .GreaterThan(x => x.ScheduledStartDate)
            .When(x => x.ScheduledCompletionDate.HasValue && x.ScheduledStartDate.HasValue)
            .WithMessage("Scheduled Completion Date must be after Scheduled Start Date");

        RuleFor(x => x.ActualCompletionDate)
            .GreaterThan(x => x.ActualStartDate)
            .When(x => x.ActualCompletionDate.HasValue && x.ActualStartDate.HasValue)
            .WithMessage("Actual Completion Date must be after Actual Start Date");

        RuleFor(x => x.ActualCompletionDate)
            .GreaterThanOrEqualTo(x => x.ScheduledCompletionDate)
            .When(x => x.ActualCompletionDate.HasValue && x.ScheduledCompletionDate.HasValue)
            .WithMessage("Actual Completion Date must be greater than or equal to Scheduled Completion Date");
    }
}