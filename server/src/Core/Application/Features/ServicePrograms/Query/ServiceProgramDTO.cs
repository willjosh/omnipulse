using Application.Features.ServiceSchedules.Query;

namespace Application.Features.ServicePrograms.Query;

/// <summary>
/// Data Transfer Object representing detailed information about a single service program and its service schedules.
/// </summary>
public class ServiceProgramDTO
{
    /// <example>1</example>
    public required int ID { get; set; }

    /// <example>Service Program Name</example>
    public required string Name { get; set; }

    /// <example>Service Program Description</example>
    public string? Description { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; }

    public required List<ServiceScheduleDTO> ServiceSchedules { get; set; } = [];

    /// <example>[1, 2, 3]</example>
    public required List<int> AssignedVehicleIDs { get; set; } = [];
}