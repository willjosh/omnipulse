using System;

namespace Domain.Entities;
using Domain.Entities.Enums;

public class VehicleAssignment : BaseEntity
{
    // FKs
    public required int VehicleID { get; set; }
    public required string AssignedToUserID { get; set; }

    public required DateTime AssignedDate { get; set; }
    public DateTime? UnassignedDate { get; set; }
    public required bool IsActive { get; set; } = true;
    public required AssignmentTypeEnum AssignmentType { get; set; }
    public string? Notes { get; set; }

    // Navigation Properties
    public required Vehicle Vehicle { get; set; }
    public required User User { get; set; }
}