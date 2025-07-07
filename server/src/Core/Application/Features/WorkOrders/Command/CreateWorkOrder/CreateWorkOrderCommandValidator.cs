using System;

using Application.Features.Shared;

using FluentValidation;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public class CreateWorkOrderCommandValidator : AbstractValidator<CreateWorkOrderCommand>
{
   public CreateWorkOrderCommandValidator()
   {
        // / WorkOrderNumber validation - matches config HasMaxLength(50) and IsUnique
        RuleFor(x => x.WorkOrderNumber)
            .NotEmpty()
            .WithMessage("Work Order Number is required")
            .MaximumLength(50)
            .WithMessage("Work Order Number cannot exceed 50 characters");

        RuleFor(x => x.VehicleId)
            .GreaterThan(0)
            .WithMessage("Vehicle ID must be greater than 0");

        RuleFor(x => x.ServiceReminderId)
            .GreaterThan(0)
            .WithMessage("Service Reminder ID must be greater than 0");

        // AssignedToUserId validation - required
        RuleFor(x => x.AssignedToUserId)
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

        // EstimatedCost validation - matches check constraint >= 0 and precision(10,2)
        RuleFor(x => x.EstimatedCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Estimated Cost must be greater than or equal to 0")
            .Must(CustomValidation.HaveValidPrecision)
            .WithMessage("Estimated Cost must have maximum 2 decimal places and 10 total digits")
            .When(x => x.EstimatedCost.HasValue);
        
        RuleFor(x => x.ActualCost)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Actual Cost must be greater than or equal to 0")
            .Must(CustomValidation.HaveValidPrecision)
            .WithMessage("Actual Cost must have maximum 2 decimal places and 10 total digits")
            .When(x => x.ActualCost.HasValue);

        // EstimatedHours validation - must be positive
        RuleFor(x => x.EstimatedHours)
            .GreaterThan(0)
            .WithMessage("Estimated Hours must be greater than 0")
            .When(x => x.EstimatedHours.HasValue);

        RuleFor(x => x.ActualHours)
            .GreaterThan(0)
            .WithMessage("Actual Hours must be greater than 0")
            .When(x => x.ActualHours.HasValue);

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