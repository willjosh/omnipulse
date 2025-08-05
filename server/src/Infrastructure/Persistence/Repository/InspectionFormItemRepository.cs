using Application.Contracts.Persistence;
using Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InspectionFormItemRepository : GenericRepository<InspectionFormItem>, IInspectionFormItemRepository
{
    public InspectionFormItemRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<PagedResult<InspectionFormItem>> GetAllInspectionFormItemsPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<InspectionFormItem>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<InspectionFormItem> ApplySearchFilter(IQueryable<InspectionFormItem> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(i =>
            EF.Functions.Like(i.ItemLabel, searchPattern) ||
            EF.Functions.Like(i.ItemDescription ?? string.Empty, searchPattern) ||
            EF.Functions.Like(i.ItemInstructions ?? string.Empty, searchPattern)
        );
    }

    private static IQueryable<InspectionFormItem> ApplySorting(IQueryable<InspectionFormItem> query, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderBy(i => i.ItemLabel); // Default sort
        }

        var sortByLower = sortBy.ToLower();
        return sortByLower switch
        {
            "itemlabel" => sortDescending
                ? query.OrderByDescending(i => i.ItemLabel)
                : query.OrderBy(i => i.ItemLabel),
            "isrequired" => sortDescending
                ? query.OrderByDescending(i => i.IsRequired)
                : query.OrderBy(i => i.IsRequired),
            "id" => sortDescending
                ? query.OrderByDescending(i => i.ID)
                : query.OrderBy(i => i.ID),
            "inspectionformitemtypeenum" => sortDescending
                ? query.OrderByDescending(i => i.InspectionFormItemTypeEnum)
                : query.OrderBy(i => i.InspectionFormItemTypeEnum),
            "createdat" => sortDescending
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt),
            "updatedat" => sortDescending
                ? query.OrderByDescending(i => i.UpdatedAt)
                : query.OrderBy(i => i.UpdatedAt),
            _ => query.OrderBy(i => i.ItemLabel) // Default sort for unrecognized fields
        };
    }

    public async Task<IReadOnlyList<InspectionFormItem>> GetItemsByFormIdAsync(int inspectionFormId)
    {
        return await _dbSet
            .Where(i => i.InspectionFormID == inspectionFormId)
            .OrderBy(i => i.ItemLabel)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InspectionFormItem>> GetRequiredItemsByFormIdAsync(int inspectionFormId)
    {
        return await _dbSet
            .Where(i => i.InspectionFormID == inspectionFormId && i.IsRequired)
            .OrderBy(i => i.ItemLabel)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<InspectionFormItem>> GetItemsByTypeAsync(InspectionFormItemTypeEnum itemType)
    {
        return await _dbSet
            .Where(i => i.InspectionFormItemTypeEnum == itemType)
            .OrderBy(i => i.ItemLabel)
            .ToListAsync();
    }

    public async Task<int> CountItemsByFormIdAsync(int inspectionFormId)
    {
        return await _dbSet.CountAsync(i => i.InspectionFormID == inspectionFormId);
    }

    public async Task<int> CountRequiredItemsByFormIdAsync(int inspectionFormId)
    {
        return await _dbSet.CountAsync(i => i.InspectionFormID == inspectionFormId && i.IsRequired);
    }

    public async Task<List<InspectionFormItem>> GetAllByInspectionFormIdAsync(int inspectionFormId)
    {
        return await _dbSet
            .Where(item => item.InspectionFormID == inspectionFormId)
            .OrderBy(item => item.CreatedAt)
            .ToListAsync();
    }

    public async Task<PagedResult<InspectionFormItem>> GetAllByInspectionFormIdPagedAsync(int inspectionFormId, PaginationParameters parameters)
    {
        var query = _dbSet.Where(i => i.InspectionFormID == inspectionFormId);

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Include(i => i.InspectionForm)
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<InspectionFormItem>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<InspectionFormItem?> GetInspectionFormItemDetailsAsync(int inspectionFormItemId)
    {
        return await _dbSet.Where(i => i.ID == inspectionFormItemId)
            .Include(i => i.InspectionForm)
            .FirstOrDefaultAsync();
    }
}