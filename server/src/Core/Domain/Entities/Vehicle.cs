using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

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
    public required int AssignedTechnicianID { get; set; }
    public required string Trim { get; set; }
    public required double Mileage { get; set; }
    public required double EngineHours { get; set; }
    public required double FuelCapacity { get; set; }
    public required FuelTypeEnum FuelType { get; set; }
    public required DateTime PurchaseDate { get; set; }
    public required double PurchasePrice { get; set; }
    public required VehicleStatusEnum Status { get; set; }
    public required string Location { get; set; }

    // Navigation properties
    public required VehicleGroup VehicleGroup { get; set; }

    public required ICollection<VehicleInspection> VehicleInspections { get; set; }

    public required ICollection<MaintenanceHistory> MaintenanceHistories { get; set; }

    public required ICollection<VehicleServiceProgram> VehicleServicePrograms { get; set; }
    public required ICollection<Issue> Issues { get; set; }
    // TODO: Add navigation properties for AssignedTechnician

}
