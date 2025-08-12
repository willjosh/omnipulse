using Application.Features.ServiceReminders.Query;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Contracts.Persistence;

public interface IServiceReminderRepository : IGenericRepository<ServiceReminder>
{
    // Simple data access for Application layer to perform calculations
    Task<List<ServiceSchedule>> GetActiveServiceSchedulesWithDataAsync(CancellationToken ct);

    // Sync calculated reminders to database for persistence
    Task SyncRemindersAsync(List<ServiceReminderDTO> calculatedReminders);

    // Paged Results for stored reminders
    Task<PagedResult<ServiceReminder>> GetAllServiceRemindersPagedAsync(PaginationParameters parameters);

    // Get service reminder with related data
    Task<ServiceReminder?> GetServiceReminderWithDetailsAsync(int serviceReminderId);

    // Query methods by vehicle
    Task<IReadOnlyList<ServiceReminder>> GetRemindersByVehicleIdAsync(int vehicleId);
    Task<IReadOnlyList<ServiceReminder>> GetOverdueRemindersByVehicleIdAsync(int vehicleId);
    Task<IReadOnlyList<ServiceReminder>> GetUpcomingRemindersByVehicleIdAsync(int vehicleId);

    // Query methods by status
    Task<IReadOnlyList<ServiceReminder>> GetRemindersByStatusAsync(ServiceReminderStatusEnum status);
    Task<IReadOnlyList<ServiceReminder>> GetRemindersByStatusesAsync(IEnumerable<ServiceReminderStatusEnum> statuses);


    // Query methods by service schedule
    Task<IReadOnlyList<ServiceReminder>> GetRemindersByServiceScheduleIdAsync(int serviceScheduleId);

    // Query methods by date ranges
    Task<IReadOnlyList<ServiceReminder>> GetRemindersByDateRangeAsync(DateTime startDate, DateTime endDate);
    Task<IReadOnlyList<ServiceReminder>> GetRemindersCompletedInDateRangeAsync(DateTime startDate, DateTime endDate);

    // Statistics methods
    Task<int> CountRemindersByVehicleIdAsync(int vehicleId);
    Task<int> CountOverdueRemindersByVehicleIdAsync(int vehicleId);
    Task<int> CountRemindersByStatusAsync(ServiceReminderStatusEnum status);

    // Business logic methods
    Task<bool> HasPendingRemindersForVehicleAsync(int vehicleId);
    Task<bool> HasOverdueRemindersForVehicleAsync(int vehicleId);
    Task<ServiceReminder?> GetNextDueReminderForVehicleAsync(int vehicleId);

    public Task<IReadOnlyList<ServiceReminder>> GetServiceRemindersByWorkOrderIdAsync(int workOrderId);

    /// <summary>
    /// Delete non-final reminders for a schedule (any status not COMPLETED or CANCELLED), excluding WorkOrder-linked reminders.
    /// </summary>
    Task<int> DeleteNonFinalRemindersForScheduleAsync(int scheduleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Add only new reminders from the calculated candidates. Existing reminders (same vehicle, schedule, due target, status) are skipped. Returns the number of inserted reminders.
    /// </summary>
    Task<int> AddNewRemindersAsync(IEnumerable<ServiceReminderDTO> candidates, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hard-deletes service reminders whose parent entities no longer exist (Vehicle or ServiceSchedule missing).
    /// Returns the number of deleted rows.
    /// </summary>
    Task<int> DeleteAllUnlinkedReminders(CancellationToken cancellationToken = default);
}