using System;

using Application.Contracts.Persistence;
using Application.Models;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class WorkOrderRepository : GenericRepository<WorkOrder>, IWorkOrderRepository
{
    public WorkOrderRepository(OmnipulseDatabaseContext context) : base(context)
    {
    }

    public Task<int> GetAllCreatedWorkOrdersCountAsync()
    {
        return _dbSet.CountAsync(wo => wo.Status == WorkOrderStatusEnum.CREATED);
    }

    public Task<int> GetAllInProgressWorkOrdersCountAsync()
    {
        return _dbSet.CountAsync(wo => wo.Status == WorkOrderStatusEnum.IN_PROGRESS);
    }

    public async Task<PagedResult<WorkOrder>> GetAllWorkOrderPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        query = query
            .Include(wo => wo.Vehicle)
            .ThenInclude(v => v.VehicleGroup)
            .Include(wo => wo.WorkOrderLineItems)
            .Include(wo => wo.User);

        if (!string.IsNullOrWhiteSpace(parameters.Search))
        {
            var search = parameters.Search.ToLowerInvariant();

            // look for matches
            query = query.Where(wo =>
                wo.Title.ToLowerInvariant().Contains(search) ||
                (wo.Description != null && wo.Description.ToLowerInvariant().Contains(search)))
                .Where(wo =>
                    (wo.User.UserName != null && wo.User.UserName.ToLowerInvariant().Contains(search)) ||
                    wo.Vehicle.Name.ToLowerInvariant().Contains(search)
                );
        }

        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        // get the total count before pagination
        var totalCount = await query.CountAsync();

        // apply pagination
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<WorkOrder>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    public async Task<WorkOrder?> GetWorkOrderWithDetailsAsync(int workOrderId)
    {
        return await _dbSet
            .Include(wo => wo.Vehicle)
            .ThenInclude(v => v.VehicleGroup)
            .Include(wo => wo.User)
            .Include(wo => wo.WorkOrderLineItems)
            .FirstOrDefaultAsync(wo => wo.ID == workOrderId);
    }

    private IQueryable<WorkOrder> ApplySorting(IQueryable<WorkOrder> query, string? sortBy, bool sortDescending)
    {
        return sortBy?.ToLowerInvariant() switch
        {
            "id" => sortDescending ?
                query.OrderByDescending(wo => wo.ID) :
                query.OrderBy(wo => wo.ID),
            "status" => sortDescending ?
                query.OrderByDescending(wo => wo.Status) :
                query.OrderBy(wo => wo.Status),
            "workordertype" => sortDescending ?
                query.OrderByDescending(wo => wo.WorkOrderType) :
                query.OrderBy(wo => wo.WorkOrderType),
            "priority" => sortDescending ?
                query.OrderByDescending(wo => wo.PriorityLevel) :
                query.OrderBy(wo => wo.PriorityLevel),
            "startodometer" => sortDescending ?
                query.OrderByDescending(wo => wo.StartOdometer) :
                query.OrderBy(wo => wo.StartOdometer),
            "scheduledstartdate" => sortDescending ?
                query.OrderByDescending(wo => wo.ScheduledStartDate) :
                query.OrderBy(wo => wo.ScheduledStartDate),
            "actualstartdate" => sortDescending ?
                query.OrderByDescending(wo => wo.ActualStartDate) :
                query.OrderBy(wo => wo.ActualStartDate),
            _ => query.OrderBy(wo => wo.ID) // Default sorting by ID
        };
    }
}