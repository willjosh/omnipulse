using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents an inspection performed by a technician on a specific vehicle, based on a predefined <see cref="InspectionForm"/> checklist.
/// Stores inspection metadata and associated checklist results.
/// </summary>
public class Inspection : BaseEntity
{
    // FKs
    /// <summary>The ID of the <see cref="InspectionForm"/> used as the template for this inspection.</summary>
    public required int InspectionFormID { get; set; }
    /// <summary>The ID of the <see cref="Vehicle"/> being inspected.</summary>
    public required int VehicleID { get; set; }
    /// <summary>The ID of the <see cref="User"/> (technician) who performed the inspection.</summary>
    public required string TechnicianID { get; set; }

    public required DateTime InspectionStartTime { get; set; }
    public required DateTime InspectionEndTime { get; set; }

    /// <summary>The vehicle's odometer reading at the time of inspection, if available.</summary>
    public double? OdometerReading { get; set; }
    /// <summary>The overall condition of the vehicle as assessed by the technician.</summary>
    public required VehicleConditionEnum VehicleCondition { get; set; }
    public string? Notes { get; set; }

    // Navigation Properties

    /// <summary>The <see cref="InspectionForm"/> used for this inspection.</summary>
    public required InspectionForm InspectionForm { get; set; }

    /// <summary>The collection of pass/fail results for each checklist item in this inspection.</summary>
    public required ICollection<InspectionPassFailItem> InspectionPassFailItems { get; set; } = [];

    /// <summary>The <see cref="Vehicle"/> being inspected.</summary>
    public required Vehicle Vehicle { get; set; }

    /// <summary>The <see cref="User"/> (technician) who performed the inspection.</summary>
    public required User User { get; set; }
}