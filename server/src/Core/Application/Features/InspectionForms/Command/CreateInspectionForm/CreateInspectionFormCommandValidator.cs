using FluentValidation;

namespace Application.Features.InspectionForms.Command.CreateInspectionForm;

/// <summary>
/// Validates <see cref="CreateInspectionFormCommand"/> using FluentValidation rules.
/// </summary>
public class CreateInspectionFormCommandValidator : AbstractValidator<CreateInspectionFormCommand>
{
    public CreateInspectionFormCommandValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty()
            .WithMessage($"{nameof(CreateInspectionFormCommand.Title)} is required")
            .MaximumLength(200)
            .WithMessage($"{nameof(CreateInspectionFormCommand.Title)} must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000)
            .WithMessage($"{nameof(CreateInspectionFormCommand.Description)} must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.IsActive)
            .NotNull()
            .WithMessage($"{nameof(CreateInspectionFormCommand.IsActive)} flag is required");
    }
}