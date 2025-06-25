using System;

namespace Domain.Entities;

public class VehicleServiceProgram : BaseEntity
{
    public required int VehicleID { get; set; }
    public required int ServiceProgramID { get; set; }
    public required DateTime AssignedDate { get; set; }
    public required Boolean IsActive { get; set; }

    // navigation properties
    public required Vehicle Vehicle { get; set; }
    public required ServiceProgram ServiceProgram { get; set; }
}
