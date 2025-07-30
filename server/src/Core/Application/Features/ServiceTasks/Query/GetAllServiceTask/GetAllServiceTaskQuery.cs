using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.ServiceTasks.Query.GetAllServiceTask;

public record GetAllServiceTaskQuery(PaginationParameters Parameters) : IRequest<PagedResult<ServiceTaskDTO>> { }