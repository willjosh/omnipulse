using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class InspectionFormRepository : GenericRepository<InspectionForm>, IInspectionFormRepository
{
    public InspectionFormRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> IsTitleUniqueAsync(string title)
    {
        return !await _dbSet.AnyAsync(x => x.Title.ToLower() == title.ToLower());
    }

    public async Task<bool> IsTitleUniqueAsync(string title, int excludeId)
    {
        return !await _dbSet.AnyAsync(x => x.Title.ToLower() == title.ToLower() && x.ID != excludeId);
    }

    public async Task<PagedResult<InspectionForm>> GetAllInspectionFormsPagedAsync(PaginationParameters parameters)
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

        return new PagedResult<InspectionForm>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<InspectionForm> ApplySearchFilter(IQueryable<InspectionForm> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(i =>
            EF.Functions.Like(i.Title, searchPattern) ||
            EF.Functions.Like(i.Description ?? string.Empty, searchPattern)
        );
    }

    private static IQueryable<InspectionForm> ApplySorting(IQueryable<InspectionForm> query, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderBy(i => i.Title); // Default sort
        }

        var sortByLower = sortBy.ToLower();
        return sortByLower switch
        {
            "title" => sortDescending
                ? query.OrderByDescending(i => i.Title)
                : query.OrderBy(i => i.Title),
            "isactive" => sortDescending
                ? query.OrderByDescending(i => i.IsActive)
                : query.OrderBy(i => i.IsActive),
            "createdat" => sortDescending
                ? query.OrderByDescending(i => i.CreatedAt)
                : query.OrderBy(i => i.CreatedAt),
            "updatedat" => sortDescending
                ? query.OrderByDescending(i => i.UpdatedAt)
                : query.OrderBy(i => i.UpdatedAt),
            _ => query.OrderBy(i => i.Title) // Default sort for unrecognized fields
        };
    }

    public async Task<InspectionForm?> GetInspectionFormWithItemsAsync(int inspectionFormId)
    {
        return await _dbSet
            .Include(i => i.InspectionFormItems)
            .FirstOrDefaultAsync(i => i.ID == inspectionFormId);
    }
}