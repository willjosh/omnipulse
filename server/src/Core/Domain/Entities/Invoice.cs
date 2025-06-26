using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class Invoice
{
    public required int WorkOrderID { get; set; }
    public required int GeneratedByUserID { get; set; }
    public required string InvoiceNumber { get; set; }
    public required DateTime InvoiceDate { get; set; }
    public required double TotalAmount { get; set; }
    public required double TaxAmount { get; set; }
    public required InvoiceStatusEnum Status { get; set; }
    public required DateTime? PaymentDate { get; set; }
    public required string PaymentMethod { get; set; }

    // Navigation properties
    public required WorkOrder WorkOrder { get; set; }

    // TODO: Add navigation properties for GeneratedBy
}
