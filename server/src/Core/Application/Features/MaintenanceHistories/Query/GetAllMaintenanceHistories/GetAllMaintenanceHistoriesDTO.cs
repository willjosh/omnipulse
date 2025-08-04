namespace Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoryDTO
{
    public int MaintenanceHistoryID { get; set; }
    public int VehicleID { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public int WorkOrderID { get; set; }
    public int[] ServiceTaskID { get; set; } = Array.Empty<int>();
    public string[] ServiceTaskName { get; set; } = Array.Empty<string>();
    public string TechnicianID { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public double MileageAtService { get; set; }
    public decimal Cost { get; set; }
    public double LabourHours { get; set; }
    public string? Notes { get; set; }
}