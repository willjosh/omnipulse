using System;

using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class VehicleGroupRepository : GenericRepository<VehicleGroup>, IVehicleGroupRepository
{
    public VehicleGroupRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<PagedResult<VehicleGroup>> GetAllVehicleGroupsPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsNoTracking().AsQueryable();

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();

            // Look for matches
            query = query.
                Where(vg =>
                    vg.Name.ToLowerInvariant().Contains(search) ||
                    (vg.Description != null && vg.Description.ToLowerInvariant().Contains(search))
                );
        }

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // get total count before pagination
        var totalCount = await query.CountAsync();

        // Apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<VehicleGroup>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private IQueryable<VehicleGroup> ApplySorting(IQueryable<VehicleGroup> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "name" => sortDescending ?
                query.OrderByDescending(vg => vg.Name) :
                query.OrderBy(vg => vg.Name),
            "description" => sortDescending ?
                query.OrderByDescending(vg => vg.Description) :
                query.OrderBy(vg => vg.Description),
            "isactive" => sortDescending ?
                query.OrderByDescending(vg => vg.IsActive) :
                query.OrderBy(vg => vg.IsActive),
            "createdat" => sortDescending ?
                query.OrderByDescending(vg => vg.CreatedAt) :
                query.OrderBy(vg => vg.CreatedAt),
            "updatedat" => sortDescending ?
                query.OrderByDescending(vg => vg.UpdatedAt) :
                query.OrderBy(vg => vg.UpdatedAt),
            _ => query.OrderBy(vg => vg.ID) // Default sorting by ID
        };
    }
}