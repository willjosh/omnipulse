using Domain.Entities.Enums;

namespace Application.Features.Vehicles.Query.GetVehicleDetails;

/// <summary>
/// Details of a vehicle.
/// </summary>
public class GetVehicleDetailsDTO
{
    /// <example>1</example>
    public int ID { get; set; }

    /// <example>BYD K9 MAL‑315</example>
    public required string Name { get; set; }

    /// <example>BYD</example>
    public required string Make { get; set; }

    /// <example>K9</example>
    public required string Model { get; set; }

    /// <example>2020</example>
    public required int Year { get; set; }

    /// <example>4B9KSLA63J2038003</example>
    public required string VIN { get; set; }

    /// <example>VBA 8012</example>
    public required string LicensePlate { get; set; }

    public required DateTime LicensePlateExpirationDate { get; set; }

    /// <example>2</example>
    public required VehicleTypeEnum VehicleType { get; set; }

    /// <example>1</example>
    public required int VehicleGroupID { get; set; }

    /// <example>RapidKL Electric Buses</example>
    public required string VehicleGroupName { get; set; }

    /// <example>Johor Bahru Driver</example>
    public required string AssignedTechnicianName { get; set; }

    /// <example>566ae2d4-a781-4690-84c0-f8b284868e43</example>
    public string? AssignedTechnicianID { get; set; }

    /// <example>K9 Standard EV</example>
    public required string Trim { get; set; }

    /// <example>12000.0</example>
    public required double Mileage { get; set; }

    /// <example>0.0</example>
    public required double FuelCapacity { get; set; }

    /// <example>4</example>
    public required FuelTypeEnum FuelType { get; set; }

    public required DateTime PurchaseDate { get; set; }

    /// <example>1600000.00</example>
    public required decimal PurchasePrice { get; set; }

    /// <example>1</example>
    public required VehicleStatusEnum Status { get; set; }

    /// <example>RapidKL Depo Cheras, Kuala Lumpur</example>
    public required string Location { get; set; }
}