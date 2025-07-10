using Application.Features.ServiceSchedules.Query.GetServiceSchedule;

using MediatR;

public record GetServiceScheduleQuery(int ServiceScheduleID) : IRequest<GetServiceScheduleDTO> { }