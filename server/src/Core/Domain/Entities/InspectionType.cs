using System;

namespace Domain.Entities;

public class InspectionType : BaseEntity
{
    public required string Name { get; set; }
    public string? Description { get; set; }
    public required Boolean IsActive { get; set; } = true;

    // navigation properties
    public required ICollection<VehicleInspection> VehicleInspections { get; set; }
    public required ICollection<CheckListItem> CheckListItems { get; set; }
}
