namespace Application.Features.ServiceReminders.Command.GenerateServiceReminders;

/// <summary>
/// Response for the <see cref="GenerateServiceRemindersCommand"/>
/// </summary>
public record GenerateServiceRemindersResponse(
    int GeneratedCount,
    int UpdatedCount,
    bool Success,
    string? ErrorMessage = null
);