namespace Domain.Entities;
using Domain.Entities.Enums;

public class Issue : BaseEntity
{
    public int VehicleID { get; set; }
    public int IssueNumber { get; set; }
    public int ReportedBy { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public IssueCategoryEnum Category { get; set; }
    public PriorityLevelEnum PriorityLevel { get; set; }
    public IssueStatusEnum Status { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public int ResolvedBy { get; set; }
    public string? ResolutionNotes { get; set; }

    // Navigation Properties
    public required ICollection<IssueAttachment> IssueAttachment { get; set; }
    public required ICollection<WorkOrderIssue> WorkOrderIssue { get; set; }
    // TODO: Connects to InspectionAttachment, User
}
