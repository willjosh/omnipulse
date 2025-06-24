using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class CheckListItem : BaseEntity
{
    public required int InspectionTypeID { get; set; }
    public required string Category { get; set; }
    public required string ItemName { get; set; }
    public string? Description { get; set; }
    public required InputTypeEnum InputType { get; set; }
    public required Boolean IsMandatory { get; set; } = true;
    
    // navigation properties
    public required InspectionType Inspection { get; set; }
}
