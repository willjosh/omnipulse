using Domain.Entities.Enums;

namespace Domain.Entities;

/// <summary>
/// Represents a single checklist item within an <see cref="InspectionForm"/>.
/// This defines what technicians are required to check, but does not store actual inspection results.
/// </summary>
public class InspectionFormItem : BaseEntity
{
    // FK
    /// <summary>Parent <see cref="InspectionForm"/>.</summary>
    public required int InspectionFormID { get; set; }

    public required string ItemLabel { get; set; }
    public string? ItemDescription { get; set; }
    public string? ItemInstructions { get; set; }
    public required bool IsRequired { get; set; } = true;

    /// <summary>The type of expected input for this checklist item (currently only Pass/Fail is supported).</summary>
    public required InspectionFormItemTypeEnum InspectionFormItemTypeEnum { get; set; } = InspectionFormItemTypeEnum.PassFail;

    /// <summary><c>false</c> = soft-deleted</summary>
    public required bool IsActive { get; set; } = true;

    // Navigation Properties

    /// <summary>The parent <see cref="InspectionForm"/> that contains this checklist item.</summary>
    public required InspectionForm InspectionForm { get; set; }
}