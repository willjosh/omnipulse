using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a single actionable service reminder for a vehicle, generated from a service schedule containing all associated service tasks.
/// </summary>
/// <remarks>
/// A service reminder is created from a <see cref="ServiceSchedule"/> and applies to a specific <see cref="Vehicle"/>.
/// It can be configured by time OR mileage, but not both (XOR constraint).
///
/// <list type="bullet">
///   <item>Must reference exactly one Vehicle via <see cref="VehicleID"/>.</item>
///   <item>Must reference exactly one ServiceSchedule via <see cref="ServiceScheduleID"/>.</item>
///   <item>Exactly one of the following scheduling options must be configured:
///     <list type="bullet">
///       <item>Time-based (<see cref="TimeIntervalValue"/> &amp; <see cref="TimeIntervalUnit"/>)</item>
///       <item>Mileage-based (<see cref="MileageInterval"/>)</item>
///     </list>
///   </item>
///   <item>Buffer values (<see cref="TimeBufferValue"/>, <see cref="MileageBuffer"/>) are optional and define when a reminder becomes "due soon".</item>
///   <item>Due information (<see cref="DueDate"/>, <see cref="DueMileage"/>) represents when the service is actually due.</item>
///   <item>The reminder can be linked to a <see cref="WorkOrder"/> when maintenance is scheduled.</item>
///   <item>Status is automatically calculated based on current date/mileage and buffer thresholds.</item>
/// </list>
/// <strong>Business Rules:</strong>
/// <list type="bullet">
///   <item>Each <see cref="ServiceSchedule"/> and <see cref="Vehicle"/> pair may have at most one <see cref="ServiceReminder"/> with status <see cref="ServiceReminderStatusEnum.UPCOMING"/> at any given time.</item>
/// </list>
/// </remarks>
public class ServiceReminder : BaseEntity
{
    // ===== FKs =====
    /// <summary>The ID of the vehicle this reminder applies to.</summary>
    public required int VehicleID { get; set; }
    /// <summary>Optional ID of the service program associated with this reminder.</summary>
    public int? ServiceProgramID { get; set; }
    /// <summary>The ID of the service schedule that generated this reminder.</summary>
    public required int ServiceScheduleID { get; set; }

    /// <summary>Optional ID of an associated work order if already scheduled.</summary>
    public int? WorkOrderID { get; set; }

    // ===== Due Information =====
    /// <summary>The date when this service is due.</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>The odometer or primary meter reading at which this service is due.</summary>
    public double? DueMileage { get; set; }

    // ===== Recurring Interval =====
    /// <summary>Recurring time interval value (e.g., 3 for "Every 3 Months").</summary>
    public int? TimeIntervalValue { get; set; }

    /// <summary>The unit for the recurring time interval.</summary>
    public TimeUnitEnum? TimeIntervalUnit { get; set; }

    /// <summary>Recurring mileage interval for this service (e.g., every 10,000 km).</summary>
    public double? MileageInterval { get; set; }

    // ===== Buffer =====
    /// <summary>The buffer value (in time units) before the due date for "due soon" status.</summary>
    public int? TimeBufferValue { get; set; }

    /// <summary>The unit for the time buffer (e.g., Days, Weeks).</summary>
    public TimeUnitEnum? TimeBufferUnit { get; set; }

    /// <summary>The buffer value (in kilometres) before the due mileage for "due soon" status.</summary>
    public int? MileageBuffer { get; set; }

    /// <summary>Difference between the vehicle's current meter and the DueMileage. Negative = distance left until due, Positive = overdue.</summary>
    public double? MeterVariance { get; set; }

    // ===== Linked Service Information =====
    /// <summary>The name of the <see cref="ServiceProgram"/> associated with this reminder, if any.</summary>
    public string? ServiceProgramName { get; set; }

    /// <summary>The name of the <see cref="ServiceSchedule"/> that generated this reminder.</summary>
    public required string ServiceScheduleName { get; set; }

    // ===== Status =====
    /// <summary>The priority level of this service reminder (e.g., Low, Medium, High, Critical).</summary>
    public required PriorityLevelEnum PriorityLevel { get; set; }

