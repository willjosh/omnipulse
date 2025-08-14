namespace Application.Features.FuelPurchases.Query;

public class FuelPurchaseDTO
{
    /// <example>1</example>
    public int ID { get; set; }

    /// <example>1</example>
    public required int VehicleId { get; set; }

    /// <example>Truck A01</example>
    public string? VehicleName { get; set; }

    /// <example>566ae2d4-a781-4690-84c0-f8b284868e43</example>
    public required string PurchasedByUserId { get; set; }

    /// <example>Jane Doe</example>
    public string? PurchasedByUserName { get; set; }

    public required DateTime PurchaseDate { get; set; }

    /// <example>45000.5</example>
    public required double OdometerReading { get; set; }

    /// <example>45.5</example>
    public required double Volume { get; set; }

    /// <example>3.25</example>
    public required decimal PricePerUnit { get; set; }

    /// <example>147.88</example>
    public required decimal TotalCost { get; set; }

    /// <example>Shell Station</example>
    public required string FuelStation { get; set; }

    /// <example>RCPT-2025-001</example>
    public required string ReceiptNumber { get; set; }
}