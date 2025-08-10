using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Contracts.Services;

using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Services;

public class ServiceReminderStatusUpdater : IServiceReminderStatusUpdater
{
    private readonly IServiceReminderRepository _serviceReminderRepository;
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IAppLogger<ServiceReminderStatusUpdater> _logger;
    private readonly TimeProvider _timeProvider;

    public ServiceReminderStatusUpdater(
        IServiceReminderRepository serviceReminderRepository,
        IVehicleRepository vehicleRepository,
        IAppLogger<ServiceReminderStatusUpdater> logger,
        TimeProvider timeProvider)
    {
        _serviceReminderRepository = serviceReminderRepository;
        _vehicleRepository = vehicleRepository;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    public async Task UpdateAllReminderStatusesAsync(CancellationToken cancellationToken = default)
    {
        _logger.LogDebug($"{nameof(UpdateAllReminderStatusesAsync)} - Starting service reminder {nameof(ServiceReminder.Status)} update");

        // Get reminders with their schedule data included
        var activeReminders = await _serviceReminderRepository.GetRemindersByStatusesAsync(GetActiveStatuses());
        if (!activeReminders.Any())
        {
            _logger.LogDebug("No active reminders found to update");
            return;
        }

        var updatedCount = 0;
        var currentTime = _timeProvider.GetUtcNow().UtcDateTime;

        foreach (var reminder in activeReminders)
        {
            // Vehicle should already be loaded via include
            if (reminder.Vehicle == null)
            {
                _logger.LogWarning("Vehicle not loaded for reminder {ReminderId}", reminder.ID);
                continue;
            }

            // ServiceSchedule should already be loaded via include
            if (reminder.ServiceSchedule == null)
            {
                _logger.LogWarning("ServiceSchedule not loaded for reminder {ReminderId}", reminder.ID);
                continue;
            }

            var newStatus = reminder.DetermineServiceReminderStatus(reminder.Vehicle.Mileage, currentTime);

            if (reminder.Status != newStatus)
            {
                var oldStatus = reminder.Status;
                reminder.Status = newStatus;
                reminder.UpdatedAt = currentTime;

                updatedCount++;
                _logger.LogDebug("Updated reminder {ReminderId} status from {OldStatus} to {NewStatus}",
                    reminder.ID, oldStatus, newStatus);
            }
        }

        if (updatedCount > 0)
        {
            await _serviceReminderRepository.SaveChangesAsync();
            _logger.LogInformation("Updated {Count} service reminder statuses", updatedCount);
        }
        else
        {
            _logger.LogDebug("No reminder statuses needed updating");
        }
    }

    private static ServiceReminderStatusEnum[] GetActiveStatuses() =>
    [
        ServiceReminderStatusEnum.UPCOMING,
        ServiceReminderStatusEnum.DUE_SOON,
        ServiceReminderStatusEnum.OVERDUE
    ];
}