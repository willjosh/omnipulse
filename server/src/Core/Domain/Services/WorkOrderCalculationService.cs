using Domain.Entities;
using Domain.Services.Models;

namespace Domain.Services;

public static class WorkOrderDomainExtension
{
    public static WorkOrderTotals CalculateTotals(this IEnumerable<WorkOrderLineItem> lineItems)
    {
        var lineItemsList = lineItems.ToList();
        
        // Ensure calculations are complete
        lineItemsList.ForEach(item => item.CalculateTotalCost());

        return new WorkOrderTotals
        {
            TotalLaborCost = lineItemsList.Sum(wo => wo.CalculateLaborCost()),
            TotalItemCost = lineItemsList.Sum(wo => wo.CalculateItemCost()),
            TotalCost = lineItemsList.Sum(wo => wo.TotalCost)
        };        
    }
    public static void EnsureTotalCalculated(this IEnumerable<WorkOrderLineItem> workOrder)
    {
        workOrder.ToList().ForEach(wo => wo.CalculateTotalCost());
    }
}
