using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class WorkOrderRepository : GenericRepository<WorkOrder>, IWorkOrderRepository
{
    public WorkOrderRepository(OmnipulseDatabaseContext context) : base(context) { }

    public Task<WorkOrder?> GetWorkOrderWithDetailsAsync(int workOrderId)
        => throw new NotImplementedException();
}