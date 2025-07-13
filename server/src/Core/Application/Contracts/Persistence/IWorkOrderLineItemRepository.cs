using System;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IWorkOrderLineItemRepository : IGenericRepository<WorkOrderLineItem>
{
    Task<List<WorkOrderLineItem>> GetByWorkOrderIdAsync(int workOrderId);
}