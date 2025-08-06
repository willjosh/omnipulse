using FluentValidation;

namespace Application.Features.ServiceReminders.Command.AddServiceReminderToExistingWorkOrder;

public class AddServiceReminderToExistingWorkOrderCommandValidator : AbstractValidator<AddServiceReminderToExistingWorkOrderCommand>
{
    public AddServiceReminderToExistingWorkOrderCommandValidator()
    {
        RuleFor(x => x.ServiceReminderID)
            .GreaterThan(0)
            .WithMessage($"{nameof(AddServiceReminderToExistingWorkOrderCommand.ServiceReminderID)} must be greater than 0.");

        RuleFor(x => x.WorkOrderID)
            .GreaterThan(0)
            .WithMessage($"{nameof(AddServiceReminderToExistingWorkOrderCommand.WorkOrderID)} must be greater than 0.");
    }
}