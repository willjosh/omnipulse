using System;
using System.Diagnostics.CodeAnalysis;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public required int InventoryID { get; set; }
    public required int MaintenanceHistoryID { get; set; }
    public required TransactionTypeEnum TransactionType { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitCost { get; set; }
    public required decimal TotalCost { get; set; }
    public required string ReferenceNumber { get; set; }
    public string? Notes { get; set; }
    public required int PerformedByUserID { get; set; }

    // navigation properties
    public required Inventory Inventory { get; set; }
    public required MaintenanceHistory MaintenanceHistory { get; set; }
}

