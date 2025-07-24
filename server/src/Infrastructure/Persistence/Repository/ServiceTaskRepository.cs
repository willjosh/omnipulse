using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceTaskRepository : GenericRepository<ServiceTask>, IServiceTaskRepository
{
    public ServiceTaskRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> DoesNameExistAsync(string name)
    {
        return await _dbSet.AnyAsync(st => st.Name == name);
    }

    public async Task<bool> DoesNameExistExcludingIdAsync(string name, int excludeServiceTaskId)
    {
        return await _dbSet.AnyAsync(st => st.Name == name && st.ID != excludeServiceTaskId);
    }

    public async Task<PagedResult<ServiceTask>> GetAllServiceTasksPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // Get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<ServiceTask>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<ServiceTask> ApplySearchFilter(IQueryable<ServiceTask> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(st =>
            EF.Functions.Like(st.Name, searchPattern) ||
            EF.Functions.Like(st.Description ?? string.Empty, searchPattern) ||
            EF.Functions.Like(st.Category.ToString(), searchPattern)
        );
    }

    // Helper method to apply sorting based on parameters
    private IQueryable<ServiceTask> ApplySorting(IQueryable<ServiceTask> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending ?
                query.OrderByDescending(st => st.Name) :
                query.OrderBy(st => st.Name),
            "category" => sortDescending ?
                query.OrderByDescending(st => st.Category) :
                query.OrderBy(st => st.Category),
            "estimatedlabourhours" => sortDescending ?
                query.OrderByDescending(st => st.EstimatedLabourHours) :
                query.OrderBy(st => st.EstimatedLabourHours),
            "estimatedcost" => sortDescending ?
                query.OrderByDescending(st => st.EstimatedCost) :
                query.OrderBy(st => st.EstimatedCost),
            "isactive" => sortDescending ?
                query.OrderByDescending(st => st.IsActive) :
                query.OrderBy(st => st.IsActive),
            "createdat" => sortDescending ?
                query.OrderByDescending(st => st.CreatedAt) :
                query.OrderBy(st => st.CreatedAt),
            _ => query.OrderBy(st => st.ID) // Default sorting by ID
        };
    }
}