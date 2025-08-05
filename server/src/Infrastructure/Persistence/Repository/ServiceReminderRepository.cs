
using Application.Contracts.Persistence;
using Application.Features.ServiceReminders.Query;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using Microsoft.EntityFrameworkCore;

using Persistence.DatabaseContext;

namespace Persistence.Repository;

public class ServiceReminderRepository : GenericRepository<ServiceReminder>, IServiceReminderRepository
{
    private readonly OmnipulseDatabaseContext _dbContext;

    public ServiceReminderRepository(OmnipulseDatabaseContext context) : base(context)
    {
        _dbContext = context;
    }

    // Simple data access method - Repository should only provide raw data
    public async Task<List<ServiceSchedule>> GetActiveServiceSchedulesWithDataAsync()
    {
        return await _dbContext.Set<ServiceSchedule>()
            .Where(ss => ss.IsActive)
            .Include(ss => ss.ServiceProgram)
                .ThenInclude(sp => sp.XrefServiceProgramVehicles)
                .ThenInclude(xspv => xspv.Vehicle)
            .Include(ss => ss.XrefServiceScheduleServiceTasks)
                .ThenInclude(xsst => xsst.ServiceTask)
            .ToListAsync();
    }

    public async Task<ServiceReminder?> GetServiceReminderWithDetailsAsync(int serviceReminderId)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)
            .Include(sr => sr.ServiceProgram)
            .Include(sr => sr.ServiceSchedule)

            .Include(sr => sr.WorkOrder)
            .FirstOrDefaultAsync(sr => sr.ID == serviceReminderId);
    }

    // Query methods by vehicle
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId)
            .OrderBy(sr => sr.Status == ServiceReminderStatusEnum.OVERDUE ? 0 :
                sr.Status == ServiceReminderStatusEnum.DUE_SOON ? 1 : 2)
            .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetOverdueRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId && sr.Status == ServiceReminderStatusEnum.OVERDUE)
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetUpcomingRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId &&
                (sr.Status == ServiceReminderStatusEnum.DUE_SOON || sr.Status == ServiceReminderStatusEnum.UPCOMING))
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    // Query methods by status
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByStatusAsync(ServiceReminderStatusEnum status)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.Status == status)
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByPriorityAsync(PriorityLevelEnum priority)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.PriorityLevel == priority)
            .OrderBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    // Query methods by service schedule
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByServiceScheduleIdAsync(int serviceScheduleId)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.ServiceScheduleID == serviceScheduleId)
            .OrderBy(sr => sr.VehicleID)
            .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue)
            .ToListAsync();
    }

    // Query methods by date ranges
    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.DueDate.HasValue && sr.DueDate.Value >= startDate && sr.DueDate.Value <= endDate)
            .OrderBy(sr => sr.DueDate)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersCompletedInDateRangeAsync(DateTime startDate, DateTime endDate)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)

            .Where(sr => sr.Status == ServiceReminderStatusEnum.COMPLETED &&
                sr.CompletedDate.HasValue &&
                sr.CompletedDate.Value >= startDate &&
                sr.CompletedDate.Value <= endDate)
            .OrderBy(sr => sr.CompletedDate)
            .ToListAsync();
    }

    // Statistics methods
    public async Task<int> CountRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet.CountAsync(sr => sr.VehicleID == vehicleId);
    }

    public async Task<int> CountOverdueRemindersByVehicleIdAsync(int vehicleId)
    {
        return await _dbSet.CountAsync(sr => sr.VehicleID == vehicleId && sr.Status == ServiceReminderStatusEnum.OVERDUE);
    }

    public async Task<int> CountRemindersByStatusAsync(ServiceReminderStatusEnum status)
    {
        return await _dbSet.CountAsync(sr => sr.Status == status);
    }

    // Business logic methods
    public async Task<bool> HasPendingRemindersForVehicleAsync(int vehicleId)
    {
        return await _dbSet.AnyAsync(sr => sr.VehicleID == vehicleId &&
                                    sr.Status != ServiceReminderStatusEnum.COMPLETED &&
                                    sr.Status != ServiceReminderStatusEnum.CANCELLED);
    }

    public async Task<bool> HasOverdueRemindersForVehicleAsync(int vehicleId)
    {
        return await _dbSet.AnyAsync(sr => sr.VehicleID == vehicleId && sr.Status == ServiceReminderStatusEnum.OVERDUE);
    }

    public async Task<ServiceReminder?> GetNextDueReminderForVehicleAsync(int vehicleId)
    {
        return await _dbSet

            .Include(sr => sr.ServiceSchedule)
            .Where(sr => sr.VehicleID == vehicleId &&
                sr.Status != ServiceReminderStatusEnum.COMPLETED &&
                sr.Status != ServiceReminderStatusEnum.CANCELLED &&
                sr.DueDate.HasValue)
            .OrderBy(sr => sr.DueDate)
            .FirstOrDefaultAsync();
    }

    public async Task<IReadOnlyList<ServiceReminder>> GetServiceRemindersByWorkOrderIdAsync(int workOrderID)
    {
        return await _dbSet.Where(sr => sr.WorkOrderID == workOrderID).ToListAsync();
    }

    public async Task<PagedResult<ServiceReminder>> GetAllServiceRemindersPagedAsync(PaginationParameters parameters)
    {
        var query = _dbSet.AsQueryable();

        // Include related data
        query = query
            .Include(sr => sr.Vehicle)
            .Include(sr => sr.ServiceProgram)
            .Include(sr => sr.ServiceSchedule)
            .Include(sr => sr.WorkOrder);

        // Apply search filter
        query = ApplySearchFilter(query, parameters.Search);

        // Apply sorting
        query = ApplySorting(query, parameters.SortBy, parameters.SortDescending);

        var totalCount = await query.CountAsync();
        var items = await query
            .Skip((parameters.PageNumber - 1) * parameters.PageSize)
            .Take(parameters.PageSize)
            .ToListAsync();

        return new PagedResult<ServiceReminder>
        {
            Items = items,
            TotalCount = totalCount,
            PageNumber = parameters.PageNumber,
            PageSize = parameters.PageSize
        };
    }

    private static IQueryable<ServiceReminder> ApplySearchFilter(IQueryable<ServiceReminder> query, string? searchText)
    {
        if (string.IsNullOrWhiteSpace(searchText)) return query;

        string searchPattern = $"%{searchText.Trim().ToLowerInvariant()}%";

        return query.Where(sr =>
            EF.Functions.Like(sr.ServiceScheduleName, searchPattern) ||
            EF.Functions.Like(sr.ServiceProgramName ?? string.Empty, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.Name, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.Make, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.Model, searchPattern) ||
            EF.Functions.Like(sr.Vehicle.LicensePlate, searchPattern)
        );
    }

    private static IQueryable<ServiceReminder> ApplySorting(IQueryable<ServiceReminder> query, string? sortBy, bool sortDescending)
    {
        if (string.IsNullOrWhiteSpace(sortBy))
        {
            return query.OrderBy(sr => sr.Status == ServiceReminderStatusEnum.OVERDUE ? 0 :
                              sr.Status == ServiceReminderStatusEnum.DUE_SOON ? 1 : 2)
                        .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue); // Default sort: overdue first, then by due date
        }

        var sortByLower = sortBy.ToLower();
        return sortByLower switch
        {
            "vehiclename" => sortDescending
                ? query.OrderByDescending(sr => sr.Vehicle.Name)
                : query.OrderBy(sr => sr.Vehicle.Name),

            "serviceschedulename" => sortDescending
                ? query.OrderByDescending(sr => sr.ServiceScheduleName)
                : query.OrderBy(sr => sr.ServiceScheduleName),
            "serviceprogramname" => sortDescending
                ? query.OrderByDescending(sr => sr.ServiceProgramName)
                : query.OrderBy(sr => sr.ServiceProgramName),
            "duedate" => sortDescending
                ? query.OrderByDescending(sr => sr.DueDate)
                : query.OrderBy(sr => sr.DueDate),
            "duemileage" => sortDescending
                ? query.OrderByDescending(sr => sr.DueMileage)
                : query.OrderBy(sr => sr.DueMileage),
            "status" => sortDescending
                ? query.OrderByDescending(sr => sr.Status)
                : query.OrderBy(sr => sr.Status),
            "prioritylevel" => sortDescending
                ? query.OrderByDescending(sr => sr.PriorityLevel)
                : query.OrderBy(sr => sr.PriorityLevel),
            "createdat" => sortDescending
                ? query.OrderByDescending(sr => sr.CreatedAt)
                : query.OrderBy(sr => sr.CreatedAt),
            "updatedat" => sortDescending
                ? query.OrderByDescending(sr => sr.UpdatedAt)
                : query.OrderBy(sr => sr.UpdatedAt),
            _ => query.OrderBy(sr => sr.Status == ServiceReminderStatusEnum.OVERDUE ? 0 :
                              sr.Status == ServiceReminderStatusEnum.DUE_SOON ? 1 : 2)
                      .ThenBy(sr => sr.DueDate ?? DateTime.MaxValue) // Default sort for unrecognized fields
        };
    }
}