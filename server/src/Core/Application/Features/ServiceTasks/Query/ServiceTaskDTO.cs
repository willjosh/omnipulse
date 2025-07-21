using Domain.Entities.Enums;

namespace Application.Features.ServiceTasks.Query;

public class ServiceTaskDTO
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required double EstimatedLabourHours { get; set; }
    public required decimal EstimatedCost { get; set; }
    public required ServiceTaskCategoryEnum Category { get; set; }
    public required bool IsActive { get; set; }
}