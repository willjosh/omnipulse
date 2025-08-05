using Domain.Entities.Enums;

namespace Application.Features.ServiceReminders.Query;

/// <summary>
/// Represents information about a specific service task within a service reminder.
/// </summary>
public class ServiceTaskInfoDTO
{
    /// <summary>Service task ID.</summary>
    /// <example>1</example>
    public required int ServiceTaskID { get; set; }

    /// <summary>Service task name for display purposes.</summary>
    /// <example>Engine Oil Change</example>
    public required string ServiceTaskName { get; set; }

    /// <summary>Service task category.</summary>
    /// <example>PREVENTIVE</example>
    public required ServiceTaskCategoryEnum ServiceTaskCategory { get; set; }

    /// <summary>Estimated labor hours for this task.</summary>
    /// <example>2.0</example>
    public required double EstimatedLabourHours { get; set; }

    /// <summary>Estimated cost for this task.</summary>
    /// <example>75.50</example>
    public required decimal EstimatedCost { get; set; }

    /// <summary>Task description for additional context.</summary>
    /// <example>Replace engine oil and oil filter</example>
    public string? Description { get; set; }

    /// <summary>Whether this task is required or optional.</summary>
    /// <example>true</example>
    public required bool IsRequired { get; set; }
}