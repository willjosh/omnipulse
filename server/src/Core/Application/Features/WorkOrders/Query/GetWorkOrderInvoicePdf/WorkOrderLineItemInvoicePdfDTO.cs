namespace Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

public class WorkOrderLineItemInvoicePdfDTO
{
    public string? InventoryItemName { get; set; }
    public string? ServiceTaskName { get; set; }
    public decimal? LaborTotal { get; set; }
    public decimal? ItemTotal { get; set; }
    public decimal? SubTotal { get; set; }
}