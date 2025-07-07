using System;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
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

    public Task<PagedResult<GetAllIssueDTO>> Handle(GetAllIssueQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}