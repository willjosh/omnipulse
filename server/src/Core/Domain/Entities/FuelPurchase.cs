namespace Domain.Entities;

public class FuelPurchase : BaseEntity
{
    public required int VehicleId { get; set; }

    public required int PurchasedBy { get; set; }

    public required DateTime PurchaseDate { get; set; }

    public required double OdometerReading { get; set; }

    public required double Volume { get; set; }

    public required double PricePerUnit { get; set; }

    public required float TotalCost { get; set; }

    public required string FuelStation { get; set; }

    public required string ReceiptNumber { get; set; }

    public string? Text { get; set; }

}
