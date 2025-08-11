namespace Application.Features.ServiceReminders.Command.GenerateServiceReminders;

/// <summary>
/// Response for the <see cref="SyncServiceRemindersCommand"/>
/// </summary>
public record SyncServiceRemindersResponse(
    int GeneratedCount,
    bool Success,
    string? ErrorMessage = null
);