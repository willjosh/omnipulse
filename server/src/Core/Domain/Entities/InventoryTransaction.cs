using Domain.Entities.Enums;

namespace Domain.Entities;

public class InventoryTransaction : BaseEntity
{
    public required int InventoryID { get; set; }
    public required TransactionTypeEnum TransactionType { get; set; }
    public required int Quantity { get; set; }
    public required decimal UnitCost { get; set; }
    public required decimal TotalCost { get; set; }
    public required string PerformedByUserID { get; set; }

    // navigation properties
    public required Inventory Inventory { get; set; }
    public required User User { get; set; }
}