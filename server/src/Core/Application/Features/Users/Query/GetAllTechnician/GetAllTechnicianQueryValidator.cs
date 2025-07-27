using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.Users.Query.GetAllTechnician;

public class GetAllTechnicianQueryValidator : AbstractValidator<GetAllTechnicianQuery>
{
    public GetAllTechnicianQueryValidator()
    {
        var technicianSortFields = new[] {
        "firstname",
        "lastname",
        "hiredate",
        "isactive",
        "email"
       };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<User>(technicianSortFields));
    }
}