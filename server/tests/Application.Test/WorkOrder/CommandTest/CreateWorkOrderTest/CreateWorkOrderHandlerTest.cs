using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrders.Command.CreateWorkOrder;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.WorkOrder.CommandTest.CreateWorkOrderTest;

public class CreateWorkOrderHandlerTest
{

    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IServiceReminderRepository> _mockServiceReminderRepository;
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly Mock<IWorkOrderIssueRepository> _mockWorkOrderIssueRepository;
    private readonly Mock<IAppLogger<CreateWorkOrderCommandHandler>> _mockLogger;
    private readonly CreateWorkOrderCommandHandler _handler;
    private readonly Mock<IValidator<CreateWorkOrderCommand>> _mockValidator;

    public CreateWorkOrderHandlerTest()
    {
        _mockWorkOrderRepository = new();
        _mockUserRepository = new();
        _mockVehicleRepository = new();
        _mockServiceReminderRepository = new();
        _mockIssueRepository = new();
        _mockWorkOrderIssueRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkOrderMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _handler = new CreateWorkOrderCommandHandler(_mockWorkOrderRepository.Object, _mockUserRepository.Object, _mockVehicleRepository.Object, _mockServiceReminderRepository.Object, _mockIssueRepository.Object, _mockLogger.Object, mapper);
    }

