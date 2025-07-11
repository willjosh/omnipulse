using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class MaintenanceHistoryRepository : GenericRepository<MaintenanceHistory>, IMaintenanceHistoryRepository
{
    public MaintenanceHistoryRepository(OmnipulseDatabaseContext context) : base(context) { }

    public async Task<PagedResult<MaintenanceHistory>> GetAllMaintenanceHistoriesPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        query = query
            .Include(mh => mh.WorkOrder);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = $"%{parameters.Search}%";
            query = query.Where(mh =>
                EF.Functions.Like(mh.WorkOrder.Title, search) ||
                mh.WorkOrderID.ToString().Contains(parameters.Search) ||
                (mh.Description != null && EF.Functions.Like(mh.Description, search))
            );
        }

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        var totalCount = await query.CountAsync();

        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<MaintenanceHistory>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private IQueryable<MaintenanceHistory> ApplySorting(IQueryable<MaintenanceHistory> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "servicedate" => sortDescending ? query.OrderByDescending(mh => mh.ServiceDate) : query.OrderBy(mh => mh.ServiceDate),
            "workorderid" => sortDescending ? query.OrderByDescending(mh => mh.WorkOrderID) : query.OrderBy(mh => mh.WorkOrderID),
            "mileageatservice" => sortDescending ? query.OrderByDescending(mh => mh.MileageAtService) : query.OrderBy(mh => mh.MileageAtService),
            "cost" => sortDescending ? query.OrderByDescending(mh => mh.Cost) : query.OrderBy(mh => mh.Cost),
            "labourhours" => sortDescending ? query.OrderByDescending(mh => mh.LabourHours) : query.OrderBy(mh => mh.LabourHours),
            "createdat" => sortDescending ? query.OrderByDescending(mh => mh.CreatedAt) : query.OrderBy(mh => mh.CreatedAt),
            _ => query.OrderBy(mh => mh.ID)
        };
    }
}