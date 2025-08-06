using Domain.Entities.Enums;

namespace Application.Features.ServiceReminders.Query;

/// <summary>
/// Represents a calculated service reminder for a specific vehicle and service task occurrence.
/// </summary>
public class ServiceReminderDTO
{
    /// <summary>The ID of this service reminder.</summary>
    /// <example>1</example>
    public required int ID { get; set; }

    /// <summary>Optional ID of the work order this reminder is linked to (set when AddServiceReminderToExistingWorkOrderCommand is called).</summary>
    /// <example>1</example>
    public int? WorkOrderID { get; set; }

    /// <summary>Vehicle ID this reminder applies to.</summary>
    /// <example>1</example>
    public required int VehicleID { get; set; }

    /// <summary>Vehicle name for display purposes.</summary>
    /// <example>Bus #001</example>
    public required string VehicleName { get; set; }

    /// <summary>Service program ID associated with this reminder.</summary>
    /// <example>1</example>
    public int? ServiceProgramID { get; set; }

    /// <summary>Service program name for display purposes.</summary>
    /// <example>Standard Maintenance Program</example>
    public string? ServiceProgramName { get; set; }

    /// <summary>Service schedule ID that generated this reminder.</summary>
    /// <example>1</example>
    public required int ServiceScheduleID { get; set; }

    /// <summary>Service schedule name for display purposes.</summary>
    /// <example>Oil Change Schedule</example>
    public required string ServiceScheduleName { get; set; }

    /// <summary>Collection of service tasks included in this reminder.</summary>
    public required List<ServiceTaskInfoDTO> ServiceTasks { get; set; } = [];

    /// <summary>Total estimated labor hours for all tasks in this reminder.</summary>
    /// <example>3.5</example>
    public required double TotalEstimatedLabourHours { get; set; }

    /// <summary>Total estimated cost for all tasks in this reminder.</summary>
    /// <example>225.75</example>
    public required decimal TotalEstimatedCost { get; set; }

    /// <summary>Number of tasks included in this reminder.</summary>
    /// <example>2</example>
    public required int TaskCount { get; set; }

    /// <summary>The calculated due date for this service occurrence.</summary>
    /// <example>2024-08-15T00:00:00</example>
    public DateTime? DueDate { get; set; }

    /// <summary>The calculated due mileage for this service occurrence.</summary>
    /// <example>15000.0</example>
    public double? DueMileage { get; set; }

    /// <summary>Current status of this reminder.</summary>
    /// <example>2</example>
    public required ServiceReminderStatusEnum Status { get; set; }

    /// <summary>Priority level calculated based on status and other factors.</summary>
    /// <example>3</example>
    public required PriorityLevelEnum PriorityLevel { get; set; }

    /// <summary>Time interval value for recurrence.</summary>
    /// <example>7</example>
    public int? TimeIntervalValue { get; set; }

    /// <summary>Time interval unit for recurrence.</summary>
    /// <example>2</example>
    public TimeUnitEnum? TimeIntervalUnit { get; set; }

    /// <summary>Mileage interval for recurrence.</summary>
    /// <example>5000.0</example>
    public double? MileageInterval { get; set; }

    /// <summary>Time buffer value for due soon threshold.</summary>
    /// <example>3</example>
    public int? TimeBufferValue { get; set; }

    /// <summary>Time buffer unit for due soon threshold.</summary>
    /// <example>2</example>
    public TimeUnitEnum? TimeBufferUnit { get; set; }

    /// <summary>Mileage buffer for due soon threshold.</summary>
    /// <example>500</example>
    public int? MileageBuffer { get; set; }

    /// <summary>Current vehicle mileage at time of calculation.</summary>
    /// <example>22000.0</example>
    public required double CurrentMileage { get; set; }

    /// <summary>Difference between current mileage and due mileage (negative = distance left, positive = overdue).</summary>
    /// <example>2000.0</example>
    public double? MileageVariance { get; set; }

    /// <summary>Days until due (negative = overdue by X days).</summary>
    /// <example>-3</example>
    public int? DaysUntilDue { get; set; }

    /// <summary>Occurrence number for this specific interval (1 = first occurrence, 2 = second, etc.).</summary>
    /// <example>2</example>
    public required int OccurrenceNumber { get; set; }

    /// <summary>Whether this reminder was calculated based on time intervals.</summary>
    /// <example>true</example>
    public required bool IsTimeBasedReminder { get; set; }

    /// <summary>Whether this reminder was calculated based on mileage intervals.</summary>
    /// <example>false</example>
    public required bool IsMileageBasedReminder { get; set; }
}