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
    public required ReminderStatusEnum Status { get; set; }

    /// <summary>The date when this service was completed, if applicable.</summary>
    public DateTime? CompletedDate { get; set; }

    // ===== Navigation Properties =====
    /// <summary>Work order associated with this reminder.</summary>
    public required WorkOrder? WorkOrder { get; set; }

    /// <summary>Navigation property to the related service program.</summary>
    public required ServiceProgram ServiceProgram { get; set; }

    /// <summary>Navigation property to the related service schedule.</summary>
    public required ServiceSchedule ServiceSchedule { get; set; }

    /// <summary>Navigation property to the related service task.</summary>
    public required ServiceTask ServiceTask { get; set; }

    /// <summary>Navigation property to the related vehicle.</summary>
    public required Vehicle Vehicle { get; set; }
}