using System;
using Domain.Entities.Enums;


namespace Domain.Entities;

public class WorkOrder
{
    public required string WorkOrderNumber;
    public required string Title;
    public string? Description;
    public required WorkTypeEnum WorkOrderType;
    public required PriorityLevelEnum PriorityLevel;
    public required WorkOrderStatusEnum Status;
    public double? EstimatedCost;
    public double? ActualCost;
    public double? Estimated_Hours;
    public double? ActualHours;
    public DateTime? ScheduledStartDate;
    public DateTime? ActualStartDate;
    public required double StartOdometer;
    public double? EndOdometer;

    // TODO: Navigation properties
}
