using FluentValidation;

namespace Application.Features.InspectionFormItems.Command.CreateInspectionFormItem;

/// <summary>
/// Validates <see cref="CreateInspectionFormItemCommand"/> using FluentValidation rules.
/// </summary>
public sealed class CreateInspectionFormItemCommandValidator : AbstractValidator<CreateInspectionFormItemCommand>
{
    // Constants
    private const int ItemLabelMaxLength = 200;
    private const int ItemDescriptionMaxLength = 500;
    private const int ItemInstructionsMaxLength = 4000;

    public CreateInspectionFormItemCommandValidator()
    {
        RuleFor(x => x.InspectionFormID)
            .GreaterThan(0)
            .WithMessage($"{nameof(CreateInspectionFormItemCommand.InspectionFormID)} must be a positive integer");

        RuleFor(x => x.ItemLabel)
            .NotEmpty()
            .WithMessage($"{nameof(CreateInspectionFormItemCommand.ItemLabel)} is required")
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage($"{nameof(CreateInspectionFormItemCommand.ItemLabel)} cannot be whitespace only")
            .MinimumLength(1)
            .MaximumLength(ItemLabelMaxLength)
            .WithMessage($"{nameof(CreateInspectionFormItemCommand.ItemLabel)} must not exceed {ItemLabelMaxLength} characters");

        RuleFor(x => x.ItemDescription)
            .MaximumLength(ItemDescriptionMaxLength)
            .WithMessage($"{nameof(CreateInspectionFormItemCommand.ItemDescription)} must not exceed {ItemDescriptionMaxLength} characters")
            .When(x => !string.IsNullOrEmpty(x.ItemDescription));

        RuleFor(x => x.ItemInstructions)
            .MaximumLength(ItemInstructionsMaxLength)
            .WithMessage($"{nameof(CreateInspectionFormItemCommand.ItemInstructions)} must not exceed {ItemInstructionsMaxLength} characters")
            .When(x => !string.IsNullOrEmpty(x.ItemInstructions));

        RuleFor(x => x.InspectionFormItemTypeEnum)
            .IsInEnum()
            .WithMessage($"Invalid {nameof(CreateInspectionFormItemCommand.InspectionFormItemTypeEnum)} selected");

        RuleFor(x => x.IsRequired)
            .NotNull()
            .WithMessage($"{nameof(CreateInspectionFormItemCommand.IsRequired)} flag is required");
    }
}