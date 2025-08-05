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

    public async Task<WorkOrderIssue> AddAsync(WorkOrderIssue entity)
    {
        entity.AssignedDate = DateTime.UtcNow;

        var result = await _dbSet.AddAsync(entity);
        await _dbContext.SaveChangesAsync();

        return result.Entity;
    }

    public void Delete(WorkOrderIssue entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<bool> ExistsAsync(int workOrderID)
    {
        return await _dbSet.AnyAsync(e => e.WorkOrderID == workOrderID);
    }

    public async Task<bool> ExistsAsync(int workOrderId, int issueId)
    {
        return await _dbSet.AnyAsync(e => e.WorkOrderID == workOrderId && e.IssueID == issueId);
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _dbContext.SaveChangesAsync();
    }

    public async Task<IEnumerable<WorkOrderIssue>> AddRangeAsync(IEnumerable<WorkOrderIssue> entities)
    {
        var workOrderIssues = entities.ToList();
        var utcNow = DateTime.UtcNow;

        foreach (var entity in workOrderIssues)
        {
            entity.AssignedDate = utcNow;
        }

        await _dbSet.AddRangeAsync(workOrderIssues);

        return workOrderIssues;
    }

    public async Task DeleteByWorkOrderIdAsync(int workOrderId)
    {
        var issues = await _dbSet.Where(i => i.WorkOrderID == workOrderId).ToListAsync();
        if (issues.Count > 0)
        {
            _dbSet.RemoveRange(issues);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<WorkOrderIssue>> GetByWorkOrderIDAsync(int workOrderId)
    {
        return await _dbSet
            .Where(i => i.WorkOrderID == workOrderId)
            .Include(i => i.WorkOrder)
            .Include(i => i.Issue)
            .ToListAsync();
    }

    public async Task<IEnumerable<WorkOrderIssue>> GetByWorkOrderIDsAsync(IEnumerable<int> workOrderIds)
    {
        return await _dbSet
            .Where(i => workOrderIds.Contains(i.WorkOrderID))
            .Include(i => i.WorkOrder)
            .Include(i => i.Issue)
            .ToListAsync();
    }

    public async Task<WorkOrderIssue?> GetByIdAsync(int workOrderId, int issueId)
    {
        return await _dbSet
            .Include(i => i.WorkOrder)
            .Include(i => i.Issue)
            .FirstOrDefaultAsync(i => i.WorkOrderID == workOrderId && i.IssueID == issueId);
    }

    public void Update(WorkOrderIssue entity)
    {
        entity.AssignedDate = DateTime.UtcNow;
        _dbSet.Update(entity);
    }

    public async Task<IEnumerable<WorkOrderIssue>> GetAllAsync()
    {
        return await _dbSet
            .Include(i => i.WorkOrder)
            .Include(i => i.Issue)
            .ToListAsync();
    }

    public async Task<bool> ExistsByIssueIdAsync(int issueId)
    {
        return await _dbSet.AnyAsync(e => e.IssueID == issueId);
    }

    public async Task<IEnumerable<WorkOrderIssue>> GetByIssueIdAsync(int issueId)
    {
        return await _dbSet
            .Where(i => i.IssueID == issueId)
            .Include(i => i.WorkOrder)
            .Include(i => i.Issue)
            .ToListAsync();
    }

    public async Task DeleteByIssueIdAsync(int issueId)
    {
        var workOrderIssues = await _dbSet.Where(i => i.IssueID == issueId).ToListAsync();
        if (workOrderIssues.Count > 0)
        {
            _dbSet.RemoveRange(workOrderIssues);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteSpecificAsync(int workOrderId, int issueId)
    {
        var workOrderIssue = await _dbSet
            .FirstOrDefaultAsync(i => i.WorkOrderID == workOrderId && i.IssueID == issueId);

        if (workOrderIssue != null)
        {
            _dbSet.Remove(workOrderIssue);
            await _dbContext.SaveChangesAsync();
        }
    }

}