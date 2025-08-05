using Application.Models.PaginationModels;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IWorkOrderRepository : IGenericRepository<WorkOrder>
{
    public Task<WorkOrder?> GetWorkOrderWithDetailsAsync(int workOrderId);
    public Task<PagedResult<WorkOrder>> GetAllWorkOrderPagedAsync(PaginationParameters parameters);
    public Task<int> GetAllCreatedWorkOrdersCountAsync();
    public Task<int> GetAllInProgressWorkOrdersCountAsync();
}