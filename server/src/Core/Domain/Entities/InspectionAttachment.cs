using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class InspectionAttachment : BaseEntity
{
    public required int VehicleInspectionID { get; set; }
    public required int CheckListItemID { get; set; }
    public required AttachmentTypeEnum FileType { get; set; }
    public required string FileName { get; set; }
    public required string FilePath { get; set; }
    public required string FileSize { get; set; }
    public string? Description { get; set; }
    
    // navigation properties
    public required VehicleInspection VehicleInspection { get; set; }
    public required CheckListItem CheckListItem { get; set; }
}
