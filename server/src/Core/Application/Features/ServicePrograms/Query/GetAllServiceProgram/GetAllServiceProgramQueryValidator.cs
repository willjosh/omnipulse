using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.ServicePrograms.Query.GetAllServiceProgram;

public class GetAllServiceProgramQueryValidator : AbstractValidator<GetAllServiceProgramQuery>
{
    public GetAllServiceProgramQueryValidator()
    {
        var serviceProgramSortFields = new[] {
            "name",
            "description",
            "isactive",
            "createdat",
            "updatedat"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .WithMessage($"{nameof(GetAllServiceProgramQuery.Parameters)} cannot be null.")
            .SetValidator(new PaginationValidator<ServiceProgram>(serviceProgramSortFields));
    }
}