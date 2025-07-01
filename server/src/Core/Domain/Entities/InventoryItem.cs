using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class InventoryItem : BaseEntity
{
    public required string ItemNumber { get; set; }
    public required string ItemName { get; set; }
    public string? Description { get; set; }
    public InventoryItemCategoryEnum? Category { get; set; }
    public string? Manufacturer { get; set; }
    public string? ManufacturerPartNumber { get; set; } // MPNs are unique within a manufacturer
    public string? UniversalProductCode { get; set; } // https://en.wikipedia.org/wiki/Universal_Product_Code We use UPC-A (12 digits)
    public decimal? UnitCost { get; set; }
    public InventoryItemUnitCostMeasurementUnitEnum? UnitCostMeasurementUnit { get; set; }
    public string? Supplier { get; set; }
    public double? WeightKG { get; set; }
    public required Boolean IsActive { get; set; } = true;
    public string? CompatibleVehicleTypes { get; set; }

    // Navigation Properties
    public required ICollection<Inventory> Inventories { get; set; } = [];
    public required ICollection<WorkOrderLineItem> WorkOrderLineItems { get; set; } = [];
}
