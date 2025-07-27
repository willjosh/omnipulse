namespace Application.Features.VehicleGroups.Query.GetAllVehicleGroup;

public class GetAllVehicleGroupDTO
{
    public int ID { get; set; }
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool IsActive { get; set; }
}