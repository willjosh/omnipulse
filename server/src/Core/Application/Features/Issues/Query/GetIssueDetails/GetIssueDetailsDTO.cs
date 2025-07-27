using Domain.Entities.Enums;

namespace Application.Features.Issues.Query.GetIssueDetails;

public class GetIssueDetailsDTO
{
    public int ID { get; set; }
    public required int IssueNumber { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required IssueStatusEnum Status { get; set; }
    public required PriorityLevelEnum PriorityLevel { get; set; }
    public required IssueCategoryEnum Category { get; set; }
    public required int VehicleID { get; set; }
    public required string VehicleName { get; set; }
    public required string ReportedByUserID { get; set; }
    public required string ReportedByUserName { get; set; }
    public DateTime? ReportedDate { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string? ResolutionNotes { get; set; }
    public string? ResolvedByUserID { get; set; }
    public string? ResolvedByUserName { get; set; }
}