using System;
using System.Threading;
using System.Threading.Tasks;

using Application.Models.PaginationModels;

using AutoMapper;

using FluentValidation;

using MediatR;

namespace Application.Features.Issues.Query.GetAllIssue;

public class GetAllIssueQueryHandler : IRequestHandler<GetAllIssueQuery, PagedResult<GetAllIssueDTO>>
{
    public Task<PagedResult<GetAllIssueDTO>> Handle(GetAllIssueQuery request, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}