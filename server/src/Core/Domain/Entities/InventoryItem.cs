using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class InventoryItem : BaseEntity
{
    public required string ItemNumber { get; set; }
    public required string ItemName { get; set; }
    public required string Description { get; set; }
    public required InventoryItemCategoryEnum Category { get; set; }
    public required string Brand { get; set; }
    public required string Supplier { get; set; }
    public decimal UnitPrice { get; set; }
    public double WeightKG { get; set; }
    public required Boolean IsActive { get; set; } = true;
    public required string CompatibleVehicleTypes { get; set; }

    // navigation properties 
    public required ICollection<Inventory> Inventories { get; set; } = [];
}
