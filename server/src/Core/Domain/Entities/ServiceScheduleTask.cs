using System;

namespace Domain.Entities;

public class ServiceScheduleTask : BaseEntity
{
    // FKs
    public required int ServiceScheduleID { get; set; }
    public required int ServiceTaskID { get; set; }

    public required bool IsMandatory { get; set; } = true;
    public required int SequenceOrder { get; set; } // TODO: Change to SequenceNumber?

    // Navigation properties
    public required ServiceSchedule ServiceSchedule { get; set; }
    public required ServiceTask ServiceTask { get; set; }
}
