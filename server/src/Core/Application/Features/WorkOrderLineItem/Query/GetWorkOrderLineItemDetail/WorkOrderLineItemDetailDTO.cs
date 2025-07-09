using System;

using Domain.Entities.Enums;

namespace Application.Features.WorkOrderLineItem.Query.GetWorkOrderLineItemDetail;

public class WorkOrderLineItemDetailDTO
{
    // Work Order Line Item
    public int ID { get; set; }
    public required int WorkOrderID { get; set; }    
    public required LineItemTypeEnum ItemType { get; set; }
    public required int Quantity { get; set; }
    public string? Description { get; set; }

    // Inventory Item Fields 
    public int? InventoryItemID { get; set; }
    public required string InventoryItemName { get; set; }
    
    // Assigned User
    public required string AssignedToUserID { get; set; }
    public required string AssignedToUserName { get; set; }

    // Cost
    public required decimal SubTotal { get; set; } 
    public required decimal LaborCost { get; set; }
    public required decimal ItemCost { get; set; }
    
    // Service Task 
    public required int ServiceTaskID { get; set; }
    public required string ServiceTaskName { get; set; }
}
