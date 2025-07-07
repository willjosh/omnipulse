using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Issues.Query.GetAllIssue;

public class GetAllIssueQueryHandler : IRequestHandler<GetAllIssueQuery, PagedResult<GetAllIssueDTO>>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<GetAllIssueQueryHandler> _logger;
    private readonly IValidator<GetAllIssueQuery> _validator;

    public GetAllIssueQueryHandler(IIssueRepository issueRepository, IMapper mapper, IAppLogger<GetAllIssueQueryHandler> logger, IValidator<GetAllIssueQuery> validator)
    {
        _issueRepository = issueRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public async Task<PagedResult<GetAllIssueDTO>> Handle(GetAllIssueQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling GetAllIssueQuery");
        // validate the request
        var validationResult = _validator.Validate(request);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
            _logger.LogWarning($"GetAllIssue - Validation failed: {errorMessages}");
            throw new BadRequestException(errorMessages);
        }

        // get all issues from the repository (replace with paged method if available)
        var allIssues = await _issueRepository.GetAllAsync();
        // Simulate paging
        var skip = (request.Parameters.PageNumber - 1) * request.Parameters.PageSize;
        var pagedItems = allIssues.Skip(skip).Take(request.Parameters.PageSize).ToList();
        var issueDTOs = _mapper.Map<List<GetAllIssueDTO>>(pagedItems);

        var pagedResult = new PagedResult<GetAllIssueDTO>
        {
            Items = issueDTOs,
            TotalCount = allIssues.Count,
            PageNumber = request.Parameters.PageNumber,
            PageSize = request.Parameters.PageSize
        };

        _logger.LogInformation($"Returning {pagedResult.TotalCount} issues for page {pagedResult.PageNumber} with page size {pagedResult.PageSize}");
        return pagedResult;
    }
}