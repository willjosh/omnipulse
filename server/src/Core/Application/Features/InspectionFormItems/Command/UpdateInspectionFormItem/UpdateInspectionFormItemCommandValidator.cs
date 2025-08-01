using FluentValidation;

namespace Application.Features.InspectionFormItems.Command.UpdateInspectionFormItem;

/// <summary>
/// Validates <see cref="UpdateInspectionFormItemCommand"/>.
/// </summary>
public sealed class UpdateInspectionFormItemCommandValidator : AbstractValidator<UpdateInspectionFormItemCommand>
{
    // Constants
    private const int ItemLabelMaxLength = 200;
    private const int ItemDescriptionMaxLength = 500;
    private const int ItemInstructionsMaxLength = 4000;

    public UpdateInspectionFormItemCommandValidator()
    {
        RuleFor(x => x.InspectionFormItemID)
            .GreaterThan(0)
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.InspectionFormItemID)} must be a positive integer");

        RuleFor(x => x.InspectionFormID)
            .GreaterThan(0)
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.InspectionFormID)} must be a positive integer");

        RuleFor(x => x.ItemLabel)
            .NotEmpty()
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.ItemLabel)} is required")
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.ItemLabel)} cannot be whitespace only")
            .MinimumLength(1)
            .MaximumLength(ItemLabelMaxLength)
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.ItemLabel)} must not exceed {ItemLabelMaxLength} characters");

        RuleFor(x => x.ItemDescription)
            .MaximumLength(ItemDescriptionMaxLength)
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.ItemDescription)} must not exceed {ItemDescriptionMaxLength} characters")
            .When(x => !string.IsNullOrEmpty(x.ItemDescription));

        RuleFor(x => x.ItemInstructions)
            .MaximumLength(ItemInstructionsMaxLength)
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.ItemInstructions)} must not exceed {ItemInstructionsMaxLength} characters")
            .When(x => !string.IsNullOrEmpty(x.ItemInstructions));


        RuleFor(x => x.IsRequired)
            .NotNull()
            .WithMessage($"{nameof(UpdateInspectionFormItemCommand.IsRequired)} flag is required");
    }
}