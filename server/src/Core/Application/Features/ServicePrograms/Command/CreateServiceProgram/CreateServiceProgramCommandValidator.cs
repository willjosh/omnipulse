using FluentValidation;

namespace Application.Features.ServicePrograms.Command.CreateServiceProgram;

public sealed class CreateServiceProgramCommandValidator : AbstractValidator<CreateServiceProgramCommand>
{
    // Constants
    private const int NameMaxLength = 200;
    private const int DescriptionMaxLength = 500;

    public CreateServiceProgramCommandValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .WithMessage("Service program name is required")
            .Must(x => !string.IsNullOrWhiteSpace(x))
            .WithMessage("Service program name cannot be whitespace only")
            .MinimumLength(1)
            .MaximumLength(NameMaxLength)
            .WithMessage($"Service program name must not exceed {NameMaxLength} characters");

        RuleFor(x => x.Description)
            .MaximumLength(DescriptionMaxLength)
            .WithMessage($"Description must not exceed {DescriptionMaxLength} characters");
    }
}