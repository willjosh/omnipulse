using System;

namespace Domain.Entities;

public class VehicleGroup : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required bool IsActive { get; set; } = true;
}
