namespace Domain.Entities;

public class XrefServiceProgramVehicle
{
    public required int ServiceProgramID { get; set; }
    public required int VehicleID { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public required ServiceProgram ServiceProgram { get; set; }
    public required Vehicle Vehicle { get; set; }
}