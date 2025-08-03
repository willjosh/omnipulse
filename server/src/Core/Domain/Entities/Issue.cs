namespace Domain.Entities;
using Domain.Entities.Enums;

public class Issue : BaseEntity
{
    public required int VehicleID { get; set; }
    public required int IssueNumber { get; set; }
    public required string ReportedByUserID { get; set; }
    public DateTime? ReportedDate { get; set; } = DateTime.UtcNow;
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required IssueCategoryEnum Category { get; set; }
    public required PriorityLevelEnum PriorityLevel { get; set; }
    public required IssueStatusEnum Status { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? ResolvedByUserID { get; set; }
    public string? ResolutionNotes { get; set; }

    public int? InspectionID { get; set; }

    // Navigation Properties
    public required ICollection<IssueAttachment> IssueAttachments { get; set; } = [];
    public required ICollection<IssueAssignment> IssueAssignments { get; set; } = [];
    public required Vehicle Vehicle { get; set; }
    public required User ReportedByUser { get; set; }
    public User? ResolvedByUser { get; set; }
}