using Application.Features.ServiceSchedules.Query;

namespace Application.Features.ServicePrograms.Query.GetServiceProgram;

/// <summary>
/// Data Transfer Object representing detailed information about a single service program.
/// </summary>
public class ServiceProgramDTO
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsActive { get; set; }
    public required List<ServiceScheduleDTO> ServiceSchedules { get; set; } = [];
    public required List<int> AssignedVehicleIds { get; set; } = [];
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}