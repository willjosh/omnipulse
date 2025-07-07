using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public record CreateWorkOrderCommand(
    string WorkOrderNumber,
    int VehicleId,
    int ServiceReminderId,
    string AssignedToUserId,
    string Title,
    string? Description,
    WorkTypeEnum WorkOrderType,
    PriorityLevelEnum PriorityLevel,
    WorkOrderStatusEnum Status,
    decimal? EstimatedCost,
    decimal? ActualCost,
    double? EstimatedHours,
    double? ActualHours,
    DateTime? ScheduledStartDate,
    DateTime? ActualStartDate,
    double StartOdometer,
    double? EndOdometer
) : IRequest<int> { }