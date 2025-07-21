using System;

namespace Application.Features.InventoryItemLocations.Query.GetAllInventoryItemLocation;

public class GetAllInventoryItemLocationDTO
{
    public int ID { get; set; }
    public required string LocationName { get; set; }
    public required string Address { get; set; }
    public required double Longitude { get; set; }
    public required double Latitude { get; set; }
    public required int Capacity { get; set; }
    public required bool IsActive { get; set; }
}