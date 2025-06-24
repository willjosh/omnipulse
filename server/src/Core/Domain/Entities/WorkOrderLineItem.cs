using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class WorkOrderLineItem : BaseEntity
{
    public required LineItemTypeEnum ItemType { get; set; }
    public string? Description { get; set; }
    public required int Quantity { get; set; }
    public required double UnitCost { get; set; }
    public double TotalCost => Quantity * UnitCost;
    public double? LaborHours { get; set; }

    // Navigation properties
    public required WorkOrder WorkOrder { get; set; }
    public required InventoryItem InventoryItem { get; set; }
    // TODO: Service Task Navigation property
}
