using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrders.Query.GetAllWorkOrder;
using Application.Features.WorkOrders.Query.GetWorkOrderDetail;
using Application.MappingProfiles;
using Application.Models;
using Application.Models.PaginationModels;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Enums;
using FluentValidation;
using Moq;

namespace Application.Test.WorkOrders.QueryTest.GetAllWorkOrder;

public class GetAllWorkOrderQueryHandlerTest
{
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IWorkOrderLineItemRepository> _mockWorkOrderLineItemRepository;
    private readonly Mock<IAppLogger<GetAllWorkOrderQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllWorkOrderQuery>> _mockValidator;
    private readonly GetAllWorkOrderQueryHandler _handler;

    public GetAllWorkOrderQueryHandlerTest()
    {
        _mockWorkOrderRepository = new();
        _mockWorkOrderLineItemRepository = new();
        _mockLogger = new();
        _mockValidator = new Mock<IValidator<GetAllWorkOrderQuery>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkOrderMappingProfile>();
            cfg.AddProfile<WorkOrderLineItemMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _handler = new GetAllWorkOrderQueryHandler(
            _mockWorkOrderRepository.Object,
            _mockWorkOrderLineItemRepository.Object,
            _mockLogger.Object,
            mapper,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(GetAllWorkOrderQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(GetAllWorkOrderQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(invalidResult);
    }

    // Helper method to create test vehicle
    private Vehicle CreateTestVehicle()
    {
        return new Vehicle
        {
            ID = 1,
            Name = "Toyota Camry 2023",
            Make = "Toyota",
            Model = "Camry",
            Year = 2023,
            VIN = "1234567890ABCDEFG",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            AssignedTechnicianID = "user123",
            Trim = "LE",
            Mileage = 50000,
            EngineHours = 1000,
            FuelCapacity = 60.0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-2),
            PurchasePrice = 30000.00m,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Sydney",
            VehicleGroup = CreateTestVehicleGroup(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };
    }

    // Helper method to create test vehicle group
    private VehicleGroup CreateTestVehicleGroup()
    {
        return new VehicleGroup
        {
            ID = 1,
            Name = "Fleet Group 1",
            Description = "Main fleet vehicles",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    // Helper method to create test user
    private User CreateTestUser()
    {
        return new User
        {
            Id = "user123",
            FirstName = "John",
            LastName = "Doe",
            HireDate = DateTime.UtcNow.AddYears(-1),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            MaintenanceHistories = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleInspections = [],
            Vehicles = []
        };
    }

    // Helper method to create test service task
    private ServiceTask CreateTestServiceTask(int id = 1, string name = "Oil Change Service")
    {
        return new ServiceTask
        {
            ID = id,
            Name = name,
            Description = "Complete service task",
            EstimatedLabourHours = 1.5,
            EstimatedCost = 120.00m,
            Category = ServiceTaskCategoryEnum.CORRECTIVE,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceScheduleTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = []
        };
    }

    // Helper method to create test work orders
    private List<WorkOrder> CreateTestWorkOrders()
    {
        var vehicle = CreateTestVehicle();
        var user = CreateTestUser();

        return new List<WorkOrder>
        {
            new WorkOrder
            {
                ID = 1,
                VehicleID = 1,
                AssignedToUserID = "user123",
                Title = "Oil Change",
                Description = "Regular maintenance oil change",
                WorkOrderType = WorkTypeEnum.SCHEDULED,
                PriorityLevel = PriorityLevelEnum.CRITICAL,
                Status = WorkOrderStatusEnum.CREATED,
                ScheduledStartDate = DateTime.UtcNow.AddDays(1),
                StartOdometer = 50000,
                Vehicle = vehicle,
                User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                WorkOrderLineItems = [],
                Invoices = [],
                InventoryTransactions = []
            },
            new WorkOrder
            {
                ID = 2,
                VehicleID = 1,
                AssignedToUserID = "user123",
                Title = "Brake Inspection",
                Description = "Annual brake system inspection",
                WorkOrderType = WorkTypeEnum.UNSCHEDULED,
                PriorityLevel = PriorityLevelEnum.HIGH,
                Status = WorkOrderStatusEnum.IN_PROGRESS,
                ScheduledStartDate = DateTime.UtcNow,
                ActualStartDate = DateTime.UtcNow,
                StartOdometer = 51000,
                Vehicle = vehicle,
                User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                WorkOrderLineItems = [],
                Invoices = [],
                InventoryTransactions = []
            }
        };
    }

    // Helper method to create test work order line items
    private List<WorkOrderLineItem> CreateTestWorkOrderLineItems()
    {
        var serviceTask = CreateTestServiceTask();
        var user = CreateTestUser();

        return new List<WorkOrderLineItem>
        {
            new WorkOrderLineItem
            {
                ID = 1,
                WorkOrderID = 1,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                Quantity = 1,
                LaborHours = 2.0,
                HourlyRate = 75.00m,
                TotalCost = 150.00m,
                Description = "Labor for oil change",
                AssignedToUserID = "user123",
                User = user,
                WorkOrder = null!, // Will be set when needed
                ServiceTask = serviceTask,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new WorkOrderLineItem
            {
                ID = 2,
                WorkOrderID = 1,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.ITEM,
                Quantity = 5,
                UnitPrice = 8.00m,
                TotalCost = 40.00m,
                Description = "Engine oil quarts",
                AssignedToUserID = "user123",
                User = user,
                WorkOrder = null!, // Will be set when needed
                ServiceTask = serviceTask,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new WorkOrderLineItem
            {
                ID = 3,
                WorkOrderID = 2,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.BOTH,
                Quantity = 2,
                LaborHours = 1.5,
                HourlyRate = 85.00m,
                UnitPrice = 25.00m,
                TotalCost = 177.50m, // (1.5 * 85) + (2 * 25)
                Description = "Brake pad replacement",
                AssignedToUserID = "user123",
                User = user,
                WorkOrder = null!, // Will be set when needed
                ServiceTask = serviceTask,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_On_Success()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            Search = "Oil",
            SortBy = "Title",
            SortDescending = false
        };

        var query = new GetAllWorkOrderQuery(parameters);
        SetupValidValidation(query);

        var workOrders = CreateTestWorkOrders();
        var lineItems = CreateTestWorkOrderLineItems();

        var pagedWorkOrders = new PagedResult<WorkOrder>
        {
            Items = workOrders,
            TotalCount = 25,
            PageNumber = 1,
            PageSize = 5
        };

        _mockWorkOrderRepository.Setup(r => r.GetAllWorkOrderPagedAsync(parameters))
                                .ReturnsAsync(pagedWorkOrders);

        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()))
                                        .ReturnsAsync(lineItems);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetWorkOrderDetailDTO>>(result);

        Assert.Equal(25, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(5, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        // Check items
        Assert.Equal(2, result.Items.Count);

        var firstWorkOrder = result.Items[0];
        Assert.Equal(1, firstWorkOrder.ID);
        Assert.Equal("Oil Change", firstWorkOrder.Title);
        Assert.Equal("Regular maintenance oil change", firstWorkOrder.Description);
        Assert.Equal(WorkTypeEnum.SCHEDULED, firstWorkOrder.WorkOrderType);
        Assert.Equal(PriorityLevelEnum.CRITICAL, firstWorkOrder.PriorityLevel);
        Assert.Equal(WorkOrderStatusEnum.CREATED, firstWorkOrder.Status);
        Assert.Equal(50000, firstWorkOrder.StartOdometer);
        Assert.Equal(1, firstWorkOrder.VehicleID);
        Assert.Equal("Toyota Camry 2023", firstWorkOrder.VehicleName);
        Assert.Equal("user123", firstWorkOrder.AssignedToUserID);
        Assert.Equal("John Doe", firstWorkOrder.AssignedToUserName);

        // Check line items are included
        Assert.NotNull(firstWorkOrder.WorkOrderLineItems);
        Assert.Equal(2, firstWorkOrder.WorkOrderLineItems.Count);

        // Check totals are calculated
        Assert.Equal(190.00m, firstWorkOrder.TotalCost); // 150 + 40
        Assert.Equal(150.00m, firstWorkOrder.TotalLaborCost);
        Assert.Equal(40.00m, firstWorkOrder.TotalItemCost);

        // Verify line item details
        var laborLineItem = firstWorkOrder.WorkOrderLineItems.First(li => li.ItemType == LineItemTypeEnum.LABOR);
        Assert.Equal(1, laborLineItem.ID);
        Assert.Equal(1, laborLineItem.WorkOrderID);
        Assert.Equal(LineItemTypeEnum.LABOR, laborLineItem.ItemType);
        Assert.Equal(1, laborLineItem.Quantity);
        Assert.Equal("Labor for oil change", laborLineItem.Description);
        Assert.Equal("user123", laborLineItem.AssignedToUserID);
        Assert.Equal("John Doe", laborLineItem.AssignedToUserName);
        Assert.Equal(1, laborLineItem.ServiceTaskID);
        Assert.Equal("Oil Change Service", laborLineItem.ServiceTaskName);
        Assert.Equal(150.00m, laborLineItem.SubTotal);
        Assert.Equal(150.00m, laborLineItem.LaborCost);
        Assert.Equal(0.00m, laborLineItem.ItemCost);

        var itemLineItem = firstWorkOrder.WorkOrderLineItems.First(li => li.ItemType == LineItemTypeEnum.ITEM);
        Assert.Equal(2, itemLineItem.ID);
        Assert.Equal(LineItemTypeEnum.ITEM, itemLineItem.ItemType);
        Assert.Equal(5, itemLineItem.Quantity);
        Assert.Equal("Engine oil quarts", itemLineItem.Description);
        Assert.Equal(40.00m, itemLineItem.SubTotal);
        Assert.Equal(0.00m, itemLineItem.LaborCost);
        Assert.Equal(40.00m, itemLineItem.ItemCost);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.GetAllWorkOrderPagedAsync(parameters), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Work_Orders()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "NonExistentWorkOrder"
        };

        var query = new GetAllWorkOrderQuery(parameters);
        SetupValidValidation(query);

        var emptyPagedResult = new PagedResult<WorkOrder>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockWorkOrderRepository.Setup(r => r.GetAllWorkOrderPagedAsync(parameters))
                                .ReturnsAsync(emptyPagedResult);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.GetAllWorkOrderPagedAsync(parameters), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Handle_Work_Orders_Without_Line_Items()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5
        };

        var query = new GetAllWorkOrderQuery(parameters);
        SetupValidValidation(query);

        var workOrders = CreateTestWorkOrders();
        var pagedWorkOrders = new PagedResult<WorkOrder>
        {
            Items = workOrders,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 5
        };

        _mockWorkOrderRepository.Setup(r => r.GetAllWorkOrderPagedAsync(parameters))
                                .ReturnsAsync(pagedWorkOrders);

        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()))
                                        .ReturnsAsync([]);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);

        var firstWorkOrder = result.Items[0];
        Assert.NotNull(firstWorkOrder.WorkOrderLineItems);
        Assert.Empty(firstWorkOrder.WorkOrderLineItems);
        Assert.Equal(0, firstWorkOrder.TotalCost);
        Assert.Equal(0, firstWorkOrder.TotalLaborCost);
        Assert.Equal(0, firstWorkOrder.TotalItemCost);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.GetAllWorkOrderPagedAsync(parameters), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 3
        };

        var query = new GetAllWorkOrderQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<WorkOrder>
        {
            Items = [],
            TotalCount = 10,
            PageNumber = 2,
            PageSize = 3
        };

        _mockWorkOrderRepository.Setup(r => r.GetAllWorkOrderPagedAsync(parameters))
                                .ReturnsAsync(pagedResult);

        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()))
                                        .ReturnsAsync([]);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(4, result.TotalPages); // 10 / 3 = 4 pages (rounded up)
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.GetAllWorkOrderPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Last_Page()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 3,
            PageSize = 5
        };

