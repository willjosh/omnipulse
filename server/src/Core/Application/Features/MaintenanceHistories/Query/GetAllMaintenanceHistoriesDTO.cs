using System;

namespace Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoryDTO
{
    public int MaintenanceHistoryID { get; set; }
    public int VehicleID { get; set; }
    public string VehicleName { get; set; } = string.Empty;
    public int WorkOrderID { get; set; }
    public string WorkOrderNumber { get; set; } = string.Empty;
    public int ServiceTaskID { get; set; }
    public string ServiceTaskName { get; set; } = string.Empty;
    public string TechnicianID { get; set; } = string.Empty;
    public string TechnicianName { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public double MileageAtService { get; set; }
    public string? Description { get; set; }
    public decimal Cost { get; set; }
    public double LabourHours { get; set; }
    public string? Notes { get; set; }
} 