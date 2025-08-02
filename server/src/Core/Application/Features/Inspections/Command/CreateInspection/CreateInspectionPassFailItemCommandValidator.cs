using FluentValidation;

namespace Application.Features.Inspections.Command.CreateInspection;

/// <summary>
/// Validates <see cref="CreateInspectionPassFailItemCommand"/>
/// </summary>
public class CreateInspectionPassFailItemCommandValidator : AbstractValidator<CreateInspectionPassFailItemCommand>
{
    public CreateInspectionPassFailItemCommandValidator()
    {
        RuleFor(x => x.InspectionFormItemID)
            .GreaterThan(0)
            .WithMessage($"{nameof(CreateInspectionPassFailItemCommand.InspectionFormItemID)} must be greater than 0");

        RuleFor(x => x.Passed)
            .NotNull()
            .WithMessage($"{nameof(CreateInspectionPassFailItemCommand.Passed)} is required");

        RuleFor(x => x.Comment)
            .MaximumLength(1000)
            .WithMessage($"{nameof(CreateInspectionPassFailItemCommand.Comment)} must not exceed 1000 characters")
            .When(x => !string.IsNullOrEmpty(x.Comment));
    }
}