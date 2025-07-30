namespace Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

public class WorkOrderLineItemInvoicePdfDTO
{
    public int ID { get; set; }
    public int WorkOrderID { get; set; }
    public int? ServiceTaskID { get; set; }
    public int? InventoryItemID { get; set; }
    public decimal? LaborHours { get; set; }
    public string? Description { get; set; }
    public string? ItemType { get; set; }
    public decimal? Quantity { get; set; }
    public decimal? UnitPrice { get; set; }
    public decimal? TotalCost { get; set; }
    public decimal? HourlyRate { get; set; }
    public string? ServiceTaskName { get; set; }
}