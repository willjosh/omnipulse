
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

    public async Task<List<ServiceSchedule>> GetActiveServiceSchedulesWithDataAsync(CancellationToken ct)
    {
        return await _dbContext.Set<ServiceSchedule>()
            .Where(ss => ss.IsActive)
            .Include(ss => ss.ServiceProgram)
                .ThenInclude(sp => sp.XrefServiceProgramVehicles)
                .ThenInclude(xspv => xspv.Vehicle)
            .Include(ss => ss.XrefServiceScheduleServiceTasks)
                .ThenInclude(xsst => xsst.ServiceTask)
            .ToListAsync(cancellationToken: ct);
    }

    /// <summary>
    /// Hard delete non-final reminders for a schedule (any status that is not COMPLETED or CANCELLED),
    /// excluding reminders linked to a WorkOrder.
    /// </summary>
    public async Task<int> DeleteNonFinalRemindersForScheduleAsync(int scheduleId, CancellationToken cancellationToken = default)
    {
        // Delete non-final reminders. Exclude WorkOrder-linked.
        var rows = await _dbSet
            .Where(sr => sr.ServiceScheduleID == scheduleId &&
                         sr.WorkOrderID == null &&
                         !ServiceReminderExtensions.FinalStatuses.Contains(sr.Status))
            .ExecuteDeleteAsync(cancellationToken);

        return rows;
    }

    /// <summary>
    /// Sync calculated reminders to database for persistence
    /// </summary>
    public async Task SyncRemindersAsync(List<ServiceReminderDTO> calculatedReminders)
    {
        // Validate: ensure at most one UPCOMING per vehicle-schedule pair
        var upcomingGroups = calculatedReminders
            .Where(r => r.Status == ServiceReminderStatusEnum.UPCOMING)
            .GroupBy(r => new { r.VehicleID, r.ServiceScheduleID })
            .Where(g => g.Count() > 1);

        if (upcomingGroups.Any())
        {
            var firstGroup = upcomingGroups.First();
            throw new InvalidOperationException(
                $"Multiple UPCOMING reminders for VehicleID {firstGroup.Key.VehicleID}, " +
                $"ServiceScheduleID {firstGroup.Key.ServiceScheduleID}. Only one is allowed.");
        }

        // BATCH 1: Get potential existing reminders by vehicle and schedule
        var vehicleIds = calculatedReminders.Select(cr => cr.VehicleID).Distinct().ToList();
        var scheduleIds = calculatedReminders.Select(cr => cr.ServiceScheduleID).Distinct().ToList();

        var potentialReminders = await _dbSet
            .Where(sr => vehicleIds.Contains(sr.VehicleID) && scheduleIds.Contains(sr.ServiceScheduleID))
            .ToListAsync();

        // BATCH 2: Get all required vehicles and schedules
        var vehicles = await _dbContext.Set<Vehicle>()
            .Where(v => vehicleIds.Contains(v.ID))
            .ToDictionaryAsync(v => v.ID);

        var schedules = await _dbContext.Set<ServiceSchedule>()
            .Where(s => scheduleIds.Contains(s.ID))
            .ToDictionaryAsync(s => s.ID);

        var remindersToAdd = new List<ServiceReminder>();
        var remindersToUpdate = new List<ServiceReminder>();

        foreach (var calculated in calculatedReminders)
        {
            // Find existing in memory
            var existing = potentialReminders.FirstOrDefault(sr =>
                sr.VehicleID == calculated.VehicleID &&
                sr.ServiceScheduleID == calculated.ServiceScheduleID &&
                sr.DueDate == calculated.DueDate &&
                sr.DueMileage == calculated.DueMileage);

            if (existing == null)
            {
                if (!vehicles.TryGetValue(calculated.VehicleID, out var vehicle) ||
                    !schedules.TryGetValue(calculated.ServiceScheduleID, out var serviceSchedule))
                {
                    continue;
                }

                // CREATE: New reminder with minimal fields
                var newReminder = new ServiceReminder
                {
                    ID = 0, // Will be auto-generated
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    VehicleID = calculated.VehicleID,
                    Vehicle = vehicle,
                    ServiceScheduleID = calculated.ServiceScheduleID,
                    ServiceSchedule = serviceSchedule,
                    WorkOrderID = calculated.WorkOrderID,
                    DueDate = calculated.DueDate,
                    DueMileage = calculated.DueMileage,
                    Status = calculated.Status,
                    CompletedDate = null,
                    CancelReason = null
                };

                remindersToAdd.Add(newReminder);
            }
            else if (existing.Status != ServiceReminderStatusEnum.COMPLETED)
            {
                // UPDATE: Only update status (preserve completed ones)
                existing.Status = calculated.Status;
                existing.UpdatedAt = DateTime.UtcNow;
                remindersToUpdate.Add(existing);
            }
        }

        // BATCH 3: Perform bulk operations
        if (remindersToAdd.Count != 0)
        {
            await _dbSet.AddRangeAsync(remindersToAdd);
        }
        // EF Core tracks changes for remindersToUpdate automatically

        await _dbContext.SaveChangesAsync();
    }

    public async Task<int> AddNewRemindersAsync(IEnumerable<ServiceReminderDTO> candidates, CancellationToken cancellationToken = default)
    {
        var candidateList = candidates.ToList();
        if (candidateList.Count == 0) return 0;

        var vehicleIds = candidateList.Select(c => c.VehicleID).Distinct().ToList();
        var scheduleIds = candidateList.Select(c => c.ServiceScheduleID).Distinct().ToList();

        var existing = await _dbSet
            .Where(sr => vehicleIds.Contains(sr.VehicleID) && scheduleIds.Contains(sr.ServiceScheduleID))
            .Select(sr => new { sr.VehicleID, sr.ServiceScheduleID, sr.DueDate, sr.DueMileage })
            .ToListAsync(cancellationToken);

        var existingKeys = new HashSet<(int, int, DateTime?, double?)>(
            existing.Select(e => (e.VehicleID, e.ServiceScheduleID, e.DueDate, e.DueMileage))
        );

        // Load required navs to satisfy required properties
        var vehicles = await _dbContext.Set<Vehicle>()
            .Where(v => vehicleIds.Contains(v.ID))
            .ToDictionaryAsync(v => v.ID, cancellationToken);

        var schedules = await _dbContext.Set<ServiceSchedule>()
            .Where(s => scheduleIds.Contains(s.ID))
            .ToDictionaryAsync(s => s.ID, cancellationToken);

        var toInsert = new List<ServiceReminder>();
        foreach (var c in candidateList)
        {
            var key = (c.VehicleID, c.ServiceScheduleID, c.DueDate, c.DueMileage);
            if (existingKeys.Contains(key)) continue;

            if (!vehicles.TryGetValue(c.VehicleID, out var vehicle) || !schedules.TryGetValue(c.ServiceScheduleID, out var schedule))
            {
                continue;
            }

            toInsert.Add(new ServiceReminder
            {
                ID = 0,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleID = c.VehicleID,
                Vehicle = vehicle,
                ServiceScheduleID = c.ServiceScheduleID,
                ServiceSchedule = schedule,
                WorkOrderID = c.WorkOrderID,
                DueDate = c.DueDate,
                DueMileage = c.DueMileage,
                Status = c.Status,
                CompletedDate = null,
                CancelReason = null
            });
        }

        if (toInsert.Count == 0) return 0;

        await _dbSet.AddRangeAsync(toInsert, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
        return toInsert.Count;
    }

    /// <summary>
    /// Deletes all service reminders that are no longer linked to valid parent entities.
    /// Typically used when a service schedule is deleted, deactivated, or when vehicles are unassigned from the associated service program
    /// </summary>
    /// <param name="cancellationToken">Cancellation token for the async operation.</param>
    /// <returns>The number of reminders deleted.</returns>
    public async Task<int> DeleteAllUnlinkedReminders(CancellationToken cancellationToken = default)
    {
        // Conditions:
        // 1) Vehicle missing, OR
        // 2) ServiceSchedule missing, OR
        // 3) Vehicle no longer assigned to the ServiceProgram that owns the ServiceSchedule.
        var numRowsDeleted = await _dbSet
            .Where(sr =>
                !_dbContext.Set<Vehicle>().Any(v => v.ID == sr.VehicleID) ||
                !_dbContext.Set<ServiceSchedule>().Any(s => s.ID == sr.ServiceScheduleID) ||
                !_dbContext.Set<ServiceSchedule>().Any(s => s.ID == sr.ServiceScheduleID &&
                    _dbContext.Set<XrefServiceProgramVehicle>().Any(x => x.ServiceProgramID == s.ServiceProgramID && x.VehicleID == sr.VehicleID))
            )
            .ExecuteDeleteAsync(cancellationToken);

        return numRowsDeleted;
    }

    public async Task<ServiceReminder?> GetServiceReminderWithDetailsAsync(int serviceReminderId)
    {
        return await _dbSet
            .Include(sr => sr.Vehicle)
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

    public async Task<IReadOnlyList<ServiceReminder>> GetRemindersByStatusesAsync(IEnumerable<ServiceReminderStatusEnum> statuses)
    {
        var statusList = statuses.ToList();
        return await _dbSet
            .Include(sr => sr.Vehicle)
            .Include(sr => sr.ServiceSchedule)
            .Where(sr => statusList.Contains(sr.Status))
            .ToListAsync();
    }

    // Note: Priority is now calculated on demand from status, not stored in the entity

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

        // Include related data for MapEntitiesToDTOs method
        query = query
            .Include(sr => sr.Vehicle)
            .Include(sr => sr.ServiceSchedule)
                .ThenInclude(ss => ss.ServiceProgram)
            .Include(sr => sr.ServiceSchedule)
                .ThenInclude(ss => ss.XrefServiceScheduleServiceTasks)
                .ThenInclude(xsst => xsst.ServiceTask)
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
            EF.Functions.Like(sr.ServiceSchedule.Name, searchPattern) ||
            EF.Functions.Like(sr.ServiceSchedule.ServiceProgram.Name ?? string.Empty, searchPattern) ||
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
                ? query.OrderByDescending(sr => sr.ServiceSchedule.Name)
                : query.OrderBy(sr => sr.ServiceSchedule.Name),
            "serviceprogramname" => sortDescending
                ? query.OrderByDescending(sr => sr.ServiceSchedule.ServiceProgram.Name)
                : query.OrderBy(sr => sr.ServiceSchedule.ServiceProgram.Name),
            "duedate" => sortDescending
                ? query.OrderByDescending(sr => sr.DueDate)
                : query.OrderBy(sr => sr.DueDate),
            "duemileage" => sortDescending
                ? query.OrderByDescending(sr => sr.DueMileage)
                : query.OrderBy(sr => sr.DueMileage),
            "status" => sortDescending
                ? query.OrderByDescending(sr => sr.Status)
                : query.OrderBy(sr => sr.Status),
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