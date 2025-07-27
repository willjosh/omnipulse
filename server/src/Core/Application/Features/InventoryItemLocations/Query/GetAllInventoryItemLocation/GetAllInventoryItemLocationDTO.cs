namespace Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

public class GetAllInventoryItemLocationDTO
{
    /// <example>1</example>
    public int ID { get; set; }

    /// <example>Main Warehouse</example>
    public required string LocationName { get; set; }

    /// <example>123 Fleet St, Sydney, NSW 2000</example>
    public required string Address { get; set; }

    /// <example>151.2093</example>
    public required double Longitude { get; set; }

    /// <example>-33.8688</example>
    public required double Latitude { get; set; }

    /// <example>500</example>
    public required int Capacity { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; }
}