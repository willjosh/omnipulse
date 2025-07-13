using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrderLineItem.Models;
using Application.Features.WorkOrders.Command.CreateWorkOrder;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.WorkOrders.CommandTest.CreateWorkOrderTest;

public class CreateWorkOrderHandlerTest
{
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IServiceReminderRepository> _mockServiceReminderRepository;
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly Mock<IWorkOrderIssueRepository> _mockWorkOrderIssueRepository;
    private readonly Mock<IWorkOrderLineItemRepository> _mockWorkOrderLineItemRepository;
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IAppLogger<CreateWorkOrderCommandHandler>> _mockLogger;
    private readonly CreateWorkOrderCommandHandler _handler;
    private readonly Mock<IValidator<CreateWorkOrderCommand>> _mockValidator;

    public CreateWorkOrderHandlerTest()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockServiceReminderRepository = new Mock<IServiceReminderRepository>();
        _mockWorkOrderRepository = new Mock<IWorkOrderRepository>();
        _mockIssueRepository = new Mock<IIssueRepository>();
        _mockWorkOrderIssueRepository = new Mock<IWorkOrderIssueRepository>();
        _mockWorkOrderLineItemRepository = new Mock<IWorkOrderLineItemRepository>();
        _mockInventoryItemRepository = new Mock<IInventoryItemRepository>();
        _mockServiceTaskRepository = new Mock<IServiceTaskRepository>();
        _mockLogger = new Mock<IAppLogger<CreateWorkOrderCommandHandler>>();
        _mockValidator = new Mock<IValidator<CreateWorkOrderCommand>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkOrderMappingProfile>();
            cfg.AddProfile<WorkOrderLineItemMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _handler = new CreateWorkOrderCommandHandler(
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
            _mockValidator.Object);
    }

    private CreateWorkOrderCommand CreateValidCommand(
        int vehicleId = 1,
        string assignedToUserId = "1a0a07ba-19b9-4e88-bcfd-2ae76e81fca5",
        string title = "Test Work Order",
        string? description = null,
        WorkTypeEnum workOrderType = WorkTypeEnum.SCHEDULED,
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.HIGH,
        WorkOrderStatusEnum status = WorkOrderStatusEnum.ASSIGNED,
        DateTime? scheduledStartDate = null,
        DateTime? actualStartDate = null,
        DateTime? scheduledCompletionDate = null,
        DateTime? actualCompletionDate = null,
        double startOdometer = 1000.0,
        double? endOdometer = null,
        List<int>? issueIdList = null,
        List<CreateWorkOrderLineItemDTO>? workOrderLineItems = null
    )
    {
        return new CreateWorkOrderCommand(
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

    private void SetupLineItemUserValidation(List<CreateWorkOrderLineItemDTO> lineItems)
    {
        var assignedUserIds = lineItems
            .Where(item => !string.IsNullOrEmpty(item.AssignedToUserID))
            .Select(item => item.AssignedToUserID!)
            .Distinct()
            .ToList();

        foreach (var userId in assignedUserIds)
        {
            _mockUserRepository.Setup(u => u.ExistsAsync(userId)).ReturnsAsync(true);
        }
    }

    private WorkOrder CreateWorkOrderFromCommand(CreateWorkOrderCommand command)
    {
        return new WorkOrder
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            // WorkOrder required properties
            VehicleID = command.VehicleID,
            AssignedToUserID = command.AssignedToUserID,
            Title = command.Title,
            WorkOrderType = command.WorkOrderType,
            PriorityLevel = command.PriorityLevel,
            Status = command.Status,
            StartOdometer = command.StartOdometer,

            // Optional properties
            Description = command.Description,
            ScheduledStartDate = command.ScheduledStartDate,
            ActualStartDate = command.ActualStartDate,
            ScheduledCompletionDate = command.ScheduledCompletionDate,
            ActualCompletionDate = command.ActualCompletionDate,
            EndOdometer = command.EndOdometer,

            // Required navigation properties - initialize as empty collections or null
            Vehicle = null!,
            MaintenanceHistories = [],
            User = null!,
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    private List<WorkOrderIssue> CreateWorkOrderIssuesFromCommand(CreateWorkOrderCommand command, WorkOrder workOrder)
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

    private List<WorkOrderLineItem> CreateWorkOrderLineItemsFromCommand(List<CreateWorkOrderLineItemDTO> lineItemDtos, int workOrderID)
    {
        return lineItemDtos.Select(dto => new WorkOrderLineItem
        {
            ID = 0, // Will be set by repository
            WorkOrderID = workOrderID,
            ServiceTaskID = dto.ServiceTaskID,
            ItemType = dto.ItemType,
            Quantity = dto.Quantity,
            InventoryItemID = dto.InventoryItemID,
            AssignedToUserID = dto.AssignedToUserID!,
            Description = dto.Description,
            LaborHours = dto.LaborHours,
            UnitPrice = dto.UnitPrice,
            HourlyRate = dto.HourlyRate,
            TotalCost = CalculateLineItemTotal(dto), // Calculate total cost
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,

            // Navigation properties
            User = null!,
            WorkOrder = null!,
            InventoryItem = null,
            ServiceTask = null!
        }).ToList();
    }

    private decimal CalculateLineItemTotal(CreateWorkOrderLineItemDTO dto)
    {
        return dto.ItemType switch
        {
            LineItemTypeEnum.ITEM => (dto.UnitPrice ?? 0) * dto.Quantity,
            LineItemTypeEnum.LABOR => (dto.HourlyRate ?? 0) * (decimal)(dto.LaborHours ?? 0),
            LineItemTypeEnum.BOTH => ((dto.UnitPrice ?? 0) * dto.Quantity) + ((dto.HourlyRate ?? 0) * (decimal)(dto.LaborHours ?? 0)),
            _ => 0
        };
    }

    private List<CreateWorkOrderLineItemDTO> CreateSampleLineItems()
    {
        return new List<CreateWorkOrderLineItemDTO>
        {
            // PARTS item
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.ITEM,
                InventoryItemID = 101,
                AssignedToUserID = "tech-001",
                Description = "Oil Filter",
                Quantity = 1,
                UnitPrice = 15.99m,
                HourlyRate = null,
                LaborHours = null
            },
            // LABOR item
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 2,
                ItemType = LineItemTypeEnum.LABOR,
                AssignedToUserID = "tech-002",
                Description = "Oil Change Labor",
                Quantity = 1,
                HourlyRate = 85.00m,
                LaborHours = 0.5,
                InventoryItemID = null,
                UnitPrice = null
            }
        };
    }

    [Fact]
    public async Task Handler_Should_Return_WorkOrderId_When_Command_Is_Valid()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);

        // check entities exist
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify all mocks were called correctly
        _mockVehicleRepository.Verify(v => v.ExistsAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(u => u.ExistsAsync(command.AssignedToUserID), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Once);

        // Verify that WorkOrderLineItem repository was NOT called since no line items provided
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Never);
        _mockInventoryItemRepository.Verify(i => i.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        _mockServiceTaskRepository.Verify(s => s.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
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

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);
        _mockIssueRepository.Setup(i => i.AllExistAsync(issueIdList)).ReturnsAsync(true);

        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);
        _mockWorkOrderIssueRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()))
            .ReturnsAsync(expectedWorkOrderIssues);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify all mocks were called correctly
        _mockVehicleRepository.Verify(v => v.ExistsAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(u => u.ExistsAsync(command.AssignedToUserID), Times.Once);
        _mockIssueRepository.Verify(i => i.AllExistAsync(issueIdList), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Once);
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_WorkOrderId_When_Command_Has_Valid_LineItems()
    {
        // Given
        var lineItems = CreateSampleLineItems();
        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);
        var expectedLineItems = CreateWorkOrderLineItemsFromCommand(lineItems, expectedWorkOrder.ID);

        // Setup entity existence validation
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // Setup line item entity validation
        var inventoryItemIds = lineItems.Where(li => li.InventoryItemID.HasValue).Select(li => li.InventoryItemID!.Value).ToList();
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();

        _mockInventoryItemRepository.Setup(i => i.AllExistAsync(inventoryItemIds)).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(true);

        SetupLineItemUserValidation(lineItems);

        // Setup repository returns
        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()))
            .ReturnsAsync(expectedLineItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify all main entity validations
        _mockVehicleRepository.Verify(v => v.ExistsAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(u => u.ExistsAsync(command.AssignedToUserID), Times.Once);

        // Verify line item entity validations
        _mockInventoryItemRepository.Verify(i => i.AllExistAsync(inventoryItemIds), Times.Once);
        _mockServiceTaskRepository.Verify(s => s.AllExistAsync(serviceTaskIds), Times.Once);

        // Verify repository operations
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.Is<IEnumerable<WorkOrderLineItem>>(
            items => items.Count() == lineItems.Count)), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_WorkOrderId_When_Command_Has_Issues_And_LineItems()
    {
        // Given
        var issueIdList = new List<int> { 1, 2, 3 };
        var lineItems = CreateSampleLineItems();
        var command = CreateValidCommand(issueIdList: issueIdList, workOrderLineItems: lineItems);
        SetupValidValidation(command);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);
        var expectedWorkOrderIssues = CreateWorkOrderIssuesFromCommand(command, expectedWorkOrder);
        var expectedLineItems = CreateWorkOrderLineItemsFromCommand(lineItems, expectedWorkOrder.ID);

        // Setup all entity validations
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);
        _mockIssueRepository.Setup(i => i.AllExistAsync(issueIdList)).ReturnsAsync(true);

        var inventoryItemIds = lineItems.Where(li => li.InventoryItemID.HasValue).Select(li => li.InventoryItemID!.Value).ToList();
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();
        _mockInventoryItemRepository.Setup(i => i.AllExistAsync(inventoryItemIds)).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(true);

        SetupLineItemUserValidation(lineItems);

        // Setup repository returns
        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);
        _mockWorkOrderIssueRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()))
            .ReturnsAsync(expectedWorkOrderIssues);
        _mockWorkOrderLineItemRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()))
            .ReturnsAsync(expectedLineItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify all validations and operations
        _mockVehicleRepository.Verify(v => v.ExistsAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(u => u.ExistsAsync(command.AssignedToUserID), Times.Once);
        _mockIssueRepository.Verify(i => i.AllExistAsync(issueIdList), Times.Once);
        _mockInventoryItemRepository.Verify(i => i.AllExistAsync(inventoryItemIds), Times.Once);
        _mockServiceTaskRepository.Verify(s => s.AllExistAsync(serviceTaskIds), Times.Once);

        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Once);
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Vehicle_Does_Not_Exist()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(false);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("Vehicle", exception.Message);
        Assert.Contains(command.VehicleID.ToString(), exception.Message);

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

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);
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

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(false);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("User", exception.Message);
        Assert.Contains(command.AssignedToUserID, exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_InventoryItem_Does_Not_Exist()
    {
        // Given
        var lineItems = CreateSampleLineItems();
        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        // Setup main entities as existing
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // Setup line item validation - inventory items don't exist
        var inventoryItemIds = lineItems.Where(li => li.InventoryItemID.HasValue).Select(li => li.InventoryItemID!.Value).ToList();
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();

        _mockInventoryItemRepository.Setup(i => i.AllExistAsync(inventoryItemIds)).ReturnsAsync(false); // Inventory items don't exist
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(true);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("InventoryItem", exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_ServiceTask_Does_Not_Exist()
    {
        // Given
        var lineItems = CreateSampleLineItems();
        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        // Setup main entities as existing
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // Setup line item validation - service tasks don't exist
        var inventoryItemIds = lineItems.Where(li => li.InventoryItemID.HasValue).Select(li => li.InventoryItemID!.Value).ToList();
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();

        _mockInventoryItemRepository.Setup(i => i.AllExistAsync(inventoryItemIds)).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(false); // Service tasks don't exist

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("ServiceTask", exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Command_Is_Invalid()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "WorkOrderNumber", "Work Order Number is required");

        // Setup all entities as existing (but validation will fail before we check them)
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("Work Order Number is required", exception.Message);

        // Verify that no repository operations were performed
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
        _mockWorkOrderIssueRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderIssue>>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Never);

        // Verify that existence checks were never called due to early validation failure
        _mockVehicleRepository.Verify(v => v.ExistsAsync(It.IsAny<int>()), Times.Never);
        _mockUserRepository.Verify(u => u.ExistsAsync(It.IsAny<string>()), Times.Never);
        _mockServiceReminderRepository.Verify(s => s.ExistsAsync(It.IsAny<int>()), Times.Never);
        _mockInventoryItemRepository.Verify(i => i.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        _mockServiceTaskRepository.Verify(s => s.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_LineItem_AssignedUser_Does_Not_Exist()
    {
        // Given
        var lineItems = new List<CreateWorkOrderLineItemDTO>
        {
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                AssignedToUserID = "non-existent-user", // This user doesn't exist
                Description = "Labor task",
                Quantity = 1,
                HourlyRate = 85.00m,
                LaborHours = 2.0,
                InventoryItemID = null,
                UnitPrice = null
            }
        };

        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        // Setup main entities as existing
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // Setup line item validation
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(true);

        // Line item user doesn't exist
        _mockUserRepository.Setup(u => u.ExistsAsync("non-existent-user")).ReturnsAsync(false);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        Assert.Contains("User", exception.Message);
        Assert.Contains("non-existent-user", exception.Message);

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Multiple_LineItem_Entities_Do_Not_Exist()
    {
        // Given
        var lineItems = new List<CreateWorkOrderLineItemDTO>
        {
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 999, // Doesn't exist
                ItemType = LineItemTypeEnum.ITEM,
                InventoryItemID = 888, // Doesn't exist
                Description = "Non-existent items",
                Quantity = 1,
                UnitPrice = 50.00m,
                AssignedToUserID = null,
                HourlyRate = null,
                LaborHours = null
            }
        };

        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        // Setup main entities as existing
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // Setup line item validation - both fail
        var inventoryItemIds = lineItems.Where(li => li.InventoryItemID.HasValue).Select(li => li.InventoryItemID!.Value).ToList();
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();

        _mockInventoryItemRepository.Setup(i => i.AllExistAsync(inventoryItemIds)).ReturnsAsync(false);
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(false);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        // Should fail on the first validation that runs (likely inventory items)
        Assert.True(exception.Message.Contains("InventoryItem") || exception.Message.Contains("ServiceTask"));

        // Verify that AddAsync was never called
        _mockWorkOrderRepository.Verify(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Skip_InventoryItem_Validation_When_No_LineItems_Have_InventoryItems()
    {
        // Given
        var lineItems = new List<CreateWorkOrderLineItemDTO>
        {
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                Description = "Labor only task",
                Quantity = 1,
                HourlyRate = 85.00m,
                LaborHours = 2.0,
                InventoryItemID = null, // No inventory item
                UnitPrice = null,
                AssignedToUserID = null
            }
        };

        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);
        var expectedLineItems = CreateWorkOrderLineItemsFromCommand(lineItems, expectedWorkOrder.ID);

        // Setup entity existence validation
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        // Setup line item validation
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(true);

        // Setup repository returns
        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()))
            .ReturnsAsync(expectedLineItems);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify that inventory item validation was NOT called since no line items have inventory items
        _mockInventoryItemRepository.Verify(i => i.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);

        // Verify other validations were called
        _mockServiceTaskRepository.Verify(s => s.AllExistAsync(serviceTaskIds), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Calculate_TotalCost_For_Different_LineItem_Types()
    {
        // Given
        var lineItems = new List<CreateWorkOrderLineItemDTO>
        {
            // LABOR type
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                Description = "Labor work",
                Quantity = 1,
                HourlyRate = 100.00m,
                LaborHours = 2.5,
                AssignedToUserID = "tech-001"
            },
            // ITEM type  
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 2,
                ItemType = LineItemTypeEnum.ITEM,
                Description = "Parts",
                Quantity = 3,
                UnitPrice = 25.00m,
                InventoryItemID = 101
            },
            // BOTH type
            new CreateWorkOrderLineItemDTO
            {
                ServiceTaskID = 3,
                ItemType = LineItemTypeEnum.BOTH,
                Description = "Labor + Parts",
                Quantity = 2,
                UnitPrice = 30.00m,
                HourlyRate = 80.00m,
                LaborHours = 1.5,
                InventoryItemID = 102,
                AssignedToUserID = "tech-002"
            }
        };

        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        // Setup all validations
        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        var inventoryItemIds = lineItems.Where(li => li.InventoryItemID.HasValue).Select(li => li.InventoryItemID!.Value).ToList();
        var serviceTaskIds = lineItems.Select(li => li.ServiceTaskID).ToList();
        _mockInventoryItemRepository.Setup(i => i.AllExistAsync(inventoryItemIds)).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(s => s.AllExistAsync(serviceTaskIds)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync("tech-001")).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync("tech-002")).ReturnsAsync(true);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);
        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);

        // Capture the line items to verify cost calculation
        List<WorkOrderLineItem> capturedLineItems = null!;
        _mockWorkOrderLineItemRepository.Setup(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()))
            .Callback<IEnumerable<WorkOrderLineItem>>(items => capturedLineItems = items.ToList())
            .ReturnsAsync(new List<WorkOrderLineItem>());

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);
        Assert.NotNull(capturedLineItems);
        Assert.Equal(3, capturedLineItems.Count);

        // Verify cost calculations
        var laborItem = capturedLineItems.First(li => li.ItemType == LineItemTypeEnum.LABOR);
        Assert.Equal(250.00m, laborItem.TotalCost); // 100 * 2.5

        var itemItem = capturedLineItems.First(li => li.ItemType == LineItemTypeEnum.ITEM);
        Assert.Equal(75.00m, itemItem.TotalCost); // 25 * 3

        var bothItem = capturedLineItems.First(li => li.ItemType == LineItemTypeEnum.BOTH);
        Assert.Equal(180.00m, bothItem.TotalCost); // (30 * 2) + (80 * 1.5) = 60 + 120
    }

    [Fact]
    public async Task Handler_Should_Handle_Empty_LineItems_List()
    {
        // Given
        var lineItems = new List<CreateWorkOrderLineItemDTO>(); // Empty list
        var command = CreateValidCommand(workOrderLineItems: lineItems);
        SetupValidValidation(command);

        var expectedWorkOrder = CreateWorkOrderFromCommand(command);

        _mockVehicleRepository.Setup(v => v.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockUserRepository.Setup(u => u.ExistsAsync(command.AssignedToUserID)).ReturnsAsync(true);

        _mockWorkOrderRepository.Setup(r => r.AddAsync(It.IsAny<Domain.Entities.WorkOrder>())).ReturnsAsync(expectedWorkOrder);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedWorkOrder.ID, result);

        // Verify line item operations were NOT called
        _mockInventoryItemRepository.Verify(i => i.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        _mockServiceTaskRepository.Verify(s => s.AllExistAsync(It.IsAny<IEnumerable<int>>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.AddRangeAsync(It.IsAny<IEnumerable<WorkOrderLineItem>>()), Times.Never);
    }
}