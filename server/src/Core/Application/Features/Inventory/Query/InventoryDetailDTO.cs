namespace Application.Features.Inventory.Query;

public class InventoryDetailDTO
{
    public required int ID { get; set; }
    public required string InventoryItemName { get; set; }
    public required string LocationName { get; set; }
    public required int QuantityOnHand { get; set; }
    public required int MinStockLevel { get; set; }
    public required int MaxStockLevel { get; set; }
    public required bool NeedsReorder { get; set; }
    public DateTime? LastRestockedDate { get; set; }
    public required decimal UnitCost { get; set; }
}