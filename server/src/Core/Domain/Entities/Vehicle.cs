using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class Vehicle : BaseEntity
{
    public required string Name { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public required string Year { get; set; }
    public required string VIN { get; set; }
    public required string LicensePlate { get; set; }
    public required DateTime LicensePlateExpirationDate { get; set; }
    public required VehicleTypeEnum VehicleType { get; set; }
    public required string Trim { get; set; }
    public required double Mileage { get; set; }
    public required double EngineHours { get; set; }
    public required double FuelCapacity { get; set; }
    public required FuelTypeEnum FuelType { get; set; }
    public required DateTime PurchaseDate { get; set; }
    public required string PurchasePrice { get; set; }
    public required VehicleStatusEnum Status { get; set; }
    public required string Location { get; set; }

    // Navigation properties TODO: Add navigation properties
}
