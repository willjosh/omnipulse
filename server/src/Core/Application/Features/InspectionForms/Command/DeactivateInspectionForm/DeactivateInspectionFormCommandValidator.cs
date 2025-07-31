using FluentValidation;

namespace Application.Features.InspectionForms.Command.DeactivateInspectionForm;

public sealed class DeactivateInspectionFormCommandValidator : AbstractValidator<DeactivateInspectionFormCommand>
{
    public DeactivateInspectionFormCommandValidator()
    {
        RuleFor(x => x.InspectionFormID)
            .GreaterThan(0)
            .WithMessage($"{nameof(DeactivateInspectionFormCommand.InspectionFormID)} must be a positive integer");
    }
}