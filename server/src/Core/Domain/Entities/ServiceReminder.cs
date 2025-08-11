using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a service reminder - a computed target (due date/mileage) for a vehicle from a service schedule.
/// </summary>
/// <remarks>
/// A service reminder stores only the computed "next trigger" (due targets) and state, not schedule configuration.
/// Schedule rules remain on <see cref="ServiceSchedule"/> to avoid data duplication and drift.
///
/// <list type="bullet">
///   <item>Must reference exactly one Vehicle via <see cref="VehicleID"/>.</item>
///   <item>Must reference exactly one ServiceSchedule via <see cref="ServiceScheduleID"/>.</item>
///   <item>Stores computed due targets (<see cref="DueDate"/> and/or <see cref="DueMileage"/>) based on schedule type.</item>
///   <item>Can be linked to a <see cref="WorkOrder"/> when maintenance is scheduled.</item>
///   <item>Status calculation uses current vehicle state and schedule buffer rules (computed on demand).</item>
/// </list>
///
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

    /// <summary>The ID of the service schedule that generated this reminder.</summary>
    public required int ServiceScheduleID { get; set; }

    /// <summary>Optional ID of an associated work order if already scheduled.</summary>
    public int? WorkOrderID { get; set; }

    // ===== Computed Due Targets =====
    /// <summary>The date when this service is due (for time-based schedules).</summary>
    public DateTime? DueDate { get; set; }

    /// <summary>The mileage at which this service is due (for mileage-based schedules).</summary>
    public double? DueMileage { get; set; }

    // ===== State =====
    /// <summary>The current status of this service reminder.</summary>
    public required ServiceReminderStatusEnum Status { get; set; }

    /// <summary>The date when this service was completed, if applicable.</summary>
    public DateTime? CompletedDate { get; set; }

    /// <summary>Reason for cancellation</summary>
    /// <example>Schedule deleted</example>
    public string? CancelReason { get; set; }

    // ===== Navigation Properties =====
    /// <summary>Navigation property to the related vehicle.</summary>
    public required Vehicle Vehicle { get; set; }

    /// <summary>Navigation property to the related service schedule.</summary>
    public required ServiceSchedule ServiceSchedule { get; set; }

    /// <summary>Work order associated with this reminder.</summary>
    public WorkOrder? WorkOrder { get; set; }
}

/// <summary>
/// Extension methods for ServiceReminder
/// </summary>
public static class ServiceReminderExtensions
{
    /// <summary>
    /// Single source of truth for final states.
    /// </summary>
    public static readonly ServiceReminderStatusEnum[] FinalStatuses =
    [
        ServiceReminderStatusEnum.COMPLETED,
        ServiceReminderStatusEnum.CANCELLED
    ];

    /// <summary>
    /// Determines the current <see cref="ServiceReminderStatusEnum"/> based on due targets and schedule buffers.
    /// </summary>
    /// <param name="serviceReminder">The reminder being evaluated.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <param name="currentDate">The current date/time (UTC).</param>
    /// <returns>The calculated status.</returns>
    public static ServiceReminderStatusEnum DetermineServiceReminderStatus(this ServiceReminder serviceReminder, double? currentOdometer, DateTime? currentDate = null)
    {
        var now = currentDate ?? DateTime.UtcNow;

        // Preserve final states
        if (serviceReminder.IsFinalState()) return serviceReminder.Status;

        // Ensure we have the schedule data
        if (serviceReminder.ServiceSchedule == null)
            throw new InvalidOperationException("ServiceSchedule must be loaded to calculate status");

        // Use the schedule's buffer values to determine status
        var schedule = serviceReminder.ServiceSchedule;

        // Check if overdue
        if (serviceReminder.IsOverdueByTime(now) || (currentOdometer.HasValue && serviceReminder.IsOverdueByMileage(currentOdometer.Value)))
            return ServiceReminderStatusEnum.OVERDUE;

        // Check if due soon using schedule's buffer configuration
        if (serviceReminder.IsDueSoonByTime(schedule, now) || (currentOdometer.HasValue && serviceReminder.IsDueSoonByMileage(schedule, currentOdometer.Value)))
            return ServiceReminderStatusEnum.DUE_SOON;

        // Otherwise, it's just upcoming
        return ServiceReminderStatusEnum.UPCOMING;
    }

    /// <summary>
    /// Determines whether the service reminder is in a final state (completed or cancelled).
    /// </summary>
    public static bool IsFinalState(this ServiceReminder serviceReminder)
    {
        return FinalStatuses.Contains(serviceReminder.Status);
    }

    // ===== Status Checks =====

    /// <summary>
    /// Checks if the reminder is overdue by time.
    /// </summary>
    public static bool IsOverdueByTime(this ServiceReminder serviceReminder, DateTime? currentDate = null)
    {
        var now = currentDate ?? DateTime.UtcNow;
        return serviceReminder.DueDate.HasValue && now > serviceReminder.DueDate;
    }

    /// <summary>
    /// Checks if the reminder is overdue by mileage.
    /// </summary>
    public static bool IsOverdueByMileage(this ServiceReminder serviceReminder, double currentOdometer)
    {
        return serviceReminder.DueMileage.HasValue && currentOdometer > serviceReminder.DueMileage;
    }

