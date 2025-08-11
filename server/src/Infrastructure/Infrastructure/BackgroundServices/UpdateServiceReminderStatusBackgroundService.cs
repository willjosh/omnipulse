using Application.Contracts.Persistence;
using Application.Contracts.Services;
using Application.Features.ServiceReminders.Command.GenerateServiceReminders;

using Domain.Entities;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

/// <summary>
/// Background service that continuously updates statuses and syncs <see cref="ServiceReminder"/>s
/// </summary>
public class UpdateServiceReminderStatusBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UpdateServiceReminderStatusBackgroundService> _logger;
    private readonly TimeProvider _timeProvider;

    public UpdateServiceReminderStatusBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UpdateServiceReminderStatusBackgroundService> logger,
        TimeProvider timeProvider)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
        _timeProvider = timeProvider;
    }

    // Constants
    private const bool Enabled = true;
    private static readonly TimeSpan UpdateInterval = TimeSpan.FromMinutes(1);
    private static readonly TimeSpan GenerationInterval = TimeSpan.FromHours(1);
    private static readonly TimeSpan RetryDelay = TimeSpan.FromSeconds(20);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        // if (!Enabled)
        // {
        //     _logger.LogInformation($"{nameof(UpdateServiceReminderStatusBackgroundService)} is disabled.");
        //     return;
        // }

        _logger.LogInformation($"{nameof(UpdateServiceReminderStatusBackgroundService)} started.");

        var lastGenerationTime = _timeProvider.GetUtcNow().UtcDateTime;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var reminderStatusUpdater = scope.ServiceProvider.GetRequiredService<IServiceReminderStatusUpdater>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();
                var reminderRepository = scope.ServiceProvider.GetRequiredService<IServiceReminderRepository>();

                // Update statuses every minute
                await reminderStatusUpdater.UpdateAllReminderStatusesAsync(stoppingToken);

                // Sync reminders every hour
                var timeSinceLastGeneration = _timeProvider.GetUtcNow().UtcDateTime - lastGenerationTime;
                if (timeSinceLastGeneration >= GenerationInterval)
                {
                    _logger.LogInformation("Triggering service reminder sync");
                    var syncResult = await mediator.Send(new SyncServiceRemindersCommand(), stoppingToken);

                    if (syncResult.Success)
                    {
                        _logger.LogInformation("Successfully generated {Count} service reminders", syncResult.GeneratedCount);
                        lastGenerationTime = _timeProvider.GetUtcNow().UtcDateTime;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to sync service reminders: {ErrorMessage}", syncResult.ErrorMessage);
                    }
                }

                // Cleanup: delete any unlinked/orphaned reminders (vehicle/schedule missing, or vehicle no longer in program)
                var deleted = await reminderRepository.DeleteAllUnlinkedReminders(stoppingToken);
                if (deleted > 0)
                {
                    _logger.LogInformation("Deleted {Count} unlinked service reminders during maintenance sweep", deleted);
                }

                await Task.Delay(UpdateInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while updating service reminder statuses");
                await Task.Delay(RetryDelay, stoppingToken);
            }
        }

        _logger.LogInformation($"{nameof(UpdateServiceReminderStatusBackgroundService)} stopped");
    }
}