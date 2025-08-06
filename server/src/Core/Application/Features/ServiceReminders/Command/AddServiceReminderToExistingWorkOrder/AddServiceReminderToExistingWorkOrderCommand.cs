using Domain.Entities;
using MediatR;

namespace Application.Features.ServiceReminders.Command.AddServiceReminderToExistingWorkOrder;

/// <summary>
/// Command for linking a service reminder to an existing work order.
/// </summary>
/// <param name="ServiceReminderID">The ID of the <see cref="ServiceReminder"/> to link.</param>
/// <param name="WorkOrderID">The ID of the existing <see cref="WorkOrder"/> to link to.</param>
/// <returns>The ID of the work order that was linked.</returns>
public record AddServiceReminderToExistingWorkOrderCommand(
    int ServiceReminderID,
    int WorkOrderID
) : IRequest<int>;