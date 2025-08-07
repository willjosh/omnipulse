namespace Application.Contracts.Services;

public interface IServiceReminderStatusUpdater
{
    Task UpdateAllReminderStatusesAsync(CancellationToken cancellationToken = default);
}