namespace Domain.Entities;

public class Inventory : BaseEntity
{
    public int InventoryItemID { get; set; }
    public int InventoryItemLocationID { get; set; }
    public int QuantityOnHand { get; set; }
    public int MinStockLevel { get; set; }
    public int MaxStockLevel { get; set; }
    public int ReorderPoint { get; set; }
    public int LastRestockedDate { get; set; }
    public decimal UnitCost { get; set; }

    // Navigation Properties
    public required InventoryItemLocation InventoryItemLocation { get; set; }
    public required InventoryItem InventoryItem { get; set; }
}