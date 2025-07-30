using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.ServiceSchedules.Query.GetAllServiceSchedule;

public record GetAllServiceScheduleQuery(PaginationParameters Parameters) : IRequest<PagedResult<ServiceScheduleDTO>>;