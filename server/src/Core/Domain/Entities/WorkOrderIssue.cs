using System;

namespace Domain.Entities;

public class WorkOrderIssue : BaseEntity
{
    public required int WorkOrderID { get; set; }
    public required int IssueID { get; set; }

    // Navigation Properties
    public required Issue Issue { get; set; }
    // TODO: Connects to WorkOrder
}
