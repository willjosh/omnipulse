namespace Domain.Entities;

public class FuelPurchase : BaseEntity
{
    public required int Id { get; set; }
    public DateTime? Created_at { get; set; }

    public DateTime? Updated_at { get; set; }

    public required int Vehicle_id { get; set; }

    public required int Purchased_by { get; set; }

    public required DateTime Purchase_date { get; set; }

    public required double Odometer_reading { get; set; }

    public required double Volume { get; set; }

    public required float Price_per_unit { get; set; }

    public required float Total_cost { get; set; }

    public required string Fuel_station { get; set; }

    public required string Receipt_number { get; set; }

    public string? Text { get; set; } 

}
