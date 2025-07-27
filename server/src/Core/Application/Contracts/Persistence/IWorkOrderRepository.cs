using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IWorkOrderRepository : IGenericRepository<WorkOrder>
{
    public Task<WorkOrder?> GetWorkOrderWithDetailsAsync(int workOrderId);
}