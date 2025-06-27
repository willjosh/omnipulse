using System;

namespace Domain.Entities;

public class MaintenanceHistory : BaseEntity
{
    public required int VehicleID { get; set; }
    public required int WorkOrderID { get; set; }
    public required int ServiceTaskID { get; set; }
    public required int TechnicianID { get; set; }
    public required DateTime ServiceDate { get; set; }
    public required double MileageAtService { get; set; }
    public string? Description { get; set; }
    public required decimal Cost { get; set; }
    public required double LabourHours { get; set; }
    public string? Notes { get; set; }

    // navigation properties
    public required Vehicle Vehicle { get; set; }
    public required WorkOrder WorkOrder { get; set; }
    public required ServiceTask ServiceTask { get; set; }
    public required User User { get; set; }
    public required ICollection<InventoryTransaction> InventoryTransactions { get; set; } = [];
}