    /// <summary>
    /// Checks if the reminder is due soon by time using schedule's buffer configuration.
    /// </summary>
    public static bool IsDueSoonByTime(this ServiceReminder serviceReminder, ServiceSchedule schedule, DateTime? currentDate = null)
    {
        if (!serviceReminder.DueDate.HasValue ||
            !schedule.TimeBufferValue.HasValue ||
            !schedule.TimeBufferUnit.HasValue)
        {
            return false;
        }

        var now = currentDate ?? DateTime.UtcNow;
        var bufferStartDate = serviceReminder.DueDate.Value.AddDays(-ConvertToDays(schedule.TimeBufferValue.Value, schedule.TimeBufferUnit.Value));

        return now >= bufferStartDate && now < serviceReminder.DueDate;
    }

    /// <summary>
    /// Checks if the reminder is due soon by mileage using schedule's buffer configuration.
    /// </summary>
    public static bool IsDueSoonByMileage(this ServiceReminder serviceReminder, ServiceSchedule schedule, double currentOdometer)
    {
        if (!serviceReminder.DueMileage.HasValue || !schedule.MileageBuffer.HasValue)
        {
            return false;
        }

        var bufferStartMileage = serviceReminder.DueMileage.Value - schedule.MileageBuffer.Value;
        return currentOdometer >= bufferStartMileage && currentOdometer < serviceReminder.DueMileage;
    }

    // ===== Type Determination (based on due targets) =====

    /// <summary>
    /// Determines the schedule type based on which due targets are set.
    /// </summary>
    public static ServiceScheduleTypeEnum GetScheduleType(this ServiceReminder serviceReminder)
    {
        // Use the associated schedule if available
        if (serviceReminder.ServiceSchedule != null)
            return serviceReminder.ServiceSchedule.GetScheduleType();

        // Otherwise infer from due targets
        if (serviceReminder.DueDate.HasValue && !serviceReminder.DueMileage.HasValue)
            return ServiceScheduleTypeEnum.TIME;
        if (!serviceReminder.DueDate.HasValue && serviceReminder.DueMileage.HasValue)
            return ServiceScheduleTypeEnum.MILEAGE;

        throw new InvalidOperationException("Cannot determine schedule type from reminder");
    }

    // ===== Calculation Methods =====

    /// <summary>
    /// Calculates the number of days until this reminder is due.
    /// Negative means overdue by X days.
    /// </summary>
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
    public static double? CalculateMileageVariance(this ServiceReminder serviceReminder, double currentOdometer)
    {
        if (!serviceReminder.DueMileage.HasValue) return null;
        return currentOdometer - serviceReminder.DueMileage.Value;
    }

    // ===== Helper Methods =====

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

    /// <summary>
    /// Determines whether a mileage-based due point should be considered <see cref="ServiceReminderStatusEnum.UPCOMING"/>.
    /// <see cref="ServiceReminderStatusEnum.UPCOMING"/> when <paramref name="currentMileage"/> &lt; (<paramref name="dueMileage"/> - <paramref name="dueSoonThresholdMileage"/>).
    /// </summary>
    /// <param name="currentMileage">The vehicle's current odometer (km).</param>
    /// <param name="dueMileage">The due mileage for the occurrence (km).</param>
    /// <param name="dueSoonThresholdMileage">The due-soon buffer (km).</param>
    /// <returns><c>true</c> if <see cref="ServiceReminderStatusEnum.UPCOMING"/> by mileage; otherwise <c>false</c>.</returns>
    public static bool IsUpcomingByMileage(double currentMileage, double dueMileage, double dueSoonThresholdMileage)
    {
        return currentMileage < (dueMileage - dueSoonThresholdMileage);
    }

    /// <summary>
    /// Determines whether a time-based due point should be considered <see cref="ServiceReminderStatusEnum.UPCOMING"/>.
    /// A due point is <see cref="ServiceReminderStatusEnum.UPCOMING"/> when <paramref name="nowUtc"/> is strictly earlier than
    /// (<paramref name="dueDateUtc"/> minus the due-soon threshold derived from <paramref name="dueSoonValue"/> and <paramref name="dueSoonUnit"/>).
    /// </summary>
    /// <param name="nowUtc">Current time (UTC).</param>
    /// <param name="dueDateUtc">Due date (UTC).</param>
    /// <param name="dueSoonValue">Due-soon threshold magnitude.</param>
    /// <param name="dueSoonUnit">Due-soon threshold unit.</param>
    /// <returns><c>true</c> if <see cref="ServiceReminderStatusEnum.UPCOMING"/> by time; otherwise <c>false</c>.</returns>
    public static bool IsUpcomingByTime(DateTime nowUtc, DateTime dueDateUtc, int dueSoonValue, TimeUnitEnum dueSoonUnit)
    {
        var bufferDays = ConvertToDays(dueSoonValue, dueSoonUnit);
        return nowUtc < dueDateUtc.AddDays(-bufferDays);
    }
}