using System;

using MediatR;

namespace Application.Features.Issues.Query.GetIssueDetails;

public record GetIssueDetailsQuery(int IssueID) : IRequest<GetIssueDetailsDTO> { }