namespace Domain.Entities.Enums;

/// <summary>
/// Represents the expected response type for an inspection checklist item.
/// Determines how technicians will complete the inspection checklist item during inspection.
/// </summary>
public enum InspectionFormItemTypeEnum
{
    PassFail = 1, // Boolean
    // Text, // Free-form comments
    // Numeric, // e.g., tyre pressure
    // Photo, // Photo evidence
    // Dropdown // Select from predefined options
}