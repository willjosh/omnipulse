using System;

using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class WorkOrderLineItemRepository : GenericRepository<WorkOrderLineItem>, IWorkOrderLineItemRepository
{
    public WorkOrderLineItemRepository(OmnipulseDatabaseContext context) : base(context)
    {
    }

    public async Task<List<WorkOrderLineItem>> GetByWorkOrderIdAsync(int workOrderID)
    {
        return await _dbSet
            .Include(woli => woli.User)
            .Include(woli => woli.InventoryItem)
            .Include(woli => woli.ServiceTask)
            .Where(woli => woli.WorkOrderID == workOrderID)
            .ToListAsync();
    }

    public async Task<List<WorkOrderLineItem>> GetByWorkOrderIdsAsync(List<int> workOrderIds)
    {
        if (workOrderIds == null || !workOrderIds.Any())
            return new List<WorkOrderLineItem>();
         
        return await _dbSet
            .Include(woli => woli.User)
            .Include(woli => woli.InventoryItem)
            .Include(woli => woli.ServiceTask)
            .Where(woli => workOrderIds.Contains(woli.WorkOrderID))
            .ToListAsync();
    }
}
