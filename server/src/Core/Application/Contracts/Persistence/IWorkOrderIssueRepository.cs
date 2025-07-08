using System;

using Domain.Entities;

namespace Application.Contracts.Persistence;

public interface IWorkOrderIssueRepository 
{
   Task<WorkOrderIssue> AddAsync(WorkOrderIssue entity); 
   void Delete(WorkOrderIssue entity);
   Task<bool> ExistsAsync(int id);
   Task<int> SaveChangesAsync();
   Task<IEnumerable<WorkOrderIssue>> AddRangeAsync(IEnumerable<WorkOrderIssue> entities);
}