    /// <summary>The current status of this service reminder.</summary>
    public required ServiceReminderStatusEnum Status { get; set; }

    /// <summary>The date when this service was completed, if applicable.</summary>
    public DateTime? CompletedDate { get; set; }

    // ===== Navigation Properties =====
    /// <summary>Work order associated with this reminder.</summary>
    public WorkOrder? WorkOrder { get; set; }

    /// <summary>Navigation property to the related service program.</summary>
    public ServiceProgram? ServiceProgram { get; set; }

    /// <summary>Navigation property to the related service schedule.</summary>
    public required ServiceSchedule ServiceSchedule { get; set; }

    /// <summary>Navigation property to the related vehicle.</summary>
    public required Vehicle Vehicle { get; set; }
}

/// <summary>
/// Extension methods for ServiceReminder
/// </summary>
public static class ServiceReminderExtensions
{
    /// <summary>
    /// Determines the current <see cref="ServiceReminderStatusEnum"/> for a service reminder based on its due date/mileage, buffer thresholds, and the current vehicle state.
    /// </summary>
    /// <param name="serviceReminder">The reminder being evaluated.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <param name="currentDate">The current date/time (UTC).</param>
    /// <returns>The calculated status.</returns>
    public static ServiceReminderStatusEnum DetermineServiceReminderStatus(this ServiceReminder serviceReminder, double? currentOdometer, DateTime? currentDate = null)
    {
        // Use DateTime.UtcNow if no date is provided
        var now = currentDate ?? DateTime.UtcNow;

        // Preserve COMPLETED or CANCELLED state
        if (serviceReminder.IsFinalState()) return serviceReminder.Status;

        // ScheduleType XOR Constraint
        if (!serviceReminder.HasExactlyOneScheduleType()) throw new InvalidOperationException("Reminder must be either time-based or mileage-based.");

        // Check if overdue
        if (serviceReminder.IsOverdueByTime(now) || (currentOdometer.HasValue && serviceReminder.IsOverdueByMileage(currentOdometer.Value)))
            return ServiceReminderStatusEnum.OVERDUE;

        // Check if due soon
        if (serviceReminder.IsDueSoonByTime(now) || (currentOdometer.HasValue && serviceReminder.IsDueSoonByMileage(currentOdometer.Value)))
            return ServiceReminderStatusEnum.DUE_SOON;

        // Check if upcoming
        if (serviceReminder.IsUpcomingByTime(now) || (currentOdometer.HasValue && serviceReminder.IsUpcomingByMileage(currentOdometer.Value)))
            return ServiceReminderStatusEnum.UPCOMING;

        // Otherwise, it's just upcoming
        return ServiceReminderStatusEnum.UPCOMING;
    }

    /// <summary>
    /// Determines whether the service reminder is in a final state (e.g., completed or cancelled) and no longer actionable.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns><c>true</c> if the reminder is in a final state; otherwise, <c>false</c>.</returns>
    public static bool IsFinalState(this ServiceReminder serviceReminder)
    {
        return serviceReminder.Status == ServiceReminderStatusEnum.COMPLETED ||
               serviceReminder.Status == ServiceReminderStatusEnum.CANCELLED;
    }

    // ===== Status =====

    /// <summary>
    /// Checks if the reminder is upcoming by time (current time is before the buffer period starts).
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentDate">The current date/time (UTC). Uses DateTime.UtcNow if not provided.</param>
    /// <returns>True if the reminder is upcoming by time, false otherwise.</returns>
    public static bool IsUpcomingByTime(this ServiceReminder serviceReminder, DateTime? currentDate = null)
    {
        if (!serviceReminder.DueDate.HasValue ||
            !serviceReminder.TimeBufferValue.HasValue ||
            !serviceReminder.TimeBufferUnit.HasValue)
        {
            return false;
        }

        var now = currentDate ?? DateTime.UtcNow;
        var bufferStartDate = serviceReminder.DueDate.Value.AddDays(-ConvertToDays(serviceReminder.TimeBufferValue.Value, serviceReminder.TimeBufferUnit.Value));

        return now < bufferStartDate;
    }

