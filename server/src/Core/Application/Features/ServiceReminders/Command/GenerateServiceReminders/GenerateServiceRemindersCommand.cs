using MediatR;

namespace Application.Features.ServiceReminders.Command.GenerateServiceReminders;

/// <summary>
/// Command to sync service reminders from service schedules.
/// This should be called periodically or when service schedules/vehicle assignments change.
/// </summary>
public record SyncServiceRemindersCommand : IRequest<SyncServiceRemindersResponse>;