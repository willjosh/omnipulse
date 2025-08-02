using Domain.Entities;

namespace Application.Features.InspectionForms.Query;

/// <summary>
/// Detailed information about a single <see cref="InspectionForm"/>.
/// </summary>
public class InspectionFormDTO
{
    /// <example>1</example>
    public required int ID { get; set; }

    /// <example>Bus Inspection Form</example>
    public required string Title { get; set; }

    /// <example>Comprehensive safety inspection checklist for all buses</example>
    public string? Description { get; set; }

    /// <example>true</example>
    public required bool IsActive { get; set; }

    public required DateTime CreatedAt { get; set; }
    public required DateTime UpdatedAt { get; set; }

    /// <summary>Number of <see cref="Inspection"/> that have used this inspection form.</summary>
    public int InspectionCount { get; set; }

    /// <summary>Number of <see cref="InspectionFormItem"/> defined in this inspection form.</summary>
    public int InspectionFormItemCount { get; set; }
}