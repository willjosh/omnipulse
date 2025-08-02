using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class WorkOrderIssueRepository : IWorkOrderIssueRepository
{
    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<WorkOrderIssue> _dbSet;

    public WorkOrderIssueRepository(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<WorkOrderIssue>();
    }

    public Task<WorkOrderIssue> AddAsync(WorkOrderIssue entity)
        => throw new NotImplementedException();

    public void Delete(WorkOrderIssue entity)
        => throw new NotImplementedException();

    public Task<bool> ExistsAsync(int id)
        => throw new NotImplementedException();

    public Task<int> SaveChangesAsync()
        => throw new NotImplementedException();

    public Task<IEnumerable<WorkOrderIssue>> AddRangeAsync(IEnumerable<WorkOrderIssue> entities)
        => throw new NotImplementedException();

    public async Task DeleteByWorkOrderIdAsync(int workOrderId)
    {
        var issues = await _dbSet.Where(i => i.WorkOrderID == workOrderId).ToListAsync();
        if (issues.Count == 0)
        {
            _dbSet.RemoveRange(issues);
        }
    }

    public async Task<IEnumerable<WorkOrderIssue>> GetByWorkOrderIDAsync(int workOrderId)
    {
        return await _dbSet.Where(i => i.WorkOrderID == workOrderId).ToListAsync();
    }

    public async Task<IEnumerable<WorkOrderIssue>> GetByWorkOrderIDsAsync(IEnumerable<int> workOrderIds)
    {
        return await _dbSet.Where(i => workOrderIds.Contains(i.WorkOrderID)).ToListAsync();
    }
}