    /// <summary>
    /// Checks if the reminder is upcoming by mileage (current mileage is before the buffer period starts).
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <returns>True if the reminder is upcoming by mileage, false otherwise.</returns>
    public static bool IsUpcomingByMileage(this ServiceReminder serviceReminder, double currentOdometer)
    {
        if (!serviceReminder.DueMileage.HasValue ||
            !serviceReminder.MileageBuffer.HasValue)
        {
            return false;
        }

        var bufferStartMileage = serviceReminder.DueMileage - serviceReminder.MileageBuffer;

        return currentOdometer < bufferStartMileage;
    }

    /// <summary>
    /// Checks if the service reminder is due soon by time (within the buffer period).
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentDate">The current date/time (UTC). Uses DateTime.UtcNow if not provided.</param>
    /// <returns>True if due soon by time, false otherwise.</returns>
    public static bool IsDueSoonByTime(this ServiceReminder serviceReminder, DateTime? currentDate = null)
    {
        if (!serviceReminder.DueDate.HasValue ||
            !serviceReminder.TimeBufferValue.HasValue ||
            !serviceReminder.TimeBufferUnit.HasValue)
        {
            return false;
        }

        var now = currentDate ?? DateTime.UtcNow;
        var bufferStartDate = serviceReminder.DueDate.Value.AddDays(-ConvertToDays(serviceReminder.TimeBufferValue.Value, serviceReminder.TimeBufferUnit.Value));

        return now >= bufferStartDate && now < serviceReminder.DueDate;
    }

    /// <summary>
    /// Checks if the service reminder is due soon by mileage (within the buffer period).
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <returns>True if due soon by mileage, false otherwise.</returns>
    public static bool IsDueSoonByMileage(this ServiceReminder serviceReminder, double currentOdometer)
    {
        if (!serviceReminder.DueMileage.HasValue ||
            !serviceReminder.MileageBuffer.HasValue)
        {
            return false;
        }

        var bufferStartMileage = serviceReminder.DueMileage - serviceReminder.MileageBuffer;

        return currentOdometer >= bufferStartMileage && currentOdometer < serviceReminder.DueMileage;
    }

    /// <summary>
    /// Checks if the service reminder is overdue by time.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentDate">The current date/time (UTC). Uses DateTime.UtcNow if not provided.</param>
    /// <returns>True if overdue by time, false otherwise.</returns>
    public static bool IsOverdueByTime(this ServiceReminder serviceReminder, DateTime? currentDate = null)
    {
        var now = currentDate ?? DateTime.UtcNow;
        return serviceReminder.DueDate.HasValue && now > serviceReminder.DueDate;
    }

    /// <summary>
    /// Checks if the service reminder is overdue by mileage.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <returns>True if overdue by mileage, false otherwise.</returns>
    public static bool IsOverdueByMileage(this ServiceReminder serviceReminder, double currentOdometer)
    {
        return serviceReminder.DueMileage.HasValue && currentOdometer > serviceReminder.DueMileage;
    }

    // ===== Type Checking Methods =====

    /// <summary>
    /// Validates that the reminder has exactly one schedule type configured (XOR constraint).
    /// ServiceReminders must be either time-based OR mileage-based, never both.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns><c>true</c> if exactly one schedule type is configured; otherwise, <c>false</c>.</returns>
    public static bool HasExactlyOneScheduleType(this ServiceReminder serviceReminder)
    {
        return serviceReminder.IsTimeBased() != serviceReminder.IsMileageBased(); // Exactly one must be true (XOR)
    }

    /// <summary>
    /// Determines the schedule type based on configured intervals.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>The determined schedule type.</returns>
    /// <exception cref="InvalidOperationException">Thrown when neither time nor mileage is configured, or when both are configured (violates XOR constraint).</exception>
    public static ServiceScheduleTypeEnum GetScheduleType(this ServiceReminder serviceReminder)
    {
        if (serviceReminder.IsTimeBased() && !serviceReminder.IsMileageBased())
            return ServiceScheduleTypeEnum.TIME;
        if (!serviceReminder.IsTimeBased() && serviceReminder.IsMileageBased())
            return ServiceScheduleTypeEnum.MILEAGE;

        throw new InvalidOperationException("ServiceReminder must have exactly one schedule type configured (either time OR mileage, not both or neither)");
    }

