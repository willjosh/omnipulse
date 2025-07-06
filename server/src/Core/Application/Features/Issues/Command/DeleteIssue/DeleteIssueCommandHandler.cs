using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;

using Domain.Entities;

using MediatR;

namespace Application.Features.Issues.Command.DeleteIssue;

public class DeleteIssueCommandHandler : IRequestHandler<DeleteIssueCommand, int>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IAppLogger<DeleteIssueCommandHandler> _logger;

    public DeleteIssueCommandHandler(IIssueRepository issueRepository, IAppLogger<DeleteIssueCommandHandler> logger)
    {
        _issueRepository = issueRepository;
        _logger = logger;
    }

    public async Task<int> Handle(DeleteIssueCommand request, CancellationToken cancellationToken)
    {
        // Validate IssueID
        _logger.LogInformation($"Validating Issue with ID: {request.IssueID}");
        var issue = await _issueRepository.GetByIdAsync(request.IssueID);
        if (issue == null)
        {
            _logger.LogError($"Issue with ID {request.IssueID} not found.");
            throw new EntityNotFoundException(typeof(Issue).ToString(), "ID", request.IssueID.ToString());
        }

        // Delete Issue
        _issueRepository.Delete(issue);

        // Save Changes
        await _issueRepository.SaveChangesAsync();
        _logger.LogInformation($"Issue with ID: {request.IssueID} deleted");

        return request.IssueID;
    }
}