using Application.Contracts.Persistence;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InspectionPassFailItemRepository : IInspectionPassFailItemRepository
{
    private readonly OmnipulseDatabaseContext _context;
    protected readonly DbSet<InspectionPassFailItem> _dbSet;

    public InspectionPassFailItemRepository(OmnipulseDatabaseContext context)
    {
        _context = context;
        _dbSet = context.Set<InspectionPassFailItem>();
    }

    public async Task<InspectionPassFailItem?> GetByCompositeKeyAsync(int inspectionId, int inspectionFormItemId)
    {
        return await _dbSet.FirstOrDefaultAsync(x => x.InspectionID == inspectionId && x.InspectionFormItemID == inspectionFormItemId);
    }

    public async Task<IReadOnlyList<InspectionPassFailItem>> GetAllAsync()
    {
        return await _dbSet.ToListAsync();
    }

    public async Task<InspectionPassFailItem> AddAsync(InspectionPassFailItem entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task<IEnumerable<InspectionPassFailItem>> AddRangeAsync(IEnumerable<InspectionPassFailItem> entities)
    {
        await _context.AddRangeAsync(entities);
        return entities;
    }

    public void Update(InspectionPassFailItem entity)
    {
        _dbSet.Update(entity);
    }

    public void Delete(InspectionPassFailItem entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task DeleteByCompositeKeyAsync(int inspectionId, int inspectionFormItemId)
    {
        var entity = await GetByCompositeKeyAsync(inspectionId, inspectionFormItemId);
        if (entity != null)
        {
            _dbSet.Remove(entity);
        }
    }

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task<IReadOnlyList<InspectionPassFailItem>> GetByInspectionIdAsync(int inspectionId)
    {
        return await _dbSet
            .Where(x => x.InspectionID == inspectionId)
            .Include(x => x.InspectionFormItem)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InspectionPassFailItem>> GetByInspectionFormItemIdAsync(int inspectionFormItemId)
    {
        return await _dbSet
            .Where(x => x.InspectionFormItemID == inspectionFormItemId)
            .Include(x => x.Inspection)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InspectionPassFailItem>> GetFailedItemsByInspectionIdAsync(int inspectionId)
    {
        return await _dbSet
            .Where(x => x.InspectionID == inspectionId && !x.Passed)
            .Include(x => x.InspectionFormItem)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InspectionPassFailItem>> GetPassedItemsByInspectionIdAsync(int inspectionId)
    {
        return await _dbSet
            .Where(x => x.InspectionID == inspectionId && x.Passed)
            .Include(x => x.InspectionFormItem)
            .ToListAsync();
    }

    public async Task<bool> ExistsAsync(int inspectionId, int inspectionFormItemId)
    {
        return await _dbSet.AnyAsync(x => x.InspectionID == inspectionId && x.InspectionFormItemID == inspectionFormItemId);
    }

    public async Task<bool> AllExistForInspectionAsync(int inspectionId, IEnumerable<int> inspectionFormItemIds)
    {
        var itemIdsList = inspectionFormItemIds.ToList();
        if (itemIdsList.Count == 0) return true;

        var existingCount = await _dbSet
            .CountAsync(x => x.InspectionID == inspectionId && itemIdsList.Contains(x.InspectionFormItemID));

        return existingCount == itemIdsList.Count;
    }

    public async Task<int> CountByInspectionIdAsync(int inspectionId)
    {
        return await _dbSet.CountAsync(x => x.InspectionID == inspectionId);
    }

    public async Task<int> CountFailedByInspectionIdAsync(int inspectionId)
    {
        return await _dbSet.CountAsync(x => x.InspectionID == inspectionId && !x.Passed);
    }

    public async Task<int> CountPassedByInspectionIdAsync(int inspectionId)
    {
        return await _dbSet.CountAsync(x => x.InspectionID == inspectionId && x.Passed);
    }

    public async Task<double> GetPassRateByInspectionIdAsync(int inspectionId)
    {
        var totalCount = await CountByInspectionIdAsync(inspectionId);
        if (totalCount == 0) return 0.0;

        var passedCount = await CountPassedByInspectionIdAsync(inspectionId);
        return (double)passedCount / totalCount;
    }
}