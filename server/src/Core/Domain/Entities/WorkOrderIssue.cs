using System;

namespace Domain.Entities;

public class WorkOrderIssue : BaseEntity
{
    public int WorkOrderID { get; set; }
    public int IssueID { get; set; }

    // Navigation Properties
    public required Issue Issue { get; set; }
    // TODO: Connects to WorkOrder
}
