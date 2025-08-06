using Domain.Entities.Enums;

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
            .When(IsTimeBased)
            .WithMessage("Time buffer cannot be greater than or equal to time interval when units are considered");

        // Mileage buffer validation
        RuleFor(x => x.MileageBuffer)
            .GreaterThanOrEqualTo(0)
            .When(x => x.MileageBuffer.HasValue)
            .WithMessage("Mileage buffer cannot be negative");

        // Mileage buffer cannot be greater than or equal to mileage interval
        RuleFor(x => x)
            .Must(x => !x.MileageBuffer.HasValue || !x.MileageInterval.HasValue || x.MileageBuffer < x.MileageInterval)
            .When(IsMileageBased)
            .WithMessage("Mileage buffer cannot be greater than or equal to mileage interval");

        // First service date validation (only for time-based schedules)
        RuleFor(x => x)
            .Must(x => !x.FirstServiceDate.HasValue || (x.TimeIntervalValue.HasValue && x.TimeIntervalUnit.HasValue))
            .When(IsTimeBased)
            .WithMessage("First service date requires TimeIntervalValue and TimeIntervalUnit to be set");

        // First service mileage validation (only for mileage-based schedules)
        RuleFor(x => x.FirstServiceMileage)
            .GreaterThanOrEqualTo(0)
            .When(x => x.FirstServiceMileage.HasValue)
            .WithMessage("First service mileage cannot be negative when provided");

        RuleFor(x => x)
            .Must(x => !x.FirstServiceMileage.HasValue || x.MileageInterval.HasValue)
            .When(IsMileageBased)
            .WithMessage("First service mileage requires MileageInterval to be set");

        // Additional validation for invalid combinations
        // First service date with mileage-based schedule (invalid)
        RuleFor(x => x)
            .Must(x => !x.FirstServiceDate.HasValue || !IsMileageBased(x))
            .WithMessage("First service date cannot be used with mileage-based schedules");

        // First service mileage with time-based schedule (invalid)
        RuleFor(x => x)
            .Must(x => !x.FirstServiceMileage.HasValue || !IsTimeBased(x))
            .WithMessage("First service mileage cannot be used with time-based schedules");

        RuleFor(x => x.ServiceTaskIDs)
            .NotNull().WithMessage("Service task list is required")
            .NotEmpty().WithMessage("At least one service task must be selected");

        // ENFORCE EITHER/OR: Exactly one type must be provided
        RuleFor(x => x)
            .Must(HaveExactlyOneScheduleType)
            .WithMessage("Service schedule must have either time-based OR mileage-based configuration, not both.");
    }

    private static bool HaveExactlyOneScheduleType(CreateServiceScheduleCommand command)
    {
        var hasTime = command.TimeIntervalValue.HasValue && command.TimeIntervalUnit.HasValue;
        var hasMileage = command.MileageInterval.HasValue;

        return hasTime != hasMileage; // Exactly one must be true
    }

    private static bool IsTimeBased(CreateServiceScheduleCommand command)
    {
        return command.TimeIntervalValue.HasValue && command.TimeIntervalUnit.HasValue;
    }

    private static bool IsMileageBased(CreateServiceScheduleCommand command)
    {
        return command.MileageInterval.HasValue;
    }

    private static bool IsTimeBufferValid(CreateServiceScheduleCommand cmd)
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