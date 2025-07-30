namespace Domain.Entities;

public class MaintenanceHistory : BaseEntity
{
    public required int WorkOrderID { get; set; }
    public required DateTime ServiceDate { get; set; }
    public required double MileageAtService { get; set; }

    // navigation properties
    public required WorkOrder WorkOrder { get; set; }
}