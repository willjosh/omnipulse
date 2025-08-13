using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Abstract base class for an inspection response item.
/// Represents a technician's response to a single checklist item during an inspection.
/// </summary>
public abstract class InspectionItemBase
{
    // ===== Composite PK =====
    /// <summary>The ID of the <see cref="Inspection"/> this item belongs to.</summary>
    public required int InspectionID { get; set; }
    /// <summary>The ID of the inspection form checklist item (<see cref="InspectionFormItem"/>) being responded to.</summary>
    public required int InspectionFormItemID { get; set; }

    /// <summary>Optional comment provided by the technician for this item.</summary>
    public string? Comment { get; set; }

    // ===== InspectionFormItem Snapshots =====
    public required string SnapshotItemLabel { get; set; }
    public string? SnapshotItemDescription { get; set; }
    public string? SnapshotItemInstructions { get; set; }
    public required bool SnapshotIsRequired { get; set; }
    public required InspectionFormItemTypeEnum SnapshotInspectionFormItemTypeEnum { get; set; }

    // ===== Navigation Properties =====
    public required Inspection Inspection { get; set; }
    public required InspectionFormItem InspectionFormItem { get; set; }
}