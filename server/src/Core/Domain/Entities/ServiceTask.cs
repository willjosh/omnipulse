namespace Domain.Entities;
using Domain.Entities.Enums;

public class ServiceTask : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required double EstimatedLabourHours { get; set; }
    public required decimal EstimatedCost { get; set; }
    public required ServiceTaskCategoryEnum Category { get; set; }
    public required bool IsActive { get; set; } = true;

    // Navigation Properties
    public required ICollection<XrefServiceScheduleServiceTask> XrefServiceScheduleServiceTasks { get; set; } = [];
    public required ICollection<MaintenanceHistory> MaintenanceHistories { get; set; } = [];
    public required ICollection<WorkOrderLineItem> WorkOrderLineItems { get; set; } = [];
}