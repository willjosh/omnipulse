using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Command.GenerateServiceReminders;
using Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;

using Domain.Entities;

using MediatR;

using Moq;

namespace Application.Test.ServiceSchedules.CommandTest.DeleteServiceSchedule;

public class DeleteServiceScheduleCommandHandlerTest
{
    private readonly DeleteServiceScheduleCommandHandler _handler;
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository;
    private readonly Mock<IXrefServiceScheduleServiceTaskRepository> _mockXrefRepo;
    private readonly Mock<IServiceReminderRepository> _mockReminderRepo;
    private readonly Mock<ISender> _mockSender;
    private readonly Mock<IAppLogger<DeleteServiceScheduleCommandHandler>> _mockLogger;

    public DeleteServiceScheduleCommandHandlerTest()
    {
        _mockServiceScheduleRepository = new();
        _mockXrefRepo = new();
        _mockReminderRepo = new();
        _mockSender = new();
        _mockLogger = new();
        _handler = new(
            _mockServiceScheduleRepository.Object,
            _mockXrefRepo.Object,
            _mockReminderRepo.Object,
            _mockSender.Object,
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
        _mockServiceScheduleRepository.Setup(repo => repo.Update(It.IsAny<ServiceSchedule>()));
        _mockServiceScheduleRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockXrefRepo.Setup(x => x.RemoveAllForScheduleAsync(command.ServiceScheduleID)).Returns(Task.CompletedTask);
        _mockReminderRepo.Setup(r => r.CancelFutureRemindersForScheduleAsync(command.ServiceScheduleID, "Schedule deleted")).Returns(Task.CompletedTask);
        _mockSender.Setup(s => s.Send(It.IsAny<IRequest<GenerateServiceRemindersResponse>>(), It.IsAny<CancellationToken>())).ReturnsAsync(new GenerateServiceRemindersResponse(0, 0, true));

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.ServiceScheduleID, result);
        Assert.True(returnedSchedule.IsSoftDeleted);
        // Verify repository interactions
        _mockServiceScheduleRepository.Verify(r => r.GetByIdAsync(command.ServiceScheduleID), Times.Once);
        _mockServiceScheduleRepository.Verify(r => r.Update(It.Is<ServiceSchedule>(s => s.ID == command.ServiceScheduleID && s.IsSoftDeleted)), Times.Once);
        _mockReminderRepo.Verify(r => r.CancelFutureRemindersForScheduleAsync(command.ServiceScheduleID, "Schedule deleted"), Times.Once);
        _mockSender.Verify(s => s.Send(It.IsAny<GenerateServiceRemindersCommand>(), It.IsAny<CancellationToken>()), Times.Once);
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
        _mockServiceScheduleRepository.Verify(repo => repo.Update(It.IsAny<ServiceSchedule>()), Times.Never);
        _mockServiceScheduleRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockReminderRepo.Verify(r => r.CancelFutureRemindersForScheduleAsync(It.IsAny<int>(), It.IsAny<string>()), Times.Never);
        _mockSender.Verify(s => s.Send(It.IsAny<IRequest<GenerateServiceRemindersResponse>>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}