using MediatR;

namespace Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;

public record CreateMaintenanceHistoryCommand(
    int WorkOrderID,
    DateTime ServiceDate,
    double MileageAtService,
    string? Description,
    decimal Cost,
    double LabourHours,
    string? Notes
) : IRequest<int>;