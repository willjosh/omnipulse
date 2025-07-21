using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class WorkOrderLineItemRepository : GenericRepository<WorkOrderLineItem>, IWorkOrderLineItemRepository
{
    public WorkOrderLineItemRepository(OmnipulseDatabaseContext context) : base(context) { }

    public Task<List<WorkOrderLineItem>> GetByWorkOrderIdAsync(int workOrderId)
        => throw new NotImplementedException();
}