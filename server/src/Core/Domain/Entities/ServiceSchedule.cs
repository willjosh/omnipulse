using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a service schedule that defines the cadence at which maintenance tasks should be performed for vehicles by mapping intervals to <see cref="ServiceTask"/>s.
/// </summary>
/// <remarks>
/// A service schedule can be configured by time, mileage, or a combination of both.
/// It belongs to a single <see cref="ServiceProgram"/>.
/// 1:N <see cref="XrefServiceScheduleServiceTask"/>s.
///
/// <list type="bullet">
///   <item>Must reference exactly one ServiceProgram via <see cref="ServiceProgramID"/>.</item>
///   <item>At least one of the following recurrence options must be provided:
///     <list type="bullet">
///       <item>Time-based (<see cref="TimeIntervalValue"/> &amp; <see cref="TimeIntervalUnit"/>)</item>
///       <item>Mileage-based (<see cref="MileageInterval"/>)</item>
///     </list>
///   </item>
///   <item>Buffer values (<see cref="TimeBufferValue"/>, <see cref="MileageBuffer"/>) are optional. If provided, they cannot exceed their corresponding interval.</item>
///   <item>First service properties (<see cref="FirstServiceDate"/>, <see cref="FirstServiceMileage"/>) are optional absolute values:
///     <list type="bullet">
///       <item><see cref="FirstServiceDate"/> defines the absolute date for the first service. If provided, requires <see cref="TimeIntervalValue"/> and <see cref="TimeIntervalUnit"/> to be set.</item>
///       <item><see cref="FirstServiceMileage"/> defines the absolute mileage for the first service. If provided, requires <see cref="MileageInterval"/> to be set.</item>
///     </list>
///   </item>
///   <item>The schedule can be activated or deactivated using the <see cref="IsActive"/> flag. Inactive schedules are ignored when calculating upcoming service reminders.</item>
/// </list>
/// </remarks>
public class ServiceSchedule : BaseEntity
{
    public required int ServiceProgramID { get; set; }
    public required string Name { get; set; }
    public int? TimeIntervalValue { get; set; }
    public TimeUnitEnum? TimeIntervalUnit { get; set; }
    public int? TimeBufferValue { get; set; }
    public TimeUnitEnum? TimeBufferUnit { get; set; }
    public int? MileageInterval { get; set; } // km
    public int? MileageBuffer { get; set; } // km
    public DateTime? FirstServiceDate { get; set; } // Absolute date for first service - requires TimeIntervalValue and TimeIntervalUnit if set
    public int? FirstServiceMileage { get; set; } // Absolute mileage for first service - requires MileageInterval if set
    public required bool IsActive { get; set; } = true;

    // Navigation Properties
    public required ICollection<XrefServiceScheduleServiceTask> XrefServiceScheduleServiceTasks { get; set; } = [];
    public required ServiceProgram ServiceProgram { get; set; }
}

/// <summary>
/// Extension methods for ServiceSchedule
/// </summary>
public static class ServiceScheduleExtensions
{
    /// <summary>
    /// Determines the schedule type based on configured intervals.
    /// </summary>
    /// <param name="schedule">The service schedule.</param>
    /// <returns>The determined schedule type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when neither time nor mileage is configured.</exception>
    public static ServiceScheduleTypeEnum GetScheduleType(this ServiceSchedule schedule)
    {
        if (schedule.TimeIntervalValue.HasValue && schedule.TimeIntervalUnit.HasValue)
            return ServiceScheduleTypeEnum.TIME;
        if (schedule.MileageInterval.HasValue)
            return ServiceScheduleTypeEnum.MILEAGE;

        throw new InvalidOperationException("Service schedule must have either time or mileage configured");
    }

    /// <summary>
    /// Checks if this schedule is time-based.
    /// </summary>
    /// <param name="schedule">The service schedule.</param>
    /// <returns>True if the schedule is time-based.</returns>
    public static bool IsTimeBased(this ServiceSchedule schedule)
    {
        return schedule.TimeIntervalValue.HasValue && schedule.TimeIntervalUnit.HasValue;
    }

    /// <summary>
    /// Checks if this schedule is mileage-based.
    /// </summary>
    /// <param name="schedule">The service schedule.</param>
    /// <returns>True if the schedule is mileage-based.</returns>
    public static bool IsMileageBased(this ServiceSchedule schedule)
    {
        return schedule.MileageInterval.HasValue;
    }

    /// <summary>
    /// Validates that the schedule has exactly one type configured.
    /// </summary>
    /// <param name="schedule">The service schedule.</param>
    /// <returns>True if the schedule has exactly one type configured.</returns>
    public static bool HasExactlyOneScheduleType(this ServiceSchedule schedule)
    {
        return schedule.IsTimeBased() != schedule.IsMileageBased(); // Exactly one must be true
    }

    /// <summary>
    /// Checks if the schedule is time-based only.
    /// </summary>
    /// <param name="schedule">The service schedule.</param>
    /// <returns>True if the schedule is time-based only.</returns>
    public static bool IsTimeBasedOnly(this ServiceSchedule schedule)
    {
        return schedule.IsTimeBased() && !schedule.IsMileageBased();
    }

    /// <summary>
    /// Checks if the schedule is mileage-based only.
    /// </summary>
    /// <param name="schedule">The service schedule.</param>
    /// <returns>True if the schedule is mileage-based only.</returns>
    public static bool IsMileageBasedOnly(this ServiceSchedule schedule)
    {
        return !schedule.IsTimeBased() && schedule.IsMileageBased();
    }
}