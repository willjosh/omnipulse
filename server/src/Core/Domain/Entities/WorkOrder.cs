using System;
using Domain.Entities.Enums;


namespace Domain.Entities;

public class WorkOrder : BaseEntity
{
    public required string WorkOrderNumber { get; set; }
    public required int VehicleID { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required WorkTypeEnum WorkOrderType { get; set; }
    public required PriorityLevelEnum PriorityLevel { get; set; }
    public required WorkOrderStatusEnum Status { get; set; }
    public double? EstimatedCost { get; set; }
    public double? ActualCost { get; set; }
    public double? Estimated_Hours { get; set; }
    public double? ActualHours { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public required double StartOdometer { get; set; }
    public double? EndOdometer { get; set; }

    // Navigation properties
    public required Vehicle Vehicle { get; set; }
    public required ICollection<MaintenanceHistory> MaintenanceHistories { get; set; }
}
