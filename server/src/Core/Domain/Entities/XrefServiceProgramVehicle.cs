namespace Domain.Entities;

public class XrefServiceProgramVehicle
{
    // Composite PK
    public required int ServiceProgramID { get; set; }
    public required int VehicleID { get; set; }

    public required DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public required double? VehicleMileageAtAssignment { get; set; }

    // Navigation Properties
    public required ServiceProgram ServiceProgram { get; set; }
    public required Vehicle Vehicle { get; set; }
}