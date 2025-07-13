using Application.Features.Shared;
using Domain.Entities;
using FluentValidation;

namespace Application.Features.ServiceSchedules.Query.GetAllServiceSchedule;

public class GetAllServiceScheduleQueryValidator : AbstractValidator<GetAllServiceScheduleQuery>
{
    public GetAllServiceScheduleQueryValidator()
    {
        var validServiceScheduleSortFields = new[] { "name", "isactive", "createdat" };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<ServiceSchedule>(validServiceScheduleSortFields));
    }
}