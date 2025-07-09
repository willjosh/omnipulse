using Application.Features.WorkOrderLineItem.Models;

using Domain.Entities.Enums;

using MediatR;

namespace Application.Features.WorkOrders.Command.CreateWorkOrder;

public record CreateWorkOrderCommand(
    int VehicleID,
    int ServiceTaskID,
    string AssignedToUserID,
    string Title,
    string? Description,
    WorkTypeEnum WorkOrderType,
    PriorityLevelEnum PriorityLevel,
    WorkOrderStatusEnum Status,
    DateTime? ScheduledStartDate,
    DateTime? ActualStartDate,
    double StartOdometer,
    double? EndOdometer,
    // Issues
    List<int>? IssueIdList,
    // Work Order Line Items
    List<CreateWorkOrderLineItemDTO>? WorkOrderLineItems
) : IRequest<int>
{ }