using System;

namespace Application.Features.WorkOrders.Query.GetWorkOrderDetail;

public class GetWorkOrderDetailTO
{
    // Work Order
    public int ID { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string WorkOrderType { get; set; }
    public required string PriorityLevel { get; set; }
    public required string Status { get; set; }
    public required DateTime? ScheduledStartDate { get; set; }
    public DateTime? ActualStartDate { get; set; }
    public required double StartOdometer { get; set; }
    public double? EndOdometer { get; set; } 

    // Vehicle
    public required int VehicleID { get; set; }
    public required string VehicleName { get; set; }

    // Assigned User
    public required string AssignedToUserID { get; set; }
    public required string AssignedToUserName { get; set; } 
}