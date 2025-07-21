using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;

using Domain.Entities;

using Moq;

using Xunit;

namespace Application.Test.ServiceSchedules.CommandTest.DeleteServiceSchedule;

public class DeleteServiceScheduleCommandHandlerTest
{
    private readonly DeleteServiceScheduleCommandHandler _handler;
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository;
    private readonly Mock<IXrefServiceScheduleServiceTaskRepository> _mockXrefRepo;
    private readonly Mock<IAppLogger<DeleteServiceScheduleCommandHandler>> _mockLogger;

    public DeleteServiceScheduleCommandHandlerTest()
    {
        _mockServiceScheduleRepository = new();
        _mockXrefRepo = new();
        _mockLogger = new();
        _handler = new(
            _mockServiceScheduleRepository.Object,
            _mockXrefRepo.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_ServiceScheduleID_On_Success()
    {
        // Arrange
        var command = new DeleteServiceScheduleCommand(ServiceScheduleID: 42);
        var returnedSchedule = new ServiceSchedule
        {
            ID = command.ServiceScheduleID,
            ServiceProgramID = 1,
            Name = "Test Schedule",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            XrefServiceScheduleServiceTasks = [],
            ServiceProgram = new ServiceProgram
            {
                ID = 1,
                Name = "Test Program",
                IsActive = true,
                XrefServiceProgramVehicles = [],
                ServiceSchedules = [],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        _mockServiceScheduleRepository.Setup(repo => repo.GetByIdAsync(command.ServiceScheduleID)).ReturnsAsync(returnedSchedule);
        _mockServiceScheduleRepository.Setup(repo => repo.Delete(returnedSchedule));
        _mockServiceScheduleRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockXrefRepo.Setup(x => x.RemoveAllForScheduleAsync(command.ServiceScheduleID)).Returns(Task.CompletedTask);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.ServiceScheduleID, result);
        _mockServiceScheduleRepository.Verify(repo => repo.GetByIdAsync(command.ServiceScheduleID), Times.Once);
        _mockXrefRepo.Verify(x => x.RemoveAllForScheduleAsync(command.ServiceScheduleID), Times.Once);
        _mockServiceScheduleRepository.Verify(repo => repo.Delete(returnedSchedule), Times.Once);
        _mockServiceScheduleRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidServiceScheduleID()
    {
        // Arrange
        var command = new DeleteServiceScheduleCommand(ServiceScheduleID: 99);
        _mockServiceScheduleRepository.Setup(repo => repo.GetByIdAsync(command.ServiceScheduleID)).ReturnsAsync((ServiceSchedule?)null);

        // Act
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        // Assert
        _mockServiceScheduleRepository.Verify(repo => repo.GetByIdAsync(command.ServiceScheduleID), Times.Once);
        _mockXrefRepo.Verify(x => x.RemoveAllForScheduleAsync(It.IsAny<int>()), Times.Never);
        _mockServiceScheduleRepository.Verify(repo => repo.Delete(It.IsAny<ServiceSchedule>()), Times.Never);
        _mockServiceScheduleRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}