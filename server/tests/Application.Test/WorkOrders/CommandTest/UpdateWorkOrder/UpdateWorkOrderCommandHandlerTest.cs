using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrderLineItem.Models;
using Application.Features.WorkOrders.Command.UpdateWorkOrder;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.WorkOrders.CommandTest.UpdateWorkOrder;

public class UpdateWorkOrderCommandHandlerTest
{
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IServiceReminderRepository> _mockServiceReminderRepository;
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly Mock<IWorkOrderIssueRepository> _mockWorkOrderIssueRepository;
    private readonly Mock<IWorkOrderLineItemRepository> _mockWorkOrderLineItemRepository;
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IAppLogger<UpdateWorkOrderCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<UpdateWorkOrderCommand>> _mockValidator;
    private readonly UpdateWorkOrderCommandHandler _handler;

    public UpdateWorkOrderCommandHandlerTest()
    {
        _mockWorkOrderRepository = new();
        _mockUserRepository = new();
        _mockVehicleRepository = new();
        _mockServiceReminderRepository = new();
        _mockIssueRepository = new();
        _mockWorkOrderIssueRepository = new();
        _mockWorkOrderLineItemRepository = new();
        _mockInventoryItemRepository = new();
        _mockServiceTaskRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkOrderMappingProfile>();
            cfg.AddProfile<WorkOrderLineItemMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _handler = new UpdateWorkOrderCommandHandler(
            _mockWorkOrderRepository.Object,
            _mockUserRepository.Object,
            _mockVehicleRepository.Object,
            _mockServiceReminderRepository.Object,
            _mockIssueRepository.Object,
            _mockWorkOrderIssueRepository.Object,
            _mockWorkOrderLineItemRepository.Object,
            _mockInventoryItemRepository.Object,
            _mockServiceTaskRepository.Object,
            _mockLogger.Object,
            mapper,
            _mockValidator.Object
        );
    }

    private UpdateWorkOrderCommand CreateValidCommand(
        int workOrderId = 1,
        int vehicleId = 1,
        string assignedToUserId = "user-123",
        string title = "Updated Work Order",
        string? description = "Updated Description",
        WorkTypeEnum workOrderType = WorkTypeEnum.SCHEDULED,
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.HIGH,
        WorkOrderStatusEnum status = WorkOrderStatusEnum.ASSIGNED,
        DateTime? scheduledStartDate = null,
        DateTime? actualStartDate = null,
        DateTime? scheduledCompletionDate = null,
        DateTime? actualCompletionDate = null,
        double startOdometer = 1000.0,
        double? endOdometer = 1010.0,
        List<int>? issueIdList = null,
        List<CreateWorkOrderLineItemDTO>? workOrderLineItems = null
    )
    {
        return new UpdateWorkOrderCommand(
            workOrderId,
            vehicleId,
            assignedToUserId,
            title,
            description,
            workOrderType,
            priorityLevel,
            status,
            scheduledStartDate,
            actualStartDate,
            scheduledCompletionDate,
            actualCompletionDate,
            startOdometer,
            endOdometer,
            issueIdList,
            workOrderLineItems
        );
    }

