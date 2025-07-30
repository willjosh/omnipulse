using FluentValidation;

namespace Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;

public sealed class AddVehicleToServiceProgramCommandValidator : AbstractValidator<AddVehicleToServiceProgramCommand>
{
    public AddVehicleToServiceProgramCommandValidator()
    {
        RuleFor(x => x.ServiceProgramID)
            .GreaterThan(0)
            .WithMessage($"{nameof(AddVehicleToServiceProgramCommand.ServiceProgramID)} must be a positive integer");

        RuleFor(x => x.VehicleID)
            .GreaterThan(0)
            .WithMessage($"{nameof(AddVehicleToServiceProgramCommand.VehicleID)} must be a positive integer");
    }
}