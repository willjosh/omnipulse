using Application.Features.ServiceReminders.Query;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Contracts.Persistence;

public interface IServiceReminderRepository : IGenericRepository<ServiceReminder>
{
    // Calculated service reminders (primary method for GetAll queries)
    Task<PagedResult<ServiceReminderDTO>> GetAllCalculatedServiceRemindersPagedAsync(PaginationParameters parameters);

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
    Task<IReadOnlyList<ServiceReminder>> GetRemindersByPriorityAsync(PriorityLevelEnum priority);

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
}