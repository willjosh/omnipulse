using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrders.Query.GetWorkOrderDetail;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.WorkOrders.QueryTest.GetWorkOrderDetail;

public class GetWorkOrderDetailHandlerTest
{
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IWorkOrderLineItemRepository> _mockWorkOrderLineItemRepository;
    private readonly Mock<IAppLogger<GetWorkOrderQueryHandler>> _mockLogger;
    private readonly IMapper _mapper;
    private readonly GetWorkOrderQueryHandler _handler;

    public GetWorkOrderDetailHandlerTest()
    {
        _mockWorkOrderRepository = new Mock<IWorkOrderRepository>();
        _mockWorkOrderLineItemRepository = new Mock<IWorkOrderLineItemRepository>();
        _mockLogger = new Mock<IAppLogger<GetWorkOrderQueryHandler>>();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkOrderMappingProfile>();
            cfg.AddProfile<WorkOrderLineItemMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new GetWorkOrderQueryHandler(
            _mockWorkOrderRepository.Object,
            _mockWorkOrderLineItemRepository.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    public User CreateUser()
    {
        return new User
        {
            Id = "guid-1",
            FirstName = "John",
            LastName = "Doe",
            HireDate = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
            Email = "john@gmail.com",
            IsActive = true,
            CreatedAt = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2004, 4, 11, 0, 0, 0, DateTimeKind.Utc),
            MaintenanceHistories = [],
            Vehicles = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleInspections = [],
        };
    }

    public Vehicle CreateVehicle()
    {
        return new Vehicle
        {
            ID = 1,
            CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Name = "Toyota Corolla",
            Make = "Toyota",
            Model = "Corolla",
            Year = 2023,
            VIN = "1234567890ABCDEFG",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 2,
            Trim = "LE",
            Mileage = 50000,
            EngineHours = 1000,
            FuelCapacity = 50.0,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = new DateTime(2021, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            PurchasePrice = 25000.00m,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
            Location = "Sydney",
            AssignedTechnicianID = "GUID123",
            VehicleGroup = null!,
            User = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };
    }

    private WorkOrder CreateWorkOrder()
    {
        var user = CreateUser();
        var vehicle = CreateVehicle();

        return new WorkOrder
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleID = 1,
            AssignedToUserID = "guid-1",
            Title = "Test Work Order",
            Description = "This is a test work order",
            WorkOrderType = Domain.Entities.Enums.WorkTypeEnum.SCHEDULED,
            PriorityLevel = Domain.Entities.Enums.PriorityLevelEnum.CRITICAL,
            Status = Domain.Entities.Enums.WorkOrderStatusEnum.IN_PROGRESS,
            ScheduledStartDate = DateTime.UtcNow,
            StartOdometer = 1000,
            User = user,
            Vehicle = vehicle,
            MaintenanceHistories = [],
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    private InventoryItem CreateInventoryItem()
    {
        return new InventoryItem
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ItemNumber = "INV-001",
            ItemName = "Test Inventory Item",
            Description = "This is a test inventory item",
            Category = Domain.Entities.Enums.InventoryItemCategoryEnum.FILTERS,
            Manufacturer = "Test Manufacturer",
            ManufacturerPartNumber = "MPN-001",
            UniversalProductCode = "UPC-001",
            UnitCost = 50.00m,
            UnitCostMeasurementUnit = Domain.Entities.Enums.InventoryItemUnitCostMeasurementUnitEnum.Litre,
            Supplier = "Test Supplier",
            WeightKG = 0.5,
            IsActive = true,
            Inventories = [],
            WorkOrderLineItems = []
        };
    }

    private ServiceTask CreateServiceTask()
    {
        return new ServiceTask
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Test Service Task",
            Description = "This is a test service task",
            EstimatedLabourHours = 2.0,
            EstimatedCost = 100.00m,
            Category = Domain.Entities.Enums.ServiceTaskCategoryEnum.INSPECTION,
            IsActive = true,
            ServiceScheduleTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = []
        };
    }

    private List<WorkOrderLineItem> CreateWorkOrderLineItems()
    {
        var user = CreateUser();
        var inventoryItem = CreateInventoryItem();
        var serviceTask = CreateServiceTask();
        var workOrder = CreateWorkOrder();

        var lineItems = new List<WorkOrderLineItem>
        {
            // Labor line item
            new WorkOrderLineItem
            {
                ID = 1,
                WorkOrderID = 1,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                Quantity = 1,
                Description = "Labor work description",
                AssignedToUserID = "guid-1",
                InventoryItemID = null,
                LaborHours = 2.0,
                HourlyRate = 75.00m,
                UnitPrice = null,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalCost = 0,
                // Navigation properties
                User = user,
                ServiceTask = serviceTask,
                InventoryItem = null,
                WorkOrder = workOrder
            },
            // Item/Parts line item
            new WorkOrderLineItem
            {
                ID = 2,
                WorkOrderID = 1,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.ITEM,
                Quantity = 2,
                Description = "Parts description",
                AssignedToUserID = "guid-1",
                InventoryItemID = 1,
                LaborHours = null,
                HourlyRate = null,
                UnitPrice = 25.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalCost = 0,
                // Navigation properties
                User = user,
                ServiceTask = serviceTask,
                InventoryItem = inventoryItem,
                WorkOrder = workOrder
            },
            // Both labor and item
            new WorkOrderLineItem
            {
                ID = 3,
                WorkOrderID = 1,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.BOTH,
                Quantity = 1,
                Description = "Labor and parts combined",
                AssignedToUserID = "guid-1",
                InventoryItemID = 1,
                LaborHours = 1.5,
                HourlyRate = 80.00m,
                UnitPrice = 30.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                TotalCost = 0,
                // Navigation properties
                User = user,
                ServiceTask = serviceTask,
                InventoryItem = inventoryItem,
                WorkOrder = workOrder
            }
        };

        // Calculate total costs for each line item
        foreach (var lineItem in lineItems)
        {
            lineItem.CalculateTotalCost();
        }

        return lineItems;
    }

    [Fact]
    public async Task Handler_Should_Return_GetWorkOrderDetailDTO_On_Success()
    {
        // Given
        var workOrder = CreateWorkOrder();
        var lineItems = new List<WorkOrderLineItem>();
        var query = new GetWorkOrderDetailQuery(1);

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(workOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdAsync(It.IsAny<int>()))
            .ReturnsAsync(lineItems);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<GetWorkOrderDetailDTO>(result);
        Assert.Equal(1, result.ID);
        Assert.Equal("Test Work Order", result.Title);
        Assert.Equal("This is a test work order", result.Description);
        Assert.Equal(WorkTypeEnum.SCHEDULED, result.WorkOrderType);
        Assert.Equal(PriorityLevelEnum.CRITICAL, result.PriorityLevel);
        Assert.Equal(WorkOrderStatusEnum.IN_PROGRESS, result.Status);
        Assert.Equal(1000, result.StartOdometer);
        Assert.Equal(1, result.VehicleID);
        Assert.Equal("Toyota Corolla", result.VehicleName);
        Assert.Equal("guid-1", result.AssignedToUserID);
        Assert.Equal("John Doe", result.AssignedToUserName); // Assuming GetFullName() returns "FirstName LastName"
        Assert.NotNull(result.WorkOrderLineItems);
        Assert.Empty(result.WorkOrderLineItems);

        // Verify repository calls
        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(1), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_NonExistent_WorkOrderID()
    {
        // Given
        var nonExistentWorkOrderId = 999;
        var query = new GetWorkOrderDetailQuery(nonExistentWorkOrderId);

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(nonExistentWorkOrderId))
            .ReturnsAsync((WorkOrder?)null);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(query, CancellationToken.None)
        );

        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(nonExistentWorkOrderId), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_GetWorkOrderDetailDTO_With_LineItems()
    {
        // Given
        var workOrder = CreateWorkOrder();
        var lineItems = CreateWorkOrderLineItems();
        var query = new GetWorkOrderDetailQuery(1);

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(workOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdAsync(It.IsAny<int>()))
            .ReturnsAsync(lineItems);

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<GetWorkOrderDetailDTO>(result);
        Assert.Equal(1, result.ID);
        Assert.Equal("Test Work Order", result.Title);

        // Verify line items are mapped correctly
        Assert.NotNull(result.WorkOrderLineItems);
        Assert.Equal(3, result.WorkOrderLineItems.Count);

        // Verify first line item (LABOR)
        var laborLineItem = result.WorkOrderLineItems.First(li => li.ItemType == LineItemTypeEnum.LABOR);
        Assert.Equal(1, laborLineItem.ID);
        Assert.Equal(1, laborLineItem.WorkOrderID);
        Assert.Equal(LineItemTypeEnum.LABOR, laborLineItem.ItemType);
        Assert.Equal(1, laborLineItem.Quantity);
        Assert.Equal("Labor work description", laborLineItem.Description);
        Assert.Equal("John Doe", laborLineItem.AssignedToUserName);
        Assert.Equal("Test Service Task", laborLineItem.ServiceTaskName);
        Assert.Null(laborLineItem.InventoryItemID);
        Assert.Equal("N/A", laborLineItem.InventoryItemName); // Since no inventory item

        // Verify second line item (ITEM)
        var itemLineItem = result.WorkOrderLineItems.First(li => li.ItemType == LineItemTypeEnum.ITEM);
        Assert.Equal(2, itemLineItem.ID);
        Assert.Equal(1, itemLineItem.WorkOrderID);
        Assert.Equal(LineItemTypeEnum.ITEM, itemLineItem.ItemType);
        Assert.Equal(2, itemLineItem.Quantity);
        Assert.Equal("Parts description", itemLineItem.Description);
        Assert.Equal("Test Inventory Item", itemLineItem.InventoryItemName);
        Assert.Equal(1, itemLineItem.InventoryItemID);

        // Verify third line item (BOTH)
        var bothLineItem = result.WorkOrderLineItems.First(li => li.ItemType == LineItemTypeEnum.BOTH);
        Assert.Equal(3, bothLineItem.ID);
        Assert.Equal(LineItemTypeEnum.BOTH, bothLineItem.ItemType);
        Assert.Equal("Labor and parts combined", bothLineItem.Description);
        Assert.Equal("Test Inventory Item", bothLineItem.InventoryItemName);

        Assert.Equal(270.00m, result.TotalLaborCost); // Labor ($150) + Both labor portion ($120) = $270
        Assert.Equal(80.00m, result.TotalItemCost);   // Item ($50) + Both item portion ($30) = $80
        Assert.Equal(350.00m, result.TotalCost);      // Total of all line items

        // Verify individual line item costs (if mapped to DTO)
        Assert.Equal(150.00m, laborLineItem.SubTotal);  // 2.0 * $75
        Assert.Equal(50.00m, itemLineItem.SubTotal);    // 2 * $25
        Assert.Equal(150.00m, bothLineItem.SubTotal);   // (1.5 * $80) + (1 * $30)

        // Verify repository calls
        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(1), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdAsync(1), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_GetWorkOrderDetailDTO_With_Empty_LineItems()
    {
        // Given
        var workOrder = CreateWorkOrder();
        var query = new GetWorkOrderDetailQuery(1);

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(It.IsAny<int>()))
            .ReturnsAsync(workOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdAsync(It.IsAny<int>()))
            .ReturnsAsync(new List<WorkOrderLineItem>()); // Empty list instead of null

        // When
        var result = await _handler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.NotNull(result.WorkOrderLineItems);
        Assert.Empty(result.WorkOrderLineItems);
        Assert.Equal(0.00m, result.TotalCost);
        Assert.Equal(0.00m, result.TotalLaborCost);
        Assert.Equal(0.00m, result.TotalItemCost);
    }
}