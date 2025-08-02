namespace Domain.Entities;

public class Inventory : BaseEntity
{
    public static Inventory CreateDefaultInventory(int inventoryItemID, decimal unitCost)
    {
        return new Inventory
        {
            ID = 0, // Assuming ID is auto-generated
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InventoryItemID = inventoryItemID,
            QuantityOnHand = 0,
            MinStockLevel = 0,
            MaxStockLevel = 0,
            NeedsReorder = false,
            LastRestockedDate = null,
            UnitCost = unitCost,
            InventoryItemLocation = null!,
            InventoryItem = null!,
            InventoryTransactions = [],
        };
    }

    public required int InventoryItemID { get; set; }
    public int? InventoryItemLocationID { get; set; }
    public required int QuantityOnHand { get; set; }
    public required int MinStockLevel { get; set; }
    public required int MaxStockLevel { get; set; }
    public required bool NeedsReorder { get; set; } = false;
    public DateTime? LastRestockedDate { get; set; }
    public required decimal UnitCost { get; set; }

    // Navigation Properties
    public InventoryItemLocation? InventoryItemLocation { get; set; }
    public required InventoryItem InventoryItem { get; set; }
    public required ICollection<InventoryTransaction> InventoryTransactions { get; set; } = [];
}