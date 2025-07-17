using Application.Features.ServiceSchedules.Query;

namespace Application.Features.ServicePrograms.Query;

/// <summary>
/// Data Transfer Object representing detailed information about a single service program and its service schedules.
/// </summary>
public class ServiceProgramDTO
{
    public required int ServiceProgramID { get; set; }
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsActive { get; set; }
    public required List<ServiceScheduleDTO> ServiceSchedules { get; set; } = [];
    public required List<int> AssignedVehicleIDs { get; set; } = [];
}