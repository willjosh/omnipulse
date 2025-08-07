using Infrastructure.BackgroundServices;

namespace Infrastructure.Configuration;

/// <summary>
/// Configuration options for <see cref="ServiceReminderStatusUpdateService"/>
/// </summary>
public class ServiceReminderUpdateOptions
{
    /// <summary>The interval between status updates. Default is 1 minute.</summary>
    public TimeSpan UpdateInterval { get; set; } = TimeSpan.FromMinutes(1);

    /// <summary>The retry delay when an error occurs. Default is 30 seconds.</summary>
    public TimeSpan RetryDelay { get; set; } = TimeSpan.FromSeconds(30);

    /// <summary>Whether the background service is enabled. Default is true.</summary>
    public bool Enabled { get; set; } = true;
}