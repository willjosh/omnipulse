using Domain.Entities;
using Domain.Entities.Enums;

namespace Application.Features.InspectionFormItems.Query.GetAllInspectionFormItem;

/// <summary>
/// Data Transfer Object representing an <see cref="InspectionFormItem"/> in a list.
/// </summary>
public class GetAllInspectionFormItemDTO
{
    /// <example>1</example>
    public required int ID { get; set; }

    /// <example>1</example>
    public required int InspectionFormID { get; set; }

    /// <example>Check Engine Oil</example>
    public required string ItemLabel { get; set; }

    /// <example>Verify engine oil level and quality</example>
    public string? ItemDescription { get; set; }

    /// <example>Remove dipstick, check oil level between min/max marks</example>
    public string? ItemInstructions { get; set; }

    public required bool IsRequired { get; set; }
    public required InspectionFormItemTypeEnum InspectionFormItemTypeEnum { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}