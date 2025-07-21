using Application.Contracts.Persistence;

using Domain.Entities;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class WorkOrderIssueRepository : IWorkOrderIssueRepository
{
    private readonly OmnipulseDatabaseContext _dbContext;
    public WorkOrderIssueRepository(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
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
}