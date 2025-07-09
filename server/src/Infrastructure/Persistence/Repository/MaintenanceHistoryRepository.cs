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
            .Include(mh => mh.Vehicle)
            .Include(mh => mh.WorkOrder)
            .Include(mh => mh.ServiceTask)
            .Include(mh => mh.User);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();
            query = query.Where(mh =>
                mh.Vehicle.Name.ToLowerInvariant().Contains(search) ||
                mh.WorkOrder.Title.ToLowerInvariant().Contains(search) ||
                mh.WorkOrderID.ToString().Contains(search) ||
                mh.ServiceTask.Name.ToLowerInvariant().Contains(search) ||
                mh.User.FirstName.ToLowerInvariant().Contains(search) ||
                mh.User.LastName.ToLowerInvariant().Contains(search) ||
                (mh.Description != null && mh.Description.ToLowerInvariant().Contains(search))
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
            "vehicleid" => sortDescending ? query.OrderByDescending(mh => mh.VehicleID) : query.OrderBy(mh => mh.VehicleID),
            "workorderid" => sortDescending ? query.OrderByDescending(mh => mh.WorkOrderID) : query.OrderBy(mh => mh.WorkOrderID),
            "servicetaskid" => sortDescending ? query.OrderByDescending(mh => mh.ServiceTaskID) : query.OrderBy(mh => mh.ServiceTaskID),
            "technicianid" => sortDescending ? query.OrderByDescending(mh => mh.TechnicianID) : query.OrderBy(mh => mh.TechnicianID),
            "mileageatservice" => sortDescending ? query.OrderByDescending(mh => mh.MileageAtService) : query.OrderBy(mh => mh.MileageAtService),
            "cost" => sortDescending ? query.OrderByDescending(mh => mh.Cost) : query.OrderBy(mh => mh.Cost),
            "labourhours" => sortDescending ? query.OrderByDescending(mh => mh.LabourHours) : query.OrderBy(mh => mh.LabourHours),
            "createdat" => sortDescending ? query.OrderByDescending(mh => mh.CreatedAt) : query.OrderBy(mh => mh.CreatedAt),
            _ => query.OrderBy(mh => mh.ID)
        };
    }
}