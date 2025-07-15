namespace Domain.Entities;

public class XrefServiceProgramVehicle
{
    // Composite PK
    public required int ServiceProgramID { get; set; }
    public required int VehicleID { get; set; }

    public required string AddedByUserID { get; set; } // FK
    public required DateTime AddedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public required ServiceProgram ServiceProgram { get; set; }
    public required Vehicle Vehicle { get; set; }
    public required User User { get; set; }
}