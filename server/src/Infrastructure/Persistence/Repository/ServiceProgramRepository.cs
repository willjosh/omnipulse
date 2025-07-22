using Application.Contracts.Persistence;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceProgramRepository : GenericRepository<ServiceProgram>, IServiceProgramRepository
{
    public ServiceProgramRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<bool> IsNameUniqueAsync(string name)
    {
        return !await _dbSet.AnyAsync(sp => sp.Name == name);
    }

    public async Task<PagedResult<ServiceProgram>> GetAllServiceProgramsPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Include ServiceSchedules and XrefServiceProgramVehicles for counting
        query = query
            .Include(sp => sp.ServiceSchedules)
            .Include(sp => sp.XrefServiceProgramVehicles);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();

            // Look for matches in name and description
            query = query.Where(sp =>
                sp.Name.ToLowerInvariant().Contains(search) ||
                (sp.Description != null && sp.Description.ToLowerInvariant().Contains(search))
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

        return new PagedResult<ServiceProgram>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    // Helper method to apply sorting based on parameters
    private static IQueryable<ServiceProgram> ApplySorting(IQueryable<ServiceProgram> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending ?
                query.OrderByDescending(sp => sp.Name) :
                query.OrderBy(sp => sp.Name),
            "description" => sortDescending ?
                query.OrderByDescending(sp => sp.Description) :
                query.OrderBy(sp => sp.Description),
            "isactive" => sortDescending ?
                query.OrderByDescending(sp => sp.IsActive) :
                query.OrderBy(sp => sp.IsActive),
            "createdat" => sortDescending ?
                query.OrderByDescending(sp => sp.CreatedAt) :
                query.OrderBy(sp => sp.CreatedAt),
            "updatedat" => sortDescending ?
                query.OrderByDescending(sp => sp.UpdatedAt) :
                query.OrderBy(sp => sp.UpdatedAt),
            _ => query.OrderBy(sp => sp.ID) // Default sorting by ID
        };
    }
}