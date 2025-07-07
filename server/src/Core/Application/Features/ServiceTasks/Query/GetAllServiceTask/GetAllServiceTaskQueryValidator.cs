using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.ServiceTasks.Query.GetAllServiceTask;

public class GetAllServiceTaskQueryValidator : AbstractValidator<GetAllServiceTaskQuery>
{
    public GetAllServiceTaskQueryValidator()
    {
        var serviceTaskSortFields = new[] {
            "name",
            "category",
            "estimatedlabourhours",
            "estimatedcost",
            "isactive",
            "createdat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<ServiceTask>(serviceTaskSortFields));
    }
}