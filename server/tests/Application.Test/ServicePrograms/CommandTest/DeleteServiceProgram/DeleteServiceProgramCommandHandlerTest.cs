using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Command.DeleteServiceProgram;

using Domain.Entities;

using Moq;

using Xunit;

namespace Application.Test.ServicePrograms.CommandTest.DeleteServiceProgram;

public class DeleteServiceProgramCommandHandlerTest
{
    private readonly DeleteServiceProgramCommandHandler _commandHandler;
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository = new();

    public DeleteServiceProgramCommandHandlerTest()
    {
        _commandHandler = new(
            _mockServiceProgramRepository.Object,
            _mockServiceScheduleRepository.Object);
    }

    [Fact]
    public async Task Handler_Should_Return_ServiceProgramID_On_Success()
    {
        // TODO
        // Arrange

        // Act

        // Assert
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_InvalidServiceProgramID()
    {
        // TODO
        // Arrange

        // Act & Assert
    }
}