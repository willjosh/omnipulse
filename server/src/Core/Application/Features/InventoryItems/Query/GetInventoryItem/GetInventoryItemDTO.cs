using Domain.Entities.Enums;

namespace Application.Features.InventoryItems.Query.GetInventoryItem;

public class GetInventoryItemDTO
{
    public int ID { get; set; }
    public required string ItemNumber { get; set; }
    public required string ItemName { get; set; }
    public string? Description { get; set; }
    public InventoryItemCategoryEnum? Category { get; set; }
    public string? Manufacturer { get; set; }
    public string? ManufacturerPartNumber { get; set; }
    public string? UniversalProductCode { get; set; }
    public decimal? UnitCost { get; set; }
    public InventoryItemUnitCostMeasurementUnitEnum? UnitCostMeasurementUnit { get; set; }
    public string? Supplier { get; set; }
    public double? WeightKG { get; set; }
    public required bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}