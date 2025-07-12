using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class XrefServiceScheduleServiceTaskRepository : IXrefServiceScheduleServiceTaskRepository
{
    private readonly OmnipulseDatabaseContext _dbContext;
    private readonly DbSet<XrefServiceScheduleServiceTask> _dbSet;

    public XrefServiceScheduleServiceTaskRepository(OmnipulseDatabaseContext context)
    {
        _dbContext = context;
        _dbSet = _dbContext.Set<XrefServiceScheduleServiceTask>();
    }

    public async Task<List<XrefServiceScheduleServiceTask>> GetByServiceScheduleIdAsync(int serviceScheduleId)
    {
        return await _dbSet.Where(x => x.ServiceScheduleID == serviceScheduleId).ToListAsync();
    }

    public async Task<List<XrefServiceScheduleServiceTask>> GetByServiceScheduleIdsAsync(IEnumerable<int> serviceScheduleIds)
    {
        return await _dbSet.Where(x => serviceScheduleIds.Contains(x.ServiceScheduleID)).ToListAsync();
    }

    public async Task<List<XrefServiceScheduleServiceTask>> GetByServiceTaskIdAsync(int serviceTaskId)
    {
        return await _dbSet.Where(x => x.ServiceTaskID == serviceTaskId).ToListAsync();
    }

    public async Task<List<XrefServiceScheduleServiceTask>> GetByServiceTaskIdsAsync(IEnumerable<int> serviceTaskIds)
    {
        return await _dbSet.Where(x => serviceTaskIds.Contains(x.ServiceTaskID)).ToListAsync();
    }

    public async Task<bool> ExistsAsync(int serviceScheduleId, int serviceTaskId)
    {
        return await _dbSet.AnyAsync(x => x.ServiceScheduleID == serviceScheduleId && x.ServiceTaskID == serviceTaskId);
    }

    public async Task AddAsync(XrefServiceScheduleServiceTask xref)
    {
        await _dbSet.AddAsync(xref);
        await _dbContext.SaveChangesAsync();
    }

    public async Task AddRangeAsync(IEnumerable<XrefServiceScheduleServiceTask> xrefs)
    {
        await _dbSet.AddRangeAsync(xrefs);
        await _dbContext.SaveChangesAsync();
    }

    public async Task RemoveAsync(int serviceScheduleId, int serviceTaskId)
    {
        var entity = await _dbSet.FirstOrDefaultAsync(x => x.ServiceScheduleID == serviceScheduleId && x.ServiceTaskID == serviceTaskId);
        if (entity != null)
        {
            _dbSet.Remove(entity);
            await _dbContext.SaveChangesAsync();
        }
    }

    public async Task RemoveAllForScheduleAsync(int serviceScheduleId)
    {
        var xrefs = await _dbSet.Where(x => x.ServiceScheduleID == serviceScheduleId).ToListAsync();
        if (xrefs.Count != 0)
        {
            _dbSet.RemoveRange(xrefs);
            await _dbContext.SaveChangesAsync();
        }
    }
}