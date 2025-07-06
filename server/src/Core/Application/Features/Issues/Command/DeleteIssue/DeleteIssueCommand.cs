using MediatR;

namespace Application.Features.Issues.Command.DeleteIssue;

public record DeleteIssueCommand(
    int IssueID
) : IRequest<int>;