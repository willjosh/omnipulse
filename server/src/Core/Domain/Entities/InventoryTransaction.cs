using System;
using System.Diagnostics.CodeAnalysis;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public int InventoryID { get; set; }
    public int MaintenanceHistoryID { get; set; }
    public TransactionTypeEnum TransactionType { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
    public decimal TotalCost { get; set; }
    public required string ReferenceNumber { get; set; }
    public required string Notes { get; set; }
    public int PerformedBy { get; set; }

    // navigation properties
    public required Inventory Inventory { get; set; }
    // TODO Add maintenance history id when maintenance table is created.
}

