using System;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using AutoMapper;

using Domain.Entities;

using MediatR;

namespace Application.Features.Issues.Query.GetIssueDetails;

public class GetIssueDetailsQueryHandler : IRequestHandler<GetIssueDetailsQuery, GetIssueDetailsDTO>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetIssueDetailsQueryHandler> _logger;

    public GetIssueDetailsQueryHandler(
        IIssueRepository issueRepository,
        IMapper mapper,
        IAppLogger<GetIssueDetailsQueryHandler> logger)
    {
        _issueRepository = issueRepository;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<GetIssueDetailsDTO> Handle(GetIssueDetailsQuery request, CancellationToken cancellationToken)
    {
        // Get issue by id
        _logger.LogInformation($"GetIssueDetailsQuery for IssueID: {request.IssueID}");
        var issue = await _issueRepository.GetIssueWithDetailsAsync(request.IssueID);

        // check if issue exists
        if (issue == null)
        {
            _logger.LogError($"Issue with ID {request.IssueID} not found.");
            throw new EntityNotFoundException(typeof(Issue).ToString(), "IssueID", request.IssueID.ToString());
        }

        // map to GetIssueDetailsDTO
        var issueDetailsDto = _mapper.Map<GetIssueDetailsDTO>(issue);

        // return GetIssueDetailsDTO
        _logger.LogInformation($"Returning IssueDetailsDTO for IssueID: {request.IssueID}");
        return issueDetailsDto;
    }
}