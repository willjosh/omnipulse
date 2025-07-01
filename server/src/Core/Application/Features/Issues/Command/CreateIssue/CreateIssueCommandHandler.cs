using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using MediatR;

namespace Application.Features.Issues.Command.CreateIssue;

public class CreateIssueCommandHandler : IRequestHandler<CreateIssueCommand, int>
{
    private readonly IIssueRepository _issueRepository;
    private readonly IMapper _mapper;
    private readonly IAppLogger<CreateIssueCommandHandler> _logger;
    private readonly IValidator<CreateIssueCommand> _validator;

    public CreateIssueCommandHandler(IIssueRepository issueRepository, IMapper mapper, IAppLogger<CreateIssueCommandHandler> logger, IValidator<CreateIssueCommand> validator)
    {
        _issueRepository = issueRepository;
        _mapper = mapper;
        _logger = logger;
        _validator = validator;
    }

    public Task<int> Handle(CreateIssueCommand request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
