namespace Domain.Entities;

/// <summary>
/// Represents a pass/fail response to a specific form checklist item (<see cref="InspectionFormItem"/>) in an inspection (<see cref="Inspection"/>).
/// </summary>
public class InspectionPassFailItem : InspectionItemBase
{
    public required bool Passed { get; set; } = false;
}