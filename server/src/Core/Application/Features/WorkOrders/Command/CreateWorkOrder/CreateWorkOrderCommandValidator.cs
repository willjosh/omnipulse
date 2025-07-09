using System;

using Application.Features.Shared;

using FluentValidation;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
    public CreateWorkOrderCommandValidator()
    {
        RuleFor(x => x.VehicleID)
            .GreaterThan(0)
            .WithMessage("Vehicle ID must be greater than 0");

        RuleFor(x => x.ServiceReminderID)
            .GreaterThan(0)
            .WithMessage("Service Reminder ID must be greater than 0");

        // AssignedToUserId validation - required
        RuleFor(x => x.AssignedToUserID)
            .NotEmpty()
            .WithMessage("Assigned To User ID is required");

        // Title validation - matches config HasMaxLength(200)
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage("Title is required")
            .MaximumLength(200)
            .WithMessage("Title cannot exceed 200 characters");

        // Description validation - matches config HasMaxLength(2000)
        RuleFor(x => x.Description)
            .MaximumLength(2000)
            .WithMessage("Description cannot exceed 2000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        // StartOdometer validation - matches check constraint >= 0
        RuleFor(x => x.StartOdometer)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Start Odometer must be greater than or equal to 0");

        // EndOdometer validation - matches check constraint >= StartOdometer
        RuleFor(x => x.EndOdometer)
            .GreaterThanOrEqualTo(x => x.StartOdometer)
            .WithMessage("End Odometer must be greater than or equal to Start Odometer")
            .When(x => x.EndOdometer.HasValue);

        // Date validation - matches check constraint ActualStartDate >= ScheduledStartDate
        RuleFor(x => x.ActualStartDate)
            .GreaterThanOrEqualTo(x => x.ScheduledStartDate)
            .WithMessage("Actual Start Date must be greater than or equal to Scheduled Start Date")
            .When(x => x.ActualStartDate.HasValue && x.ScheduledStartDate.HasValue);

        // ScheduledStartDate validation - should be in the future for new work orders
        RuleFor(x => x.ScheduledStartDate)
            .GreaterThan(DateTime.UtcNow.AddMinutes(-5)) // Allow 5 minute buffer
            .WithMessage("Scheduled Start Date should be in the future")
            .When(x => x.ScheduledStartDate.HasValue);

    }
}