using Application.Models.PaginationModels;

using MediatR;

namespace Application.Features.Issues.Query.GetAllIssue;

public record GetAllIssueQuery(PaginationParameters Parameters) : IRequest<PagedResult<GetAllIssueDTO>>;