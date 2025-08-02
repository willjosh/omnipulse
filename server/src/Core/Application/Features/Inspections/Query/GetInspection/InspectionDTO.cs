using Domain.Entities.Enums;

namespace Application.Features.Inspections.Query.GetInspection;

/// <summary>
/// Data Transfer Object for detailed inspection view.
/// Contains complete inspection information including all inspection items and related entity details.
/// </summary>
public class InspectionDTO
{
    public required int ID { get; set; }
    public required int InspectionFormID { get; set; }
    public required int VehicleID { get; set; }
    public required string TechnicianID { get; set; }
    public required DateTime InspectionStartTime { get; set; }
    public required DateTime InspectionEndTime { get; set; }

    // Snapshot fields
    public required string SnapshotFormTitle { get; set; }
    public string? SnapshotFormDescription { get; set; }

    // Inspection Response Data
    public double? OdometerReading { get; set; }
    public required List<InspectionItemDTO> InspectionItems { get; set; } = [];
    public required VehicleConditionEnum VehicleCondition { get; set; }
    public string? Notes { get; set; }

    // Related entity information
    public VehicleInfoDTO? Vehicle { get; set; }
    public TechnicianInfoDTO? Technician { get; set; }
    public InspectionFormInfoDTO? InspectionForm { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Inspection item response information.
/// Contains snapshot data to preserve historical accuracy even if the original form item changes.
/// </summary>
public class InspectionItemDTO
{
    public required int InspectionFormItemID { get; set; }
    public required bool Passed { get; set; }
    public string? Comment { get; set; }

    // Snapshot fields - preserved from the time of inspection
    public required string SnapshotItemLabel { get; set; }
    public string? SnapshotItemDescription { get; set; }
    public string? SnapshotItemInstructions { get; set; }
    public required bool SnapshotIsRequired { get; set; }
    public required InspectionFormItemTypeEnum SnapshotInspectionFormItemType { get; set; }
}

/// <summary>
/// Inspection form information for inspection details.
/// </summary>
public class InspectionFormInfoDTO
{
    public required int ID { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required bool IsActive { get; set; }
}

/// <summary>
/// Vehicle information for inspection details.
/// </summary>
public class VehicleInfoDTO
{
    public required int ID { get; set; }
    public required string Name { get; set; }
    public required string LicensePlate { get; set; }
    public required string VIN { get; set; }
}

/// <summary>
/// Technician information for inspection details.
/// </summary>
public class TechnicianInfoDTO
{
    public required string ID { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string Email { get; set; }
}