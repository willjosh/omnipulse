namespace Application.Features.ServicePrograms.Query.GetAllServiceProgram;

public class GetAllServiceProgramDTO
{
    /// <example>1</example>
    public required int ID { get; set; }

    /// <example>Service Program Name</example>
    public required string Name { get; set; }

    /// <example>Service Program Description</example>
    public string? Description { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; }

    /// <example>5</example>
    public required int ServiceScheduleCount { get; set; }

    /// <example>3</example>
    public required int AssignedVehicleCount { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}