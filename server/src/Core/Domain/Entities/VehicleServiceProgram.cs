using System;

namespace Domain.Entities;

public class VehicleServiceProgram
{
    public required int VehicleID { get; set; }
    public required int ServiceProgramID { get; set; }
    public required DateTime AssignedDate { get; set; }
    public required Boolean IsActive { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // navigation properties
    public required Vehicle Vehicle { get; set; }
    public required ServiceProgram ServiceProgram { get; set; }
}
