using Application.Features.ServiceSchedules.Query.GetServiceSchedule;

using MediatR;

namespace Application.Features.ServiceSchedules.Query.GetServiceSchedule;

public record GetServiceScheduleQuery(int ServiceScheduleID) : IRequest<GetServiceScheduleDTO> { }