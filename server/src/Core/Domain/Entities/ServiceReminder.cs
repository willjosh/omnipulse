using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>Represents a single actionable service reminder for a vehicle, generated from a service schedule and linked to a specific service task.</summary>
public class ServiceReminder : BaseEntity
{
    // ===== FKs =====
    /// <summary>The ID of the vehicle this reminder applies to.</summary>
    public required int VehicleID { get; set; }
    /// <summary>Optional ID of the service program associated with this reminder.</summary>
    public int? ServiceProgramID { get; set; }
    /// <summary>The ID of the service schedule that generated this reminder.</summary>
    public required int ServiceScheduleID { get; set; }
    /// <summary>The ID of the service task that needs to be performed.</summary>
    public required int ServiceTaskID { get; set; }
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

    /// <summary>The name of the <see cref="ServiceTask"/> that needs to be performed.</summary>
    public required string ServiceTaskName { get; set; }

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

    /// <summary>Navigation property to the related service task.</summary>
    public required ServiceTask ServiceTask { get; set; }

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
}