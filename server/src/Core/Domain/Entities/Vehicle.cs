using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a vehicle entity in the fleet management system.
/// </summary>
/// <remarks>
/// This is a core domain entity that contains all essential information about a fleet vehicle.
///
/// Key business rules:
/// - VIN (Vehicle Identification Number) must be unique across the entire fleet
/// - License plate must be unique and have a valid expiration date
/// - Vehicle must belong to exactly one VehicleGroup
/// - Mileage and engine hours can only increase over time
/// </remarks>
public class Vehicle : BaseEntity
{
    public required string Name { get; set; }
    public required string Make { get; set; }
    public required string Model { get; set; }
    public required int Year { get; set; }
    public required string VIN { get; set; }
    public required string LicensePlate { get; set; }
    public required DateTime LicensePlateExpirationDate { get; set; }
    public required VehicleTypeEnum VehicleType { get; set; }
    public required int VehicleGroupID { get; set; }
    public string? AssignedTechnicianID { get; set; }
    public required string Trim { get; set; }
    public required double Mileage { get; set; }
    public required double EngineHours { get; set; }
    public required double FuelCapacity { get; set; }
    public required FuelTypeEnum FuelType { get; set; }
    public required DateTime PurchaseDate { get; set; }
    public required decimal PurchasePrice { get; set; }
    public required VehicleStatusEnum Status { get; set; }
    public required string Location { get; set; }

    // Navigation Properties
    public User? User { get; set; }
    public required VehicleGroup VehicleGroup { get; set; }
    public required ICollection<VehicleImage> VehicleImages { get; set; } = [];
    public required ICollection<VehicleAssignment> VehicleAssignments { get; set; } = [];
    public required ICollection<VehicleDocument> VehicleDocuments { get; set; } = [];
    public required ICollection<XrefServiceProgramVehicle> XrefServiceProgramVehicles { get; set; } = [];
    public required ICollection<ServiceReminder> ServiceReminders { get; set; } = [];
    public required ICollection<Issue> Issues { get; set; } = [];
    public required ICollection<VehicleInspection> VehicleInspections { get; set; } = [];
}