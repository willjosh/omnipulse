using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using Moq;

using Xunit;

namespace Application.Test.ServiceSchedules.CommandTest.DeleteServiceSchedule;

public class DeleteServiceScheduleCommandHandlerTest
{
    private readonly DeleteServiceScheduleCommandHandler _handler;
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository;
    private readonly Mock<IAppLogger<DeleteServiceScheduleCommandHandler>> _mockLogger;

    public DeleteServiceScheduleCommandHandlerTest()
    {
        _mockServiceScheduleRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceScheduleMappingProfile>());
        var mapper = config.CreateMapper();

        _handler = new(_mockServiceScheduleRepository.Object, _mockLogger.Object, mapper);
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
            ServiceScheduleTasks = [],
            ServiceProgram = new ServiceProgram
            {
                ID = 1,
                Name = "Test Program",
                OEMTag = "OEM-123",
                PrimaryMeterType = Domain.Entities.Enums.MeterTypeEnum.KILOMETER,
                SecondaryMeterType = Domain.Entities.Enums.MeterTypeEnum.HOURS,
                IsActive = true,
                ServiceSchedules = [],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
        _mockServiceScheduleRepository.Setup(repo => repo.GetByIdAsync(command.ServiceScheduleID)).ReturnsAsync(returnedSchedule);
        _mockServiceScheduleRepository.Setup(repo => repo.Delete(returnedSchedule));
        _mockServiceScheduleRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.ServiceScheduleID, result);
        _mockServiceScheduleRepository.Verify(repo => repo.GetByIdAsync(command.ServiceScheduleID), Times.Once);
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
        _mockServiceScheduleRepository.Verify(repo => repo.Delete(It.IsAny<ServiceSchedule>()), Times.Never);
        _mockServiceScheduleRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}