namespace Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;

/// <summary>
/// Data Transfer Object representing a vehicle assigned to a service program.
/// </summary>
public class XrefServiceProgramVehicleDTO
{
    /// <example>1</example>
    public required int ServiceProgramID { get; set; }

    /// <example>1</example>
    public required int VehicleID { get; set; }

    /// <example>BYD K9 MAL‑315</example>
    public required string VehicleName { get; set; }

    /// <example>566ae2d4-a781-4690-84c0-f8b284868e43</example>
    // public required string AddedByUserID { get; set; } // XrefServiceProgramVehicle User

    public required DateTime AddedAt { get; set; }
}