using System;

using Domain.Entities.Enums;

namespace Domain.Entities;

public class WorkOrderLineItem : BaseEntity
{
    // REQUIRED FIELDS
    public required int WorkOrderID { get; set; }
    public required int ServiceTaskID { get; set; }
    public required LineItemTypeEnum ItemType { get; set; }
    public required int Quantity { get; set; }
    public required decimal TotalCost { get; set; }

    // OPTIONAL FIELDS 
    public int? InventoryItemID { get; set; }
    public string? AssignedToUserID { get; set; }
    public string? Description { get; set; }
    public double? LaborHours { get; set; }

    // PRICING FIELDS 
    public decimal? UnitPrice { get; set; }
    public decimal? HourlyRate { get; set; }

    // Navigation properties
    public required User? User { get; set; }
    public required WorkOrder WorkOrder { get; set; }
    public InventoryItem? InventoryItem { get; set; }
    public required ServiceTask ServiceTask { get; set; }

    public void CalculateTotalCost()
    {
        switch (ItemType)
        {
            case LineItemTypeEnum.LABOR:
                // Labor cost = hours * hourly rate
                TotalCost = (decimal)(LaborHours ?? 0) * (HourlyRate ?? 0);
                break;

            case LineItemTypeEnum.ITEM:
                // Item/Parts cost = quantity * unit price
                TotalCost = Quantity * (UnitPrice ?? 0);
                break;

            case LineItemTypeEnum.BOTH:
                // Both = labor cost + item cost
                var laborCost = (decimal)(LaborHours ?? 0) * (HourlyRate ?? 0);
                var itemCost = Quantity * (UnitPrice ?? 0);
                TotalCost = laborCost + itemCost;
                break;

            default:
                TotalCost = 0;
                break;
        }
    }
}