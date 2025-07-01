using System;

namespace Domain.Entities;

public class ServiceSchedule : BaseEntity
{
    public required int ServiceProgramID { get; set; }
    public required string Name { get; set; }
    public required int IntervalMileage { get; set; }
    public required int IntervalDays { get; set; }
    public required int IntervalHours { get; set; }
    public required int BufferMileage { get; set; }
    public required int BufferDays { get; set; }
    public required bool IsActive { get; set; } = true;

    // Navigation Properties
    public required ICollection<ServiceScheduleTask> ServiceScheduleTasks { get; set; } = [];
    public required ServiceProgram ServiceProgram { get; set; }
}