using FluentValidation;

namespace Application.Features.ServicePrograms.Command.DeleteServiceProgram;

public sealed class DeleteServiceProgramCommandValidator : AbstractValidator<DeleteServiceProgramCommand>
{
    public DeleteServiceProgramCommandValidator()
    {
        RuleFor(x => x.ServiceProgramID)
            .GreaterThan(0)
            .WithMessage($"{nameof(DeleteServiceProgramCommand.ServiceProgramID)} must be a positive integer");
    }
}