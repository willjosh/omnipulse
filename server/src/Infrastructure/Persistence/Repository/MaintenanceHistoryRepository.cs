using Application.Contracts.Persistence;
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
            .Include(mh => mh.WorkOrder)
                .ThenInclude(wo => wo.Vehicle)
            .Include(mh => mh.WorkOrder)
                .ThenInclude(wo => wo.User)
            .Include(mh => mh.WorkOrder)
                .ThenInclude(wo => wo.WorkOrderLineItems)
                    .ThenInclude(wol => wol.ServiceTask);

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

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

    private static IQueryable<MaintenanceHistory> ApplySearchFilter(IQueryable<MaintenanceHistory> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(mh =>
            EF.Functions.Like(mh.WorkOrder.Vehicle.Name, searchPattern) ||
            EF.Functions.Like(mh.WorkOrder.Title, searchPattern) ||
            mh.WorkOrderID.ToString().Contains(searchText) ||
            mh.WorkOrder.WorkOrderLineItems.Any(wol => EF.Functions.Like(wol.ServiceTask.Name, searchPattern)) ||
            EF.Functions.Like(mh.WorkOrder.User.FirstName, searchPattern) ||
            EF.Functions.Like(mh.WorkOrder.User.LastName, searchPattern)
        );
    }

    private IQueryable<MaintenanceHistory> ApplySorting(IQueryable<MaintenanceHistory> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "servicedate" => sortDescending ? query.OrderByDescending(mh => mh.ServiceDate) : query.OrderBy(mh => mh.ServiceDate),
            "workorderid" => sortDescending ? query.OrderByDescending(mh => mh.WorkOrderID) : query.OrderBy(mh => mh.WorkOrderID),
            "mileageatservice" => sortDescending ? query.OrderByDescending(mh => mh.MileageAtService) : query.OrderBy(mh => mh.MileageAtService),
            "createdat" => sortDescending ? query.OrderByDescending(mh => mh.CreatedAt) : query.OrderBy(mh => mh.CreatedAt),
            _ => query.OrderBy(mh => mh.ID)
        };
    }
}