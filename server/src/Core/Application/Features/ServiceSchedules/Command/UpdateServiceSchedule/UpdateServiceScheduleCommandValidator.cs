using Domain.Entities.Enums;

using FluentValidation;

namespace Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;

public sealed class UpdateServiceScheduleCommandValidator : AbstractValidator<UpdateServiceScheduleCommand>
{
    public UpdateServiceScheduleCommandValidator()
    {
        RuleFor(x => x.ServiceScheduleID)
            .NotEmpty().WithMessage("Service schedule ID is required")
            .GreaterThan(0).WithMessage("Service schedule ID must be a positive number");

        RuleFor(x => x.ServiceProgramID)
            .NotEmpty().WithMessage("Service program is required")
            .GreaterThan(0).WithMessage("Service program ID must be a positive number");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Schedule name is required")
            .Must(x => !string.IsNullOrWhiteSpace(x)).WithMessage("Schedule name cannot be whitespace only")
            .MinimumLength(1).MaximumLength(200)
            .WithMessage("Schedule name must be between 1 and 200 characters");

        // Time interval validation
        RuleFor(x => x.TimeIntervalValue)
            .GreaterThan(0)
            .When(x => x.TimeIntervalValue.HasValue)
            .WithMessage("Time interval value must be positive when provided");

        RuleFor(x => x.TimeIntervalUnit)
            .IsInEnum()
            .When(x => x.TimeIntervalUnit.HasValue)
            .WithMessage("Time interval unit must be a valid enum value when provided");

        RuleFor(x => x)
            .Must(x => (x.TimeIntervalValue.HasValue && x.TimeIntervalUnit.HasValue) || (!x.TimeIntervalValue.HasValue && !x.TimeIntervalUnit.HasValue))
            .WithMessage("Time interval value and unit must both be provided together or both be null");

        // Mileage interval validation
        RuleFor(x => x.MileageInterval)
            .GreaterThan(0)
            .When(x => x.MileageInterval.HasValue)
            .WithMessage("Mileage interval must be positive when provided");

        // At least one interval must be provided
        RuleFor(x => x)
            .Must(x => (x.TimeIntervalValue.HasValue && x.TimeIntervalUnit.HasValue) || x.MileageInterval.HasValue)
            .WithMessage("At least one recurrence option must be provided: time-based (TimeIntervalValue & TimeIntervalUnit) or mileage-based (MileageInterval)");

        // Time buffer validation
        RuleFor(x => x.TimeBufferValue)
            .GreaterThanOrEqualTo(0)
            .When(x => x.TimeBufferValue.HasValue)
            .WithMessage("Time buffer value cannot be negative");

        RuleFor(x => x.TimeBufferUnit)
            .IsInEnum()
            .When(x => x.TimeBufferUnit.HasValue)
            .WithMessage("Time buffer unit must be a valid enum value when provided");

        RuleFor(x => x)
            .Must(x => (x.TimeBufferValue.HasValue && x.TimeBufferUnit.HasValue) || (!x.TimeBufferValue.HasValue && !x.TimeBufferUnit.HasValue))
            .WithMessage("Time buffer value and unit must both be provided together or both be null");

        // Time buffer cannot be greater than or equal to time interval
        RuleFor(x => x)
            .Must(IsTimeBufferValid)
            .WithMessage("Time buffer cannot be greater than or equal to time interval when units are considered");

        // Mileage buffer validation
        RuleFor(x => x.MileageBuffer)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MileageBuffer.HasValue)
            .WithMessage("Mileage buffer cannot be negative");

        // Mileage buffer cannot be greater than or equal to mileage interval
        RuleFor(x => x)
            .Must(x => !x.MileageBuffer.HasValue || !x.MileageInterval.HasValue || x.MileageBuffer < x.MileageInterval)
            .WithMessage("Mileage buffer cannot be greater than or equal to mileage interval");

        // First service date validation
        RuleFor(x => x.FirstServiceDate)
            .GreaterThan(DateTime.Today.AddDays(-1)) // Allow today or future dates
            .When(x => x.FirstServiceDate.HasValue)
            .WithMessage("First service date must be today or in the future");

        RuleFor(x => x)
            .Must(x => !x.FirstServiceDate.HasValue || (x.TimeIntervalValue.HasValue && x.TimeIntervalUnit.HasValue))
            .WithMessage("First service date requires TimeIntervalValue and TimeIntervalUnit to be set");

        // First service mileage validation
        RuleFor(x => x.FirstServiceMileage)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FirstServiceMileage.HasValue)
            .WithMessage("First service mileage cannot be negative when provided");

        RuleFor(x => x)
            .Must(x => !x.FirstServiceMileage.HasValue || x.MileageInterval.HasValue)
            .WithMessage("First service mileage requires MileageInterval to be set");

        RuleFor(x => x.ServiceTaskIDs)
            .NotNull().WithMessage("Service task list is required")
            .NotEmpty().WithMessage("At least one service task must be selected");
    }

    private static bool IsTimeBufferValid(UpdateServiceScheduleCommand cmd)
    {
        if (!(IsSupportedTimeUnit(cmd.TimeBufferUnit) && IsSupportedTimeUnit(cmd.TimeIntervalUnit))) return true;

        if (cmd.TimeBufferValue.HasValue && cmd.TimeBufferUnit.HasValue && cmd.TimeIntervalValue.HasValue && cmd.TimeIntervalUnit.HasValue)
        {
            var bufferHours = ConvertToHours(cmd.TimeBufferValue.Value, cmd.TimeBufferUnit.Value);
            var intervalHours = ConvertToHours(cmd.TimeIntervalValue.Value, cmd.TimeIntervalUnit.Value);
            return bufferHours < intervalHours;
        }
        return true;
    }

    private static bool IsSupportedTimeUnit(TimeUnitEnum? unit) => unit is TimeUnitEnum.Hours or TimeUnitEnum.Days or TimeUnitEnum.Weeks;

    private static int ConvertToHours(int value, TimeUnitEnum unit) => unit switch
    {
        TimeUnitEnum.Hours => value,
        TimeUnitEnum.Days => value * 24,
        TimeUnitEnum.Weeks => value * 24 * 7,
        _ => throw new ArgumentException($"Unsupported time unit: {unit}")
    };
}