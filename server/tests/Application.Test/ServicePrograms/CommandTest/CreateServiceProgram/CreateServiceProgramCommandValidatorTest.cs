using Application.Features.ServicePrograms.Command.CreateServiceProgram;

namespace Application.Test.ServicePrograms.CommandTest.CreateServiceProgram;

public class CreateServiceProgramCommandValidatorTest : ServiceProgramCommandValidatorTestBase<CreateServiceProgramCommand, CreateServiceProgramCommandValidator>
{
    protected override CreateServiceProgramCommandValidator Validator { get; } = new();

    protected override CreateServiceProgramCommand CreateValidCommand(
        string name = "Service Program Name",
        string? description = "Service Program Description",
        bool isActive = true)
        => new(name, description, isActive);
}