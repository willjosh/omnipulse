using System;
using System.Net;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class InspectionChecklistResponse : BaseEntity
{
    public required int VehicleInspectionID { get; set; }
    public required int ChecklistItemID { get; set; }
    public required InspectionItemStatusEnum Status { get; set; }
    public string? TextResponse { get; set; }
    public string? Note { get; set; }
    public required Boolean RequiresAttention { get; set; } = true;

    // navigation properties
    public required CheckListItem CheckListItem { get; set; }
    public required VehicleInspection VehicleInspection { get; set; }
}
