using Application.Features.ServiceReminders.Query;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Contracts.Persistence;

public interface IServiceReminderRepository : IGenericRepository<ServiceReminder>
{
    // Simple data access for Application layer to perform calculations
    Task<List<ServiceSchedule>> GetActiveServiceSchedulesWithDataAsync();

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

    // Cancellation/update helpers
    /// <summary>
    /// Cancels future auto-generated reminders for a schedule by setting Status to CANCELLED for reminders
    /// with status UPCOMING or DUE_SOON and no linked WorkOrder.
    /// </summary>
    /// <param name="serviceScheduleId">Target schedule ID.</param>
    /// <returns>The number of reminders updated.</returns>
    Task<int> CancelFutureRemindersForScheduleAsync(int serviceScheduleId);
}