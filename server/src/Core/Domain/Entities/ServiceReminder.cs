using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>Represents a single actionable service reminder for a vehicle, generated from a service schedule containing all associated service tasks.</summary>
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
    public int? TimeBufferValue { get; set; }
    public TimeUnitEnum? TimeBufferUnit { get; set; }
    public int? MileageBuffer { get; set; } // km

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
        if (serviceReminder.Status == ServiceReminderStatusEnum.COMPLETED ||
            serviceReminder.Status == ServiceReminderStatusEnum.CANCELLED)
        {
            return serviceReminder.Status;
        }

        // Overdue checks
        bool isOverdueByTime = serviceReminder.DueDate.HasValue && now > serviceReminder.DueDate.Value;
        bool isOverdueByMileage = serviceReminder.DueMileage.HasValue &&
                                  currentOdometer.HasValue &&
                                  currentOdometer.Value > serviceReminder.DueMileage.Value;

        if (isOverdueByTime || isOverdueByMileage) return ServiceReminderStatusEnum.OVERDUE;

        // Due soon checks
        bool isDueSoonByTime = serviceReminder.DueDate.HasValue && serviceReminder.TimeBufferValue.HasValue &&
                               now >= serviceReminder.DueDate.Value.AddDays(-serviceReminder.TimeBufferValue.Value) &&
                               now < serviceReminder.DueDate.Value;

        bool isDueSoonByMileage = serviceReminder.DueMileage.HasValue &&
                                  serviceReminder.MileageBuffer.HasValue &&
                                  currentOdometer.HasValue &&
                                  currentOdometer.Value >= (serviceReminder.DueMileage.Value - serviceReminder.MileageBuffer.Value) &&
                                  currentOdometer.Value < serviceReminder.DueMileage.Value;

        if (isDueSoonByTime || isDueSoonByMileage) return ServiceReminderStatusEnum.DUE_SOON;

        // Otherwise, it's just upcoming
        return ServiceReminderStatusEnum.UPCOMING;
    }

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
    /// <returns>Mileage variance (negative if not yet due, positive if overdue), or null if due mileage or current odometer is not available.</returns>
    public static double? CalculateMileageVariance(this ServiceReminder serviceReminder, double? currentOdometer)
    {
        if (!serviceReminder.DueMileage.HasValue || !currentOdometer.HasValue) return null;
        return currentOdometer.Value - serviceReminder.DueMileage.Value;
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
    /// Calculates the next due date based on the current due date and the time interval.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>The next due date, or null if calculation is not possible.</returns>
    public static DateTime? CalculateNextDueDate(this ServiceReminder serviceReminder)
    {
        if (!serviceReminder.DueDate.HasValue || !serviceReminder.TimeIntervalValue.HasValue || !serviceReminder.TimeIntervalUnit.HasValue)
            return null;

        return serviceReminder.TimeIntervalUnit.Value switch
        {
            TimeUnitEnum.Days => serviceReminder.DueDate.Value.AddDays(serviceReminder.TimeIntervalValue.Value),
            TimeUnitEnum.Weeks => serviceReminder.DueDate.Value.AddDays(serviceReminder.TimeIntervalValue.Value * 7),
            _ => null
        };
    }

    /// <summary>
    /// Calculates the next due mileage based on the current due mileage and mileage interval.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>The next due mileage, or null if calculation is not possible.</returns>
    public static double? CalculateNextDueMileage(this ServiceReminder serviceReminder)
    {
        if (!serviceReminder.DueMileage.HasValue || !serviceReminder.MileageInterval.HasValue)
            return null;

        return serviceReminder.DueMileage.Value + serviceReminder.MileageInterval.Value;
    }

    /// <summary>
    /// Checks if the reminder is active (not completed or cancelled).
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>True if the reminder is active, false otherwise.</returns>
    public static bool IsActive(this ServiceReminder serviceReminder)
    {
        return serviceReminder.Status != ServiceReminderStatusEnum.COMPLETED &&
               serviceReminder.Status != ServiceReminderStatusEnum.CANCELLED;
    }

    /// <summary>
    /// Checks if the reminder is due today.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentDate">The current date/time (UTC). Uses DateTime.UtcNow if not provided.</param>
    /// <returns>True if the reminder is due today, false otherwise.</returns>
    public static bool IsDueToday(this ServiceReminder serviceReminder, DateTime? currentDate = null)
    {
        if (!serviceReminder.DueDate.HasValue) return false;
        var now = currentDate ?? DateTime.UtcNow;
        return serviceReminder.DueDate.Value.Date == now.Date;
    }

    /// <summary>
    /// Checks if the reminder is overdue (past its due date or mileage).
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <param name="currentDate">The current date/time (UTC). Uses DateTime.UtcNow if not provided.</param>
    /// <returns>True if the reminder is overdue, false otherwise.</returns>
    public static bool IsOverdue(this ServiceReminder serviceReminder, double? currentOdometer = null, DateTime? currentDate = null)
    {
        var now = currentDate ?? DateTime.UtcNow;

        bool isOverdueByTime = serviceReminder.DueDate.HasValue && now > serviceReminder.DueDate.Value;
        bool isOverdueByMileage = serviceReminder.DueMileage.HasValue &&
                                  currentOdometer.HasValue &&
                                  currentOdometer.Value > serviceReminder.DueMileage.Value;

        return isOverdueByTime || isOverdueByMileage;
    }

    /// <summary>
    /// Generates a human-readable summary of the reminder for display purposes.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <param name="currentOdometer">The vehicle's current odometer reading (km).</param>
    /// <param name="currentDate">The current date/time (UTC). Uses DateTime.UtcNow if not provided.</param>
    /// <returns>A formatted summary string describing the reminder's urgency and details.</returns>
    public static string GetDisplaySummary(this ServiceReminder serviceReminder, double? currentOdometer = null, DateTime? currentDate = null)
    {
        var status = serviceReminder.Status.ToString().Replace("_", " ").ToLowerInvariant();
        var scheduleName = serviceReminder.ServiceScheduleName;

        if (serviceReminder.Status == ServiceReminderStatusEnum.COMPLETED)
        {
            return $"{scheduleName} - Completed on {serviceReminder.CompletedDate?.ToString("MMM dd, yyyy")}";
        }

        if (serviceReminder.Status == ServiceReminderStatusEnum.CANCELLED)
        {
            return $"{scheduleName} - Cancelled";
        }

        var dueDateInfo = serviceReminder.DueDate.HasValue ? $"due {serviceReminder.DueDate.Value:MMM dd, yyyy}" : "";
        var dueMileageInfo = serviceReminder.DueMileage.HasValue ? $"due at {serviceReminder.DueMileage.Value:N0} km" : "";

        var dueInfo = string.Join(" or ", new[] { dueDateInfo, dueMileageInfo }.Where(s => !string.IsNullOrEmpty(s)));

        return string.IsNullOrEmpty(dueInfo)
            ? $"{scheduleName} - {status}"
            : $"{scheduleName} - {status} ({dueInfo})";
    }

    /// <summary>
    /// Checks if the reminder is time-based only (no mileage scheduling).
    /// Reflects XOR constraint - ServiceReminders can only be time-based OR mileage-based, never both.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>True if the reminder uses only time-based scheduling, false otherwise.</returns>
    public static bool IsTimeBasedOnly(this ServiceReminder serviceReminder)
    {
        return serviceReminder.IsTimeBased() && !serviceReminder.IsMileageBased();
    }

    /// <summary>
    /// Checks if the reminder is mileage-based only (no time scheduling).
    /// Reflects XOR constraint - ServiceReminders can only be time-based OR mileage-based, never both.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>True if the reminder uses only mileage-based scheduling, false otherwise.</returns>
    public static bool IsMileageBasedOnly(this ServiceReminder serviceReminder)
    {
        return !serviceReminder.IsTimeBased() && serviceReminder.IsMileageBased();
    }

    /// <summary>
    /// Validates that the reminder has exactly one schedule type configured (XOR constraint).
    /// ServiceReminders must be either time-based OR mileage-based, never both.
    /// </summary>
    /// <param name="serviceReminder">The service reminder to evaluate.</param>
    /// <returns>True if the reminder has exactly one schedule type configured.</returns>
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
        if (!serviceReminder.IsMileageBased() || !serviceReminder.DueMileage.HasValue) return null;
        return serviceReminder.DueMileage.Value + serviceReminder.MileageInterval!.Value;
    }
}