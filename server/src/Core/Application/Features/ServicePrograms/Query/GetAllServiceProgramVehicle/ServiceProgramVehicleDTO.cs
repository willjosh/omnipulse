namespace Application.Features.ServicePrograms.Query.GetAllServiceProgramVehicle;

/// <summary>
/// Data Transfer Object representing a vehicle assigned to a service program.
/// </summary>
public class ServiceProgramVehicleDTO
{
    public required int VehicleID { get; set; }
    public required string VehicleName { get; set; }
    // public required string AddedByUserID { get; set; } // TODO
    public required DateTime AddedAt { get; set; }
}