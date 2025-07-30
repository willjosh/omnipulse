using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.ServicePrograms.Query.GetAllServiceProgram;

public record GetAllServiceProgramQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllServiceProgramDTO>>;