using System;

namespace Domain.Entities;

public class InventoryItemLocation : BaseEntity
{
    public required string LocationName { get; set; }
    public required string Address { get; set; }
    public required double Longitude { get; set; }
    public required double Latitude { get; set; }
    public int Capacity { get; set; }
    public Boolean IsActive { get; set; } = true;

    // navigation properties
    public ICollection<Inventory> Inventories { get; set; } = [];
}
