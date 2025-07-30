namespace Domain.Entities;

public class WorkOrderIssue
{
    public required int WorkOrderID { get; set; }
    public required int IssueID { get; set; }
    public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

    // Navigation Properties
    public required Issue Issue { get; set; }
    public required WorkOrder WorkOrder { get; set; }
}