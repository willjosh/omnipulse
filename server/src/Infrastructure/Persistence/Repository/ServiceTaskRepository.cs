using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceTaskRepository : GenericRepository<ServiceTask>, IServiceTaskRepository
{
    public ServiceTaskRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        return await _dbSet.AnyAsync(st => st.Name == name);
    }

    public async Task<PagedResult<ServiceTask>> GetAllServiceTasksPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Apply filtering if search is provided
        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();

            query = query.Where(st =>
                st.Name.ToLowerInvariant().Contains(search) ||
                (st.Description != null && st.Description.ToLowerInvariant().Contains(search)) ||
                st.Category.ToString().ToLowerInvariant().Contains(search)
            );
        }

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