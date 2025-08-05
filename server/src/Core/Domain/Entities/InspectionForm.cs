namespace Domain.Entities;

/// <summary>
/// Represents a reusable template for vehicle inspections (<see cref="Inspection"/>).
/// This entity defines the structure of an inspection form, but does not hold inspection results/responses.
/// </summary>
public class InspectionForm : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    /// <summary><c>false</c> = soft-deleted</summary>
    public required bool IsActive { get; set; } = true;

    // Navigation Properties

    /// <summary>The collection of actual inspections that have been performed using this form.</summary>
    public required ICollection<Inspection> Inspections { get; set; } = [];

    /// <summary>The collection of checklist items that define what needs to be inspected in this form.</summary>
    public required ICollection<InspectionFormItem> InspectionFormItems { get; set; } = [];
}