using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.ServiceReminders.Query.GetAllServiceReminders;

/// <summary>
/// Query to get all calculated service reminders for all vehicles in the system.
/// This generates multiple reminder rows for each service schedule occurrence (overdue, current, upcoming).
/// </summary>
public record GetAllServiceRemindersQuery(PaginationParameters Parameters) : IRequest<PagedResult<ServiceReminderDTO>>;