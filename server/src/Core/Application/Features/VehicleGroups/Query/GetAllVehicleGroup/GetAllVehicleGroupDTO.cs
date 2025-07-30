namespace Application.Features.VehicleGroups.Query.GetAllVehicleGroup;

public class GetAllVehicleGroupDTO
{
    /// <example>1</example>
    public int ID { get; set; }

    /// <example>Transportation Fleet</example>
    public required string Name { get; set; }

    /// <example>Fleet of vehicles for transportation.</example>
    public required string Description { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; }
}