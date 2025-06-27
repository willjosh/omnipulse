namespace Domain.Entities;

public class Inventory : BaseEntity
{
    public required int InventoryItemID { get; set; }
    public required int InventoryItemLocationID { get; set; }
    public required int QuantityOnHand { get; set; }
    public required int MinStockLevel { get; set; }
    public required int MaxStockLevel { get; set; }
    public required int ReorderPoint { get; set; }
    public required int LastRestockedDate { get; set; }
    public required decimal UnitCost { get; set; }

    // Navigation Properties
    public required InventoryItemLocation InventoryItemLocation { get; set; }
    public required InventoryItem InventoryItem { get; set; }
    public required ICollection<InventoryTransaction> InventoryTransactions { get; set; } = [];
}