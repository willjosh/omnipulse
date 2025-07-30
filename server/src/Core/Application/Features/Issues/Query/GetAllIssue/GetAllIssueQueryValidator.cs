using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.Issues.Query.GetAllIssue;

public class GetAllIssueQueryValidator : AbstractValidator<GetAllIssueQuery>
{
    public GetAllIssueQueryValidator()
    {
        var issueSortFields = new[] {
            "title",
            "status",
            "prioritylevel",
            "category",
            "reporteddate",
            "resolveddate",
            "createdat",
            "updatedat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<Issue>(issueSortFields));
    }
}