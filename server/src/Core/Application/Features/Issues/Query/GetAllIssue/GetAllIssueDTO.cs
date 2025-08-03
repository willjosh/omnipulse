using Domain.Entities.Enums;

namespace Application.Features.Issues.Query.GetAllIssue;

public class GetAllIssueDTO
{
    /// <example>1</example>
    public int ID { get; set; }

    /// <example>1</example>
    public required int VehicleID { get; set; }

    /// <example>BYD K9 MAL‑315</example>
    public required string VehicleName { get; set; } // from Vehicle.Name

    /// <example>Engine overheating</example>
    public required string Title { get; set; }

    /// <example>Engine temperature gauge showing high readings during operation.</example>
    public string? Description { get; set; }

    /// <example>1</example>
    public required IssueCategoryEnum Category { get; set; }

    /// <example>2</example>
    public required PriorityLevelEnum PriorityLevel { get; set; }

    /// <example>1</example>
    public required IssueStatusEnum Status { get; set; }

    /// <example>566ae2d4-a781-4690-84c0-f8b284868e43</example>
    public required string ReportedByUserID { get; set; }

    /// <example>John Smith</example>
    public required string ReportedByUserName { get; set; } // from User.FirstName + " " + User.LastName

    public DateTime? ReportedDate { get; set; }

    public DateTime? ResolvedDate { get; set; }

    /// <example>566ae2d4-a781-4690-84c0-f8b284868e43</example>
    public string? ResolvedByUserID { get; set; }

    /// <example>Mike Johnson</example>
    public string? ResolvedByUserName { get; set; } // from User.FirstName + " " + User.LastName

    /// <example>Replaced thermostat and coolant. Engine temperature now normal.</example>
    public string? ResolutionNotes { get; set; }
}