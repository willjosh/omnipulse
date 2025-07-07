using FluentValidation;

namespace Application.Features.ServiceSchedules.Command.CreateServiceSchedule;

public sealed class CreateServiceScheduleCommandValidator : AbstractValidator<CreateServiceScheduleCommand>
{
    public CreateServiceScheduleCommandValidator()
    {
        RuleFor(x => x.ServiceProgramID)
            .NotEmpty().WithMessage("Service program is required")
            .GreaterThan(0).WithMessage("Service program ID must be a positive number");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Schedule name is required")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Schedule name cannot be whitespace only")
            .MinimumLength(1).MaximumLength(200)
            .WithMessage("Schedule name must be between 1 and 200 characters");

        RuleFor(x => x.IntervalMileage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Interval mileage cannot be negative");

        RuleFor(x => x.IntervalDays)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Interval days cannot be negative");

        RuleFor(x => x.IntervalHours)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Interval hours cannot be negative");

        RuleFor(x => x)
            .Must(x => x.IntervalMileage > 0 || x.IntervalDays > 0 || x.IntervalHours > 0)
            .WithMessage("Either a meter interval or a time interval must be provided");

        RuleFor(x => x.BufferMileage)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Buffer mileage cannot be negative");

        RuleFor(x => x.BufferDays)
            .GreaterThanOrEqualTo(0)
            .WithMessage("Buffer days cannot be negative");
    }
}