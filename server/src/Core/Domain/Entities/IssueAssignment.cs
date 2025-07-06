using System;

namespace Domain.Entities;

public class IssueAssignment
{
    public required int IssueID { get; set; }
    public required string AssignedToUserID { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;
    public DateTime? UnassignedDate { get; set; }
    public required bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation Properties
    public required Issue Issue { get; set; }
    public required User User { get; set; }
}