namespace Domain.Entities.Enums;

public enum ServiceReminderStatusEnum
{
    /// <summary>Service is scheduled in the future and is outside the "due soon" threshold.</summary>
    UPCOMING = 1,
    /// <summary>Service is scheduled in the future and is within the "due soon" threshold.</summary>
    DUE_SOON = 2,
    /// <summary>Service is overdue.</summary>
    OVERDUE = 3,
    /// <summary>Service has been completed.</summary>
    COMPLETED = 4,
    /// <summary>Service has been cancelled.</summary>
    CANCELLED = 5
}