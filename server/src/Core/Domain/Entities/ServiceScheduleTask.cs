using System;

namespace Domain.Entities;

public class ServiceScheduleTask : BaseEntity
{
    // FKs
    public required int ServiceScheduleID { get; set; }
    public required int ServiceTaskID { get; set; }

    // Navigation Properties
    public required ServiceSchedule ServiceSchedule { get; set; }
    public required ServiceTask ServiceTask { get; set; }
}