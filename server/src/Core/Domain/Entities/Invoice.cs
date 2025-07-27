using Domain.Entities.Enums;

namespace Domain.Entities;

public class Invoice
{
    public required int WorkOrderID { get; set; }
    public required string GeneratedByUserID { get; set; }
    public required string InvoiceNumber { get; set; }
    public required DateTime InvoiceDate { get; set; }
    public required decimal TotalAmount { get; set; }
    public required decimal TaxAmount { get; set; }
    public required InvoiceStatusEnum Status { get; set; }
    public required DateTime? PaymentDate { get; set; }
    public required string PaymentMethod { get; set; }

    // Navigation properties
    public required WorkOrder WorkOrder { get; set; }
    public required User User { get; set; }
}