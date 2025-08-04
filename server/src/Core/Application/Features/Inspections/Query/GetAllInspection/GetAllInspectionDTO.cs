using Domain.Entities.Enums;

namespace Application.Features.Inspections.Query.GetAllInspection;

/// <summary>
/// Data Transfer Object for inspection list view.
/// </summary>
public class GetAllInspectionDTO
{
    // PK & FKs
    public required int ID { get; set; }
    public required int InspectionFormID { get; set; }
    public required int VehicleID { get; set; }
    public required string TechnicianID { get; set; }

    // Time
    public required DateTime InspectionStartTime { get; set; }
    public required DateTime InspectionEndTime { get; set; }

    public double? OdometerReading { get; set; }
    public required VehicleConditionEnum VehicleCondition { get; set; }
    public string? Notes { get; set; }

    // Snapshot fields
    public required string SnapshotFormTitle { get; set; }
    public string? SnapshotFormDescription { get; set; }

    // Related entity information
    public required string InspectionFormName { get; set; }
    public required string VehicleName { get; set; }
    public required string TechnicianName { get; set; }

    // Summary information
    public required int InspectionItemsCount { get; set; }
    public required int PassedItemsCount { get; set; }
    public required int FailedItemsCount { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}