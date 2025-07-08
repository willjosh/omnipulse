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
    public string? Description { get; set; }                  
    public double? LaborHours { get; set; }                   
    
    // PRICING FIELDS 
    public decimal? UnitPrice { get; set; }                
    public decimal? HourlyRate { get; set; }                
    public decimal? ServicePrice { get; set; }               

    // Navigation properties
    public required WorkOrder WorkOrder { get; set; }
    public InventoryItem? InventoryItem { get; set; }         
    public required ServiceTask ServiceTask { get; set; }
}