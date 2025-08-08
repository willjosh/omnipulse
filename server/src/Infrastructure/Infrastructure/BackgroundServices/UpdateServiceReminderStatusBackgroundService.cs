using Application.Contracts.Services;
using Application.Features.ServiceReminders.Command.GenerateServiceReminders;

using Domain.Entities;

using MediatR;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Infrastructure.BackgroundServices;

/// <summary>
/// Background service that continuously generates and updates all <see cref="ServiceReminder"/>
/// </summary>
public class UpdateServiceReminderStatusBackgroundService : BackgroundService
{
    private readonly IServiceScopeFactory _serviceScopeFactory;
    private readonly ILogger<UpdateServiceReminderStatusBackgroundService> _logger;

    public UpdateServiceReminderStatusBackgroundService(
        IServiceScopeFactory serviceScopeFactory,
        ILogger<UpdateServiceReminderStatusBackgroundService> logger)
    {
        _serviceScopeFactory = serviceScopeFactory;
        _logger = logger;
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

        var lastGenerationTime = DateTime.UtcNow;

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _serviceScopeFactory.CreateScope();
                var reminderStatusUpdater = scope.ServiceProvider.GetRequiredService<IServiceReminderStatusUpdater>();
                var mediator = scope.ServiceProvider.GetRequiredService<IMediator>();

                // Update statuses every minute
                await reminderStatusUpdater.UpdateAllReminderStatusesAsync(stoppingToken);

                // Generate new reminders every hour
                var timeSinceLastGeneration = DateTime.UtcNow - lastGenerationTime;
                if (timeSinceLastGeneration >= GenerationInterval)
                {
                    _logger.LogInformation("Triggering service reminder generation");
                    var generateCommand = new GenerateServiceRemindersCommand();
                    var generateResult = await mediator.Send(generateCommand, stoppingToken);

                    if (generateResult.Success)
                    {
                        _logger.LogInformation("Successfully generated {Count} service reminders", generateResult.GeneratedCount);
                        lastGenerationTime = DateTime.UtcNow;
                    }
                    else
                    {
                        _logger.LogWarning("Failed to generate service reminders: {ErrorMessage}", generateResult.ErrorMessage);
                    }
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