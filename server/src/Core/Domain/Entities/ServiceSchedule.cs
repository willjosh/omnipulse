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