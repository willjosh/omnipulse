using Domain.Entities.Enums;

namespace Application.Features.InventoryItems.Query.GetAllInventoryItem;

public class GetAllInventoryItemDTO
{
    /// <example>1</example>
    public int ID { get; set; }

    /// <example>INV-001</example>
    public required string ItemNumber { get; set; }

    /// <example>Engine Oil Filter</example>
    public required string ItemName { get; set; }

    /// <example>High-quality oil filter for diesel engines</example>
    public string? Description { get; set; }

    /// <example>1</example>
    public InventoryItemCategoryEnum? Category { get; set; }

    /// <example>Fram</example>
    public string? Manufacturer { get; set; }

    /// <example>PH2870A</example>
    public string? ManufacturerPartNumber { get; set; }

    /// <example>123456789012</example>
    public string? UniversalProductCode { get; set; }

    /// <example>15.99</example>
    public decimal? UnitCost { get; set; }

    /// <example>1</example>
    public InventoryItemUnitCostMeasurementUnitEnum? UnitCostMeasurementUnit { get; set; }

    /// <example>AutoZone</example>
    public string? Supplier { get; set; }

    /// <example>0.5</example>
    public double? WeightKG { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; } = true;
}