using Domain.Entities.Enums;

namespace Domain.Entities;

public class InspectionChecklistResponse
{
    public required int VehicleInspectionID { get; set; }
    public required int ChecklistItemID { get; set; }
    public required InspectionItemStatusEnum Status { get; set; }
    public string? TextResponse { get; set; }
    public string? Note { get; set; }
    public required bool RequiresAttention { get; set; } = true;
    public DateTime? ResponseDate { get; set; } = DateTime.UtcNow;

    // navigation properties
    public required CheckListItem CheckListItem { get; set; }
    public required VehicleInspection VehicleInspection { get; set; }
}