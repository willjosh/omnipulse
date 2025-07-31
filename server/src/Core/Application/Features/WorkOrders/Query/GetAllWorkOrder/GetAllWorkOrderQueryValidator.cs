using System;

using Application.Features.Shared;

using Domain.Entities;

using FluentValidation;

namespace Application.Features.WorkOrders.Query.GetAllWorkOrder;

public class GetAllWorkOrderQueryValidator : AbstractValidator<GetAllWorkOrderQuery>
{
    public GetAllWorkOrderQueryValidator()
    {
        var workOrderSortFields = new[] {
            "id",
            "status",
            "workordertype",
            "priority",
            "startodometer",
            "scheduledstartdate",
            "actualstartdate",
        };

        RuleFor(x => x.Parameters)
            .NotNull()
            .SetValidator(new PaginationValidator<WorkOrder>(workOrderSortFields));
    }
}