    private CreateWorkOrderCommand CreateValidCommand(
        string workOrderNumber = "WO12345",
        int vehicleId = 1,
        int serviceReminderId = 1,
        string assignedToUserId = "1a0a07ba-19b9-4e88-bcfd-2ae76e81fca5",
        string title = "Test Work Order",
        string? description = null,
        WorkTypeEnum workOrderType = WorkTypeEnum.SCHEDULED,
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.HIGH,
        WorkOrderStatusEnum status = WorkOrderStatusEnum.ASSIGNED,
        decimal? estimatedCost = 100.00m,
        decimal? actualCost = null,
        double? estimatedHours = 2.0,
        double? actualHours = null,
        DateTime? scheduledStartDate = null,
        DateTime? actualStartDate = null,
        double startOdometer = 1000.0,
        double? endOdometer = null,
        List<int>? issueIdList = null
    )
    {
        return new CreateWorkOrderCommand(
            workOrderNumber,
            vehicleId,
            serviceReminderId,
            assignedToUserId,
            title,
            description,
            workOrderType,
            priorityLevel,
            status,
            estimatedCost,
            actualCost,
            estimatedHours,
            actualHours,
            scheduledStartDate,
            actualStartDate,
            startOdometer,
            endOdometer,
            issueIdList
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(CreateWorkOrderCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(CreateWorkOrderCommand command, string propertyName = "Name", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(invalidResult);
    }

    private Domain.Entities.WorkOrder CreateWorkOrderFromCommand(CreateWorkOrderCommand command)
    {
        return new Domain.Entities.WorkOrder
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            // WorkOrder required properties
            WorkOrderNumber = command.WorkOrderNumber,
            VehicleID = command.VehicleId,
            ServiceReminderID = command.ServiceReminderId,
            AssignedToUserID = command.AssignedToUserId,
            Title = command.Title,
            WorkOrderType = command.WorkOrderType,
            PriorityLevel = command.PriorityLevel,
            Status = command.Status,
            StartOdometer = command.StartOdometer,

            // Optional properties
            Description = command.Description,
            EstimatedCost = command.EstimatedCost,
            ActualCost = command.ActualCost,
            EstimatedHours = command.EstimatedHours,
            ActualHours = command.ActualHours,
            ScheduledStartDate = command.ScheduledStartDate,
            ActualStartDate = command.ActualStartDate,
            EndOdometer = command.EndOdometer,

            // Required navigation properties - initialize as empty collections or null
            Vehicle = null!,
            MaintenanceHistories = [],
            ServiceReminder = null!,
            User = null!,
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    private List<WorkOrderIssue> CreateWorkOrderIssuesFromCommand(CreateWorkOrderCommand command, Domain.Entities.WorkOrder workOrder)
    {
        if (command.IssueIdList == null || !command.IssueIdList.Any())
        {
            return [];
        }

        return command.IssueIdList.Select(issueId => new WorkOrderIssue
        {
            WorkOrderID = workOrder.ID,
            IssueID = issueId,
            AssignedDate = DateTime.UtcNow,
            WorkOrder = null!,
            Issue = null!
        }).ToList();
    }

    [Fact]
    public async Task Handler_Should_Return_WorkOrderId_When_Command_Is_Valid()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);

        // check entities exist
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserId)).ReturnsAsync(true);
        _mockServiceReminderRepository.Setup(s => s.ExistsAsync(command.ServiceReminderId)).ReturnsAsync(true);

        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify all mocks were called correctly
        _mockVehicleRepository.Verify(v => v.ExistsAsync(command.VehicleId), Times.Once);
        _mockUserRepository.Verify(u => u.ExistsAsync(command.AssignedToUserId), Times.Once);
        _mockServiceReminderRepository.Verify(s => s.ExistsAsync(command.ServiceReminderId), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Once);

        // Verify that WorkOrderIssue repository was NOT called since no issues provided
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Never);
        _mockIssueRepository.Verify(i => i.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Return_WorkOrderId_When_Command_Is_Valid_With_List()
    {
        // Given
        var issueIdList = new List<int> { 1, 2, 3 };
        var command = CreateValidCommand(issueIdList: issueIdList);
        SetupValidValidation(command);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);
        var expectedWorkOrderIssues = CreateWorkOrderIssuesFromCommand(command, expectedWorkOrder);

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserId)).ReturnsAsync(true);
        _mockServiceReminderRepository.Setup(s => s.ExistsAsync(command.ServiceReminderId)).ReturnsAsync(true);
        _mockIssueRepository.Setup(i => i.AllExistAsync(issueIdList)).ReturnsAsync(true);

        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);
        _mockWorkOrderIssueRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()))
            .ReturnsAsync(expectedWorkOrderIssues);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify all mocks were called correctly
        _mockVehicleRepository.Verify(v => v.ExistsAsync(command.VehicleId), Times.Once);
        _mockUserRepository.Verify(u => u.ExistsAsync(command.AssignedToUserId), Times.Once);
        _mockServiceReminderRepository.Verify(s => s.ExistsAsync(command.ServiceReminderId), Times.Once);
        _mockIssueRepository.Verify(i => i.AllExistAsync(issueIdList), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Once);
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Once);
    }
 
    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Vehicle_Does_Not_Exist()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleId)).ReturnsAsync(false);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserId)).ReturnsAsync(true);
        _mockServiceReminderRepository.Setup(s => s.ExistsAsync(command.ServiceReminderId)).ReturnsAsync(true);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Vehicle", exception.Message);
        Assert.Contains(command.VehicleId.ToString(), exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Some_Issues_Do_Not_Exist()
    {
        // Given
        var issueIdList = new List<int> { 1, 2, 999 }; // 999 doesn't exist
        var command = CreateValidCommand(issueIdList: issueIdList);
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserId)).ReturnsAsync(true);
        _mockServiceReminderRepository.Setup(s => s.ExistsAsync(command.ServiceReminderId)).ReturnsAsync(true);
        _mockIssueRepository.Setup(i => i.AllExistAsync(issueIdList)).ReturnsAsync(false);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Issue", exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Assigned_User_Does_Not_Exist()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserId)).ReturnsAsync(false);
        _mockServiceReminderRepository.Setup(s => s.ExistsAsync(command.ServiceReminderId)).ReturnsAsync(true);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("User", exception.Message);
        Assert.Contains(command.AssignedToUserId, exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Service_Reminder_Does_Not_Exist()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserId)).ReturnsAsync(true);
        _mockServiceReminderRepository.Setup(s => s.ExistsAsync(command.ServiceReminderId)).ReturnsAsync(false);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("ServiceReminder", exception.Message);
        Assert.Contains(command.ServiceReminderId.ToString(), exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Command_Is_Invalid()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "WorkOrderNumber", "Work Order Number is required");

        // Setup all entities as existing (but validation will fail before we check them)
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleId)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserId)).ReturnsAsync(true);
        _mockServiceReminderRepository.Setup(s => s.ExistsAsync(command.ServiceReminderId)).ReturnsAsync(true);

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        
        Assert.Contains("Work Order Number is required", exception.Message);

        // Verify that no repository operations were performed
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Never);
        
        // Verify that existence checks were never called due to early validation failure
        _mockVehicleRepository.Verify(v => v.ExistsAsync(It.IsAny<int>()), Times.Never);
        _mockUserRepository.Verify(u => u.ExistsAsync(It.IsAny<string>()), Times.Never);
        _mockServiceReminderRepository.Verify(s => s.ExistsAsync(It.IsAny<int>()), Times.Never);
    }
}
