using Domain.Entities.Enums;

namespace Application.Features.ServiceTasks.Query;

public class ServiceTaskDTO
{
    /// <example>1</example>
    public int ID { get; set; }

    /// <example>Service Task Name</example>
    public required string Name { get; set; }

    /// <example>Service Task Description</example>
    public string? Description { get; set; }

    /// <example>2.5</example>
    public required double EstimatedLabourHours { get; set; }

    /// <example>100.00</example>
    public required decimal EstimatedCost { get; set; }

    /// <example>1</example>
    public required ServiceTaskCategoryEnum Category { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; }
}