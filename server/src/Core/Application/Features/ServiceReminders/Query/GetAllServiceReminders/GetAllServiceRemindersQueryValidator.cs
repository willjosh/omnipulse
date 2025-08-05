using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.ServiceReminders.Query.GetAllServiceReminders;

public class GetAllServiceRemindersQueryValidator : AbstractValidator<GetAllServiceRemindersQuery>
{
    public GetAllServiceRemindersQueryValidator()
    {
        var serviceReminderSortFields = new[] {
            "vehiclename",
            "servicetaskname",
            "duedate",
            "duemileage",
            "status",
            "prioritylevel",
            "occurrencenumber"
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .WithMessage($"{nameof(GetAllServiceRemindersQuery.Parameters)} cannot be null.")
            .SetValidator(new PaginationValidator<ServiceReminder>(serviceReminderSortFields));
    }
}