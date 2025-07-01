namespace Domain.Entities;

public class FuelPurchase : BaseEntity
{
    public required int VehicleId { get; set; }

    public required string PurchasedByUserId { get; set; }

    public required DateTime PurchaseDate { get; set; }

    public required double OdometerReading { get; set; }

    public required double Volume { get; set; }

    public required decimal PricePerUnit { get; set; }

    public required decimal TotalCost { get; set; }

    public required string FuelStation { get; set; }

    public required string ReceiptNumber { get; set; }

    public string? Notes { get; set; }

    // navigation properties
    public required Vehicle Vehicle { get; set; }
    public required User User { get; set; }
}