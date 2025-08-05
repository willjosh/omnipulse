using System;

using MediatR;

namespace Application.Features.Issues.Query.GetOpenIssueData;

public record GetOpenIssueDataQuery : IRequest<GetOpenIssueDataDTO>
{

}