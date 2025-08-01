using FluentValidation;

namespace Application.Features.InspectionFormItems.Command.DeactivateInspectionFormItem;

public sealed class DeactivateInspectionFormItemCommandValidator : AbstractValidator<DeactivateInspectionFormItemCommand>
{
    public DeactivateInspectionFormItemCommandValidator()
    {
        RuleFor(x => x.InspectionFormItemID)
            .GreaterThan(0)
            .WithMessage($"{nameof(DeactivateInspectionFormItemCommand.InspectionFormItemID)} must be a positive integer");
    }
}