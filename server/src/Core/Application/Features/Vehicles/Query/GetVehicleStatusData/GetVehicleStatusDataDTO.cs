namespace Application.Features.Vehicles.Query.GetVehicleStatusData;

public class GetVehicleStatusDataDTO
{
    public int ActiveVehicleCount { get; set; }
    public int MaintenanceVehicleCount { get; set; }
    public int OutOfServiceVehicleCount { get; set; }
    public int InactiveVehicleCount { get; set; }
}