namespace Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

public class GetWorkOrderInvoicePdfDTO
{
    public required int WorkOrderID { get; set; }
    public string? WorkOrderTitle { get; set; }
    public string? WorkOrderDescription { get; set; }
    public string? WorkOrderType { get; set; }
    public string? WorkOrderPriorityLevel { get; set; }
    public string? WorkOrderStatus { get; set; }
    public string? InvoiceNumber { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string? VehicleName { get; set; }
    public string? VehicleMake { get; set; }
    public string? VehicleModel { get; set; }
    public string? VehicleVIN { get; set; }
    public string? VehicleLicensePlate { get; set; }
    public string? VehicleType { get; set; }
    public string? VehicleGroupName { get; set; }
    public double VehicleOdometer { get; set; }
    public string? IssuedByUserName { get; set; }
    public string? AssignedToUserName { get; set; }
    public DateTime? ExpectedCompletionDate { get; set; }
    public DateTime? ActualCompletionDate { get; set; }
    public DateTime? ScheduledStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public List<WorkOrderLineItemInvoicePdfDTO> WorkOrderLineItems { get; set; } = [];
    public decimal TotalLabourCost { get; set; }
    public decimal TotalItemCost { get; set; }
    public decimal TotalCost { get; set; }
}