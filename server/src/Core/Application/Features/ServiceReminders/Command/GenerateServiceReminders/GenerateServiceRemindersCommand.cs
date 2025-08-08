using MediatR;

namespace Application.Features.ServiceReminders.Command.GenerateServiceReminders;

/// <summary>
/// Command to generate and sync service reminders from service schedules.
/// This should be called periodically or when service schedules/vehicle assignments change.
/// </summary>
public record GenerateServiceRemindersCommand : IRequest<GenerateServiceRemindersResponse>;