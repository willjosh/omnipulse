using System;

using Domain.Entities.Enums;

namespace Application.Features.WorkOrderLineItem.Models;

public class CreateWorkOrderLineItemDTO
{
    public required int WorkOrderID { get; set; }
    public int? InventoryItemID { get; set; }
    public required int ServiceTaskID { get; set; }
    public string? AssignedToUserID { get; set; } // Optional, can be 0 or null
    public LineItemTypeEnum ItemType { get; set; }
    public string? Description { get; set; }
    public required int Quantity { get; set; }
    public decimal? UnitPrice { get; set; } // Optional, can be 0
    public decimal? HourlyRate { get; set; } // Optional, can be 0
    public double? LaborHours { get; set; } // Optional, can be null    
}