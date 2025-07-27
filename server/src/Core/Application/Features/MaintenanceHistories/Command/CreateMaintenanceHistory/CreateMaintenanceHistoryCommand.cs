using MediatR;

namespace Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;

public record CreateMaintenanceHistoryCommand(
    int VehicleID,
    int WorkOrderID,
    int ServiceTaskID,
    string TechnicianID,
    DateTime ServiceDate,
    double MileageAtService,
    string? Description,
    decimal Cost,
    double LabourHours,
    string? Notes
) : IRequest<int>;