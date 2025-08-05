using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;

using MediatR;

namespace Application.Features.Issues.Query.GetOpenIssueData;

public class GetOpenIssueDataQueryHandler : IRequestHandler<GetOpenIssueDataQuery, GetOpenIssueDataDTO>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IAppLogger<GetOpenIssueDataQueryHandler> _logger;

    public GetOpenIssueDataQueryHandler(
        IIssueRepository issueRepository,
        IAppLogger<GetOpenIssueDataQueryHandler> logger)
    {
        _issueRepository = issueRepository;
        _logger = logger;
    }

    public async Task<GetOpenIssueDataDTO> Handle(GetOpenIssueDataQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetOpenIssueDataQuery");

        var openIssueCount = await _issueRepository.GetAllOpenIssuesCountAsync();

        var result = new GetOpenIssueDataDTO
        {
            OpenIssueCount = openIssueCount
        };

        _logger.LogInformation("Handled GetOpenIssueDataQuery successfully");

        return result;
    }
}