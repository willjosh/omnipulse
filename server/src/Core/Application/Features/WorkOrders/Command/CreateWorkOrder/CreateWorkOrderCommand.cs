using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public record CreateWorkOrderCommand(
    int VehicleID,
    int ServiceReminderID,
    string AssignedToUserID,
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
    double? EndOdometer,
    // Issues
    List<int>? IssueIdList
) : IRequest<int>
{ }