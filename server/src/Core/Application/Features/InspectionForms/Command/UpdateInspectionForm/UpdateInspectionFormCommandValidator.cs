using FluentValidation;

namespace Application.Features.InspectionForms.Command.UpdateInspectionForm;

public sealed class UpdateInspectionFormCommandValidator : AbstractValidator<UpdateInspectionFormCommand>
{
    // Constants
    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 1000;

    public UpdateInspectionFormCommandValidator()
    {
        RuleFor(x => x.InspectionFormID)
            .GreaterThan(0)
            .WithMessage($"{nameof(UpdateInspectionFormCommand.InspectionFormID)} must be a positive integer");

        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage($"{nameof(UpdateInspectionFormCommand.Title)} is required")
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage($"{nameof(UpdateInspectionFormCommand.Title)} cannot be whitespace only")
            .MinimumLength(1)
            .MaximumLength(TitleMaxLength)
            .WithMessage($"{nameof(UpdateInspectionFormCommand.Title)} must not exceed {TitleMaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"{nameof(UpdateInspectionFormCommand.Description)} must not exceed {DescriptionMaxLength} characters");
    }
}