using System;

using Domain.Entities.Enums;

namespace Domain.Entities;

public class WorkOrder : BaseEntity

{
    public required int VehicleID { get; set; }
    public required string AssignedToUserID { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required WorkTypeEnum WorkOrderType { get; set; }
    public required PriorityLevelEnum PriorityLevel { get; set; }
    public required WorkOrderStatusEnum Status { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public required double StartOdometer { get; set; }
    public double? EndOdometer { get; set; }

    // Navigation properties
    public required Vehicle Vehicle { get; set; }
    public required ICollection<MaintenanceHistory> MaintenanceHistories { get; set; } = [];
    public required User User { get; set; }
    public required ICollection<WorkOrderLineItem> WorkOrderLineItems { get; set; } = [];
    public required ICollection<Invoice> Invoices { get; set; } = [];
    public required ICollection<InventoryTransaction> InventoryTransactions { get; set; } = [];
}