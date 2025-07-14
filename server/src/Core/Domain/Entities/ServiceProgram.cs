using System;

using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// A Service Program is a N:N relationship between <see cref="ServiceSchedule"/>s and <see cref="VehicleServiceProgram"/>s.
/// </summary>
/// <remarks>
/// A Service Program contains:
/// <list type="bullet">
///   <item>Many <see cref="ServiceSchedule"/>s</item>
///   <item>Many <see cref="VehicleServiceProgram"/>s</item>
/// </list>
/// </remarks>
public class ServiceProgram : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsActive { get; set; } = true;

    // Navigation Properties
    public required ICollection<ServiceSchedule> ServiceSchedules { get; set; } = [];
    public required ICollection<VehicleServiceProgram> VehicleServicePrograms { get; set; } = [];
}