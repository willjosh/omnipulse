using FluentValidation;

namespace Application.Features.ServicePrograms.Command.RemoveVehicleFromServiceProgram;

public sealed class RemoveVehicleFromServiceProgramCommandValidator : AbstractValidator<RemoveVehicleFromServiceProgramCommand>
{
    public RemoveVehicleFromServiceProgramCommandValidator()
    {
        RuleFor(x => x.ServiceProgramID)
            .GreaterThan(0)
            .WithMessage($"{nameof(RemoveVehicleFromServiceProgramCommand.ServiceProgramID)} must be a positive integer");

        RuleFor(x => x.VehicleID)
            .GreaterThan(0)
            .WithMessage($"{nameof(RemoveVehicleFromServiceProgramCommand.VehicleID)} must be a positive integer");
    }
}