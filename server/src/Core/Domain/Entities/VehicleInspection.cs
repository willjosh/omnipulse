using System;
using Domain.Entities.Enums;

namespace Domain.Entities;

public class VehicleInspection : BaseEntity
{
    public required int VehicleID { get; set; }
    public required int InspectionTypeID { get; set; }
    public required int TechnicianID { get; set; }
    public required DateTime InspectionDate { get; set; }
    public required DateTime StartTime { get; set; }
    public required DateTime EndTime { get; set; }
    public required double MileageAtInspection { get; set; }
    public required OverallStatusEnum OverallStatus { get; set; }
    public required string Notes { get; set; }
    public required Boolean IsPassed { get; set; } = true;

    // navigation properties
    public required Vehicle Vehicle { get; set; }
    public required InspectionType InspectionType { get; set; }
    public required User User { get; set; }
}