        var query = new GetAllWorkOrderQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<WorkOrder>
        {
            Items = [],
            TotalCount = 12,
            PageNumber = 3,
            PageSize = 5
        };

        _mockWorkOrderRepository.Setup(r => r.GetAllWorkOrderPagedAsync(parameters))
                                .ReturnsAsync(pagedResult);

        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()))
                                        .ReturnsAsync([]);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        // Verify mocks were called correctly
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.GetAllWorkOrderPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 0,
            PageSize = 10
        };

        var query = new GetAllWorkOrderQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(query, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.GetAllWorkOrderPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Calculate_Totals_Correctly_For_Mixed_Line_Items()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };

        var query = new GetAllWorkOrderQuery(parameters);
        SetupValidValidation(query);

        var vehicle = CreateTestVehicle();
        var user = CreateTestUser();

        var workOrders = new List<WorkOrder>
        {
            new WorkOrder
            {
                ID = 1,
                VehicleID = 1,
                AssignedToUserID = "user123",
                Title = "Complex Repair",
                WorkOrderType = WorkTypeEnum.UNSCHEDULED,
                PriorityLevel = PriorityLevelEnum.HIGH,
                Status = WorkOrderStatusEnum.IN_PROGRESS,
                StartOdometer = 60000,
                Vehicle = vehicle,
                User = user,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                WorkOrderLineItems = [],
                Invoices = [],
                InventoryTransactions = []
            }
        };

        var lineItems = new List<WorkOrderLineItem>
        {
            new WorkOrderLineItem
            {
                ID = 1,
                WorkOrderID = 1,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                Quantity = 1,
                LaborHours = 3.0,
                HourlyRate = 100.00m,
                TotalCost = 300.00m,
                AssignedToUserID = "user123",
                Description = "Complex repair labor",
                ServiceTask = CreateTestServiceTask(1, "Labor Task"),
                User = user,
                WorkOrder = workOrders[0],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            },
            new WorkOrderLineItem
            {
                ID = 2,
                WorkOrderID = 1,
                ServiceTaskID = 2,
                ItemType = LineItemTypeEnum.ITEM,
                Quantity = 2,
                UnitPrice = 50.00m,
                TotalCost = 100.00m,
                AssignedToUserID = "user123",
                Description = "Replacement parts",
                ServiceTask = CreateTestServiceTask(2, "Parts Task"),
                User = user,
                WorkOrder = workOrders[0],
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            }
        };

        var pagedResult = new PagedResult<WorkOrder>
        {
            Items = workOrders,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };

        _mockWorkOrderRepository.Setup(r => r.GetAllWorkOrderPagedAsync(parameters))
                                .ReturnsAsync(pagedResult);

        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()))
                                        .ReturnsAsync(lineItems);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Single(result.Items);

        var workOrder = result.Items[0];
        Assert.Equal(2, workOrder.WorkOrderLineItems.Count);
        Assert.Equal(400.00m, workOrder.TotalCost); // 300 + 100
        Assert.Equal(300.00m, workOrder.TotalLaborCost);
        Assert.Equal(100.00m, workOrder.TotalItemCost);

        // Verify line item calculations
        var laborLineItem = workOrder.WorkOrderLineItems.First(li => li.ItemType == LineItemTypeEnum.LABOR);
        Assert.Equal(300.00m, laborLineItem.LaborCost);
        Assert.Equal(0.00m, laborLineItem.ItemCost);
        Assert.Equal(300.00m, laborLineItem.SubTotal);

        var itemLineItem = workOrder.WorkOrderLineItems.First(li => li.ItemType == LineItemTypeEnum.ITEM);
        Assert.Equal(0.00m, itemLineItem.LaborCost);
        Assert.Equal(100.00m, itemLineItem.ItemCost);
        Assert.Equal(100.00m, itemLineItem.SubTotal);
    }
}