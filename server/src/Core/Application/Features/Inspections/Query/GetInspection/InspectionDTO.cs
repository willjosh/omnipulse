using Domain.Entities.Enums;

namespace Application.Features.Inspections.Query.GetInspection;

/// <summary>
/// Data Transfer Object for detailed inspection view.
/// Contains complete inspection information including all inspection items and related entity details.
/// </summary>
public class InspectionDTO
{
    public int ID { get; set; }
    public int InspectionFormID { get; set; }
    public int VehicleID { get; set; }
    public string TechnicianID { get; set; } = string.Empty;
    public DateTime InspectionStartTime { get; set; }
    public DateTime InspectionEndTime { get; set; }

    // Snapshot fields
    public string SnapshotFormTitle { get; set; } = string.Empty;
    public string? SnapshotFormDescription { get; set; }

    // Inspection Response Data
    public double? OdometerReading { get; set; }
    public List<InspectionItemDTO> InspectionItems { get; set; } = [];
    public VehicleConditionEnum VehicleCondition { get; set; }
    public string? Notes { get; set; }

    // Related entity information
    public VehicleInfoDTO? Vehicle { get; set; }
    public TechnicianInfoDTO? Technician { get; set; }
    public InspectionFormInfoDTO? InspectionForm { get; set; }

    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

/// <summary>
/// Inspection item response information.
/// Contains snapshot data to preserve historical accuracy even if the original form item changes.
/// </summary>
public class InspectionItemDTO
{
    public int InspectionFormItemID { get; set; }
    public bool Passed { get; set; }
    public string? Comment { get; set; }

    // Snapshot fields - preserved from the time of inspection
    public string SnapshotItemLabel { get; set; } = string.Empty;
    public string? SnapshotItemDescription { get; set; }
    public string? SnapshotItemInstructions { get; set; }
    public bool SnapshotIsRequired { get; set; }
    public InspectionFormItemTypeEnum SnapshotInspectionFormItemType { get; set; }
}

/// <summary>
/// Inspection form information for inspection details.
/// </summary>
public class InspectionFormInfoDTO
{
    public int ID { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsActive { get; set; }
}

/// <summary>
/// Vehicle information for inspection details.
/// </summary>
public class VehicleInfoDTO
{
    public int ID { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LicensePlate { get; set; }
    public string? VIN { get; set; }
}

/// <summary>
/// Technician information for inspection details.
/// </summary>
public class TechnicianInfoDTO
{
    public string ID { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
}