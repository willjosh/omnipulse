using System;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrders.Command.DeleteWorkOrder;

using Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

using Xunit;

namespace Application.Test.WorkOrders.CommandTest.DeleteWorkOrderTest;

public class DeleteWorkOrderCommandHandlerTest
{
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<ILogger<DeleteWorkOrderCommandHandler>> _mockLogger;
    private readonly DeleteWorkOrderCommandHandler _handler;

    public DeleteWorkOrderCommandHandlerTest()
    {
        _mockWorkOrderRepository = new();
        _mockLogger = new();
        _handler = new DeleteWorkOrderCommandHandler(_mockWorkOrderRepository.Object, _mockLogger.Object);
    }

    private WorkOrder CreateWorkOrderLikeCreateHandler(int id)
    {
        return new WorkOrder
        {
            ID = id,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleID = 1,
            AssignedToUserID = "user-123",
            Title = "Test WorkOrder",
            WorkOrderType = Domain.Entities.Enums.WorkTypeEnum.SCHEDULED,
            PriorityLevel = Domain.Entities.Enums.PriorityLevelEnum.HIGH,
            Status = Domain.Entities.Enums.WorkOrderStatusEnum.ASSIGNED,
            StartOdometer = 1000.0,
            EndOdometer = 1010.0,
            Description = "Test Description",
            ScheduledStartDate = DateTime.UtcNow.AddDays(1),
            ActualStartDate = DateTime.UtcNow.AddDays(2),
            ScheduledCompletionDate = DateTime.UtcNow.AddDays(3),
            ActualCompletionDate = DateTime.UtcNow.AddDays(4),
            Vehicle = null!,
            MaintenanceHistories = [],
            User = null!,
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    [Fact]
    public async Task Handle_Should_Return_WorkOrderID_On_Success()
    {
        // Given
        var command = new DeleteWorkOrderCommand(ID: 123);

        var returnedWorkOrder = CreateWorkOrderLikeCreateHandler(command.ID);

        _mockWorkOrderRepository.Setup(repo => repo.GetByIdAsync(command.ID)).ReturnsAsync(returnedWorkOrder);
        _mockWorkOrderRepository.Setup(repo => repo.Delete(returnedWorkOrder));
        _mockWorkOrderRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.ID, result);
        _mockWorkOrderRepository.Verify(repo => repo.GetByIdAsync(command.ID), Times.Once);
        _mockWorkOrderRepository.Verify(repo => repo.Delete(returnedWorkOrder), Times.Once);
        _mockWorkOrderRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidWorkOrderID()
    {
        // Given
        var command = new DeleteWorkOrderCommand(ID: 123);

        _mockWorkOrderRepository.Setup(repo => repo.GetByIdAsync(command.ID)).ReturnsAsync((WorkOrder?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        _mockWorkOrderRepository.Verify(repo => repo.GetByIdAsync(command.ID), Times.Once);
        _mockWorkOrderRepository.Verify(repo => repo.Delete(It.IsAny<WorkOrder>()), Times.Never);
        _mockWorkOrderRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}