using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a service schedule that defines the cadence at which maintenance tasks should be performed for vehicles by mapping intervals to <see cref="ServiceTask"/>s.
/// </summary>
/// <remarks>
/// A service schedule can be configured by time OR mileage, but not both.
/// It belongs to a single <see cref="ServiceProgram"/>.
/// 1:N <see cref="XrefServiceScheduleServiceTask"/>s.
///
/// <list type="bullet">
///   <item>Must reference exactly one ServiceProgram via <see cref="ServiceProgramID"/>.</item>
///   <item>Exactly one of the following recurrence options must be provided:
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
///   <item>Schedules are considered active unless <see cref="IsSoftDeleted"/> is true. Soft-deleted schedules are excluded from queries and generation.</item>
///   <item>Soft deletion is supported via <see cref="IsSoftDeleted"/>. Soft-deleted schedules are excluded from queries and generation.</item>
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
    public bool IsSoftDeleted { get; set; } = false;

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
    /// <exception cref="InvalidOperationException">Thrown when neither time nor mileage is configured, or when both are configured (violates XOR constraint).</exception>
    public static ServiceScheduleTypeEnum GetScheduleType(this ServiceSchedule schedule)
    {
        if (schedule.IsTimeBased() && !schedule.IsMileageBased())
            return ServiceScheduleTypeEnum.TIME;
        if (!schedule.IsTimeBased() && schedule.IsMileageBased())
            return ServiceScheduleTypeEnum.MILEAGE;

        throw new InvalidOperationException("Service schedule must have exactly one schedule type configured (either time OR mileage, not both or neither)");
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

    /// <summary>
    /// Calculates service reminder status based on schedule configuration and current state.
    /// </summary>
    /// <param name="schedule">The service schedule.</param>
    /// <param name="dueDate">The due date for the reminder.</param>
    /// <param name="dueMileage">The due mileage for the reminder.</param>
    /// <param name="currentDate">The current date.</param>
    /// <param name="currentMileage">The current vehicle mileage.</param>
    /// <returns>The calculated status.</returns>
    public static ServiceReminderStatusEnum CalculateReminderStatus(this ServiceSchedule schedule, DateTime? dueDate, double? dueMileage, DateTime currentDate, double currentMileage)
    {
        // Overdue checks
        bool isOverdueByTime = dueDate.HasValue && currentDate >= dueDate.Value;
        bool isOverdueByMileage = dueMileage.HasValue && currentMileage >= dueMileage.Value;

        if (isOverdueByTime || isOverdueByMileage) return ServiceReminderStatusEnum.OVERDUE;

        // Due soon checks
        bool isDueSoonByTime = dueDate.HasValue &&
                               schedule.TimeBufferValue.HasValue &&
                               schedule.TimeBufferUnit.HasValue &&
                               currentDate >= dueDate.Value.AddDays(-ConvertToDays(schedule.TimeBufferValue.Value, schedule.TimeBufferUnit.Value)) &&
                               currentDate < dueDate.Value;

        bool isDueSoonByMileage = dueMileage.HasValue &&
                                  schedule.MileageBuffer.HasValue &&
                                  currentMileage >= (dueMileage.Value - schedule.MileageBuffer.Value) &&
                                  currentMileage < dueMileage.Value;

        if (isDueSoonByTime || isDueSoonByMileage) return ServiceReminderStatusEnum.DUE_SOON;

        // Otherwise, it's just upcoming
        return ServiceReminderStatusEnum.UPCOMING;
    }

    /// <summary>
    /// Calculates service reminder status for time-based schedules.
    /// </summary>
    /// <param name="schedule">This service schedule.</param>
    /// <param name="dueDate">The due date for the reminder.</param>
    /// <param name="currentDate">The current date (UTC).</param>
    /// <returns>The calculated status.</returns>
    public static ServiceReminderStatusEnum CalculateTimeReminderStatus(this ServiceSchedule schedule, DateTime dueDate, DateTime currentDate)
    {
        // Overdue
        if (currentDate >= dueDate) return ServiceReminderStatusEnum.OVERDUE;

        // Due soon
        if (schedule.TimeBufferValue.HasValue && schedule.TimeBufferUnit.HasValue)
        {
            var bufferDays = ConvertToDays(schedule.TimeBufferValue.Value, schedule.TimeBufferUnit.Value);
            if (currentDate >= dueDate.AddDays(-bufferDays) &&
                currentDate < dueDate)
            {
                return ServiceReminderStatusEnum.DUE_SOON;
            }
        }

        // Otherwise, upcoming
        return ServiceReminderStatusEnum.UPCOMING;
    }

    /// <summary>
    /// Calculates service reminder status for mileage-based schedules
    /// </summary>
    /// <param name="schedule">This service schedule.</param>
    /// <param name="dueMileage">The due mileage for the reminder.</param>
    /// <param name="currentMileage">The current vehicle mileage.</param>
    /// <returns>The calculated status.</returns>
    public static ServiceReminderStatusEnum CalculateMileageReminderStatus(this ServiceSchedule schedule, double dueMileage, double currentMileage)
    {
        // Overdue
        if (currentMileage >= dueMileage) return ServiceReminderStatusEnum.OVERDUE;

        // Due soon
        if (schedule.MileageBuffer.HasValue &&
            currentMileage >= (dueMileage - schedule.MileageBuffer.Value) &&
            currentMileage < dueMileage)
        {
            return ServiceReminderStatusEnum.DUE_SOON;
        }

        // Otherwise, upcoming
        return ServiceReminderStatusEnum.UPCOMING;
    }

    /// <summary>
    /// Helper: Convert time units to days
    /// </summary>
    private static int ConvertToDays(int value, TimeUnitEnum unit)
    {
        return unit switch
        {
            TimeUnitEnum.Hours => (int)Math.Ceiling(value / 24.0),
            TimeUnitEnum.Days => value,
            TimeUnitEnum.Weeks => value * 7,
            _ => throw new ArgumentException($"Unsupported time unit: {unit}")
        };
    }
}