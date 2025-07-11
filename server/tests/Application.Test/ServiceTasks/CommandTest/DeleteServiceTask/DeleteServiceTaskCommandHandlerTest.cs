using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceTasks.Command.DeleteServiceTask;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

using Xunit;

namespace Application.Test.ServiceTasks.CommandTest.DeleteServiceTask;

public class DeleteServiceTaskCommandHandlerTest
{
    private readonly DeleteServiceTaskCommandHandler _deleteServiceTaskCommandHandler;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IAppLogger<DeleteServiceTaskCommandHandler>> _mockLogger;

    public DeleteServiceTaskCommandHandlerTest()
    {
        _mockServiceTaskRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceTaskMappingProfile>());
        var mapper = config.CreateMapper();

        _deleteServiceTaskCommandHandler = new(_mockServiceTaskRepository.Object, _mockLogger.Object, mapper);
    }

    [Fact]
    public async Task Handle_Should_Return_ServiceTaskID_On_Success()
    {
        // Given
        var command = new DeleteServiceTaskCommand(ServiceTaskID: 123);

        var returnedServiceTask = new ServiceTask
        {
            ID = command.ServiceTaskID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Test Service Task",
            Description = "Test Description",
            EstimatedLabourHours = 2.5,
            EstimatedCost = 100.00m,
            Category = ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive = true,
            XrefServiceScheduleServiceTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = []
        };

        _mockServiceTaskRepository.Setup(repo => repo.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync(returnedServiceTask);
        _mockServiceTaskRepository.Setup(repo => repo.Delete(returnedServiceTask));
        _mockServiceTaskRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _deleteServiceTaskCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.ServiceTaskID, result);
        _mockServiceTaskRepository.Verify(repo => repo.GetByIdAsync(command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(repo => repo.Delete(returnedServiceTask), Times.Once);
        _mockServiceTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidServiceTaskID()
    {
        // Given
        var command = new DeleteServiceTaskCommand(ServiceTaskID: 123);

        _mockServiceTaskRepository.Setup(repo => repo.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync((ServiceTask?)null);

        // When
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _deleteServiceTaskCommandHandler.Handle(command, CancellationToken.None));

        // Then
        _mockServiceTaskRepository.Verify(repo => repo.GetByIdAsync(command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(repo => repo.Delete(It.IsAny<ServiceTask>()), Times.Never);
        _mockServiceTaskRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}