    private WorkOrder CreateExistingWorkOrder(UpdateWorkOrderCommand command)
    {
        return new WorkOrder
        {
            ID = command.WorkOrderID,
            CreatedAt = DateTime.UtcNow.AddDays(-10),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            VehicleID = command.VehicleID,
            AssignedToUserID = command.AssignedToUserID,
            Title = "Old Title",
            WorkOrderType = WorkTypeEnum.SCHEDULED,
            PriorityLevel = PriorityLevelEnum.MEDIUM,
            Status = WorkOrderStatusEnum.ASSIGNED,
            StartOdometer = 900.0,
            EndOdometer = 950.0,
            Description = "Old Description",
            ScheduledStartDate = DateTime.UtcNow.AddDays(-5),
            ActualStartDate = DateTime.UtcNow.AddDays(-4),
            ScheduledCompletionDate = DateTime.UtcNow.AddDays(-2),
            ActualCompletionDate = DateTime.UtcNow.AddDays(-1),
            Vehicle = null!,
            MaintenanceHistories = [],
            User = null!,
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    private void SetupValidValidation(UpdateWorkOrderCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateWorkOrderCommand command, string propertyName, string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_WorkOrderId_On_Success()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var existingWorkOrder = CreateExistingWorkOrder(command);

        _mockWorkOrderRepository.Setup(r => r.GetByIdAsync(command.WorkOrderID)).ReturnsAsync(existingWorkOrder);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        _mockWorkOrderRepository.Setup(r => r.Update(existingWorkOrder));
        _mockWorkOrderRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.WorkOrderID, result);
        _mockWorkOrderRepository.Verify(r => r.GetByIdAsync(command.WorkOrderID), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.Update(existingWorkOrder), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_WorkOrder_Not_Found()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockWorkOrderRepository.Setup(r => r.GetByIdAsync(command.WorkOrderID)).ReturnsAsync((WorkOrder?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _mockWorkOrderRepository.Verify(r => r.GetByIdAsync(command.WorkOrderID), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.Update(It.IsAny<WorkOrder>()), Times.Never);
        _mockWorkOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "Title", "Title is required");

        // When & Then
        var ex = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        Assert.Contains("Title is required", ex.Message);

        _mockWorkOrderRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockWorkOrderRepository.Verify(r => r.Update(It.IsAny<WorkOrder>()), Times.Never);
        _mockWorkOrderRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Vehicle_Not_Found()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var existingWorkOrder = CreateExistingWorkOrder(command);

        _mockWorkOrderRepository.Setup(r => r.GetByIdAsync(command.WorkOrderID)).ReturnsAsync(existingWorkOrder);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _mockWorkOrderRepository.Verify(r => r.Update(It.IsAny<WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_User_Not_Found()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var existingWorkOrder = CreateExistingWorkOrder(command);

        _mockWorkOrderRepository.Setup(r => r.GetByIdAsync(command.WorkOrderID)).ReturnsAsync(existingWorkOrder);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        _mockWorkOrderRepository.Verify(r => r.Update(It.IsAny<WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Update_LineItems_And_Issues()
    {
        // Given
        var lineItems = new List<CreateWorkOrderLineItemDTO>
        {
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                Description = "Labor task",
                Quantity = 1,
                HourlyRate = 85.00m,
                LaborHours = 2.0,
                AssignedToUserID = "tech-001"
            }
        };
        var issueIds = new List<int> { 1, 2 };
        var command = CreateValidCommand(workOrderLineItems: lineItems, issueIdList: issueIds);
        SetupValidValidation(command);

        var existingWorkOrder = CreateExistingWorkOrder(command);

        _mockWorkOrderRepository.Setup(r => r.GetByIdAsync(command.WorkOrderID)).ReturnsAsync(existingWorkOrder);
        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);
        _mockIssueRepository.Setup(r => r.AllExistAsync(issueIds)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(i => i.AllExistAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(It.IsAny<IEnumerable<int>>())).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync("tech-001")).ReturnsAsync(true);

        _mockWorkOrderLineItemRepository.Setup(r => r.DeleteByWorkOrderIdAsync(command.WorkOrderID)).Returns(Task.CompletedTask);
        _mockWorkOrderLineItemRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()))
            .ReturnsAsync(new List<WorkOrderLineItem>());
        _mockWorkOrderIssueRepository.Setup(r => r.DeleteByWorkOrderIdAsync(command.WorkOrderID)).Returns(Task.CompletedTask);
        _mockWorkOrderIssueRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()))
            .ReturnsAsync(new List<WorkOrderIssue>());

        _mockWorkOrderRepository.Setup(r => r.Update(existingWorkOrder));
        _mockWorkOrderRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.WorkOrderID, result);
        _mockWorkOrderLineItemRepository.Verify(r => r.DeleteByWorkOrderIdAsync(command.WorkOrderID), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Once);
        _mockWorkOrderIssueRepository.Verify(r => r.DeleteByWorkOrderIdAsync(command.WorkOrderID), Times.Once);
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Once);
    }
}