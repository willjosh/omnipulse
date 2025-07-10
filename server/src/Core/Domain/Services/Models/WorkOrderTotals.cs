using System;

namespace Domain.Services.Models;

public class WorkOrderTotals
{
    public decimal TotalLaborCost { get; set; }
    public decimal TotalItemCost { get; set; }
    public decimal TotalCost { get; set; } 
}