    /// <summary>
    /// Checks if the reminder is time-based.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>True if the reminder uses time-based scheduling, false otherwise.</returns>
    public static bool IsTimeBased(this ServiceReminder serviceReminder)
    {
        return serviceReminder.TimeIntervalValue.HasValue && serviceReminder.TimeIntervalUnit.HasValue;
    }

    /// <summary>
    /// Checks if the reminder is mileage-based.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>True if the reminder uses mileage-based scheduling, false otherwise.</returns>
    public static bool IsMileageBased(this ServiceReminder serviceReminder)
    {
        return serviceReminder.MileageInterval.HasValue;
    }

    // ===== Calculation Methods =====

    /// <summary>
    /// Calculates the number of days until this reminder is due.
    /// Negative means overdue by X days.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentDate">The current date/time (UTC). Uses DateTime.UtcNow if not provided.</param>
    /// <returns>Days until due (negative if overdue), or null if no due date is set.</returns>
    public static int? DaysUntilDue(this ServiceReminder serviceReminder, DateTime? currentDate = null)
    {
        if (!serviceReminder.DueDate.HasValue) return null;
        var now = currentDate ?? DateTime.UtcNow;
        return (int)(serviceReminder.DueDate.Value - now).TotalDays;
    }

    /// <summary>
    /// Calculates the mileage variance between current odometer and the due mileage.
    /// Negative = km until due, Positive = overdue by km.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <returns>Mileage variance (negative if not yet due, positive if overdue), or null if due mileage is not available.</returns>
    public static double? CalculateMileageVariance(this ServiceReminder serviceReminder, double currentOdometer)
    {
        if (!serviceReminder.DueMileage.HasValue) return null;
        return currentOdometer - serviceReminder.DueMileage;
    }

    /// <summary>
    /// Calculates the priority level based on the reminder's current status.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>The calculated priority level.</returns>
    public static PriorityLevelEnum CalculatePriorityLevel(this ServiceReminder serviceReminder)
    {
        return serviceReminder.Status switch
        {
            ServiceReminderStatusEnum.OVERDUE => PriorityLevelEnum.HIGH,
            ServiceReminderStatusEnum.DUE_SOON => PriorityLevelEnum.MEDIUM,
            _ => PriorityLevelEnum.LOW
        };
    }

    /// <summary>
    /// Gets the next scheduled occurrence date for time-based reminders.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>The next occurrence date, or null if not time-based or calculation not possible.</returns>
    public static DateTime? GetNextScheduledDate(this ServiceReminder serviceReminder)
    {
        if (!serviceReminder.IsTimeBased() || !serviceReminder.DueDate.HasValue) return null;

        return serviceReminder.TimeIntervalUnit!.Value switch
        {
            TimeUnitEnum.Hours => serviceReminder.DueDate.Value.AddHours(serviceReminder.TimeIntervalValue ?? 0),
            TimeUnitEnum.Days => serviceReminder.DueDate.Value.AddDays(serviceReminder.TimeIntervalValue ?? 0),
            TimeUnitEnum.Weeks => serviceReminder.DueDate.Value.AddDays((serviceReminder.TimeIntervalValue ?? 0) * 7),
            _ => null
        };
    }

    /// <summary>
    /// Gets the next scheduled mileage for mileage-based reminders.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>The next occurrence mileage, or null if not mileage-based or calculation not possible.</returns>
    public static double? GetNextScheduledMileage(this ServiceReminder serviceReminder)
    {
        if (!serviceReminder.IsMileageBased() || !serviceReminder.DueMileage.HasValue || !serviceReminder.MileageInterval.HasValue)
            return null;
        return serviceReminder.DueMileage + serviceReminder.MileageInterval;
    }

    // ===== Status Check Methods =====

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