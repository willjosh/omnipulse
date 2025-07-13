using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.WorkOrders.Command.CompleteWorkOrder;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.WorkOrders.CommandTest.CreateWorkOrderTest;

public class CompleteWorkOrderCommandHandlerTest
{

    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IMaintenanceHistoryRepository> _mockMaintenanceHistoryRepository;
    private readonly Mock<IAppLogger<CompleteWorkOrderCommandHandler>> _mockLogger;
    private readonly CompleteWorkOrderCommandHandler _handler;

    public CompleteWorkOrderCommandHandlerTest()
    {
        _mockWorkOrderRepository = new();
        _mockMaintenanceHistoryRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<WorkOrderMappingProfile>();
            cfg.AddProfile<MaintenanceHistoryMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _handler = new CompleteWorkOrderCommandHandler(
            _mockWorkOrderRepository.Object,
            _mockMaintenanceHistoryRepository.Object,
            _mockLogger.Object,
            mapper
        );
    }

    private User CreateUser()
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

    private Vehicle CreateVehicle()
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

    private WorkOrder CreateWorkOrder(
        WorkOrderStatusEnum status = WorkOrderStatusEnum.IN_PROGRESS,
        double? endOdometer = 1500,
        DateTime? scheduledStartDate = null,
        DateTime? actualStartDate = null
    )
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
            WorkOrderType = WorkTypeEnum.SCHEDULED,
            PriorityLevel = PriorityLevelEnum.CRITICAL,
            Status = status,
            ScheduledStartDate = scheduledStartDate,
            ActualStartDate = actualStartDate,
            StartOdometer = 1000,
            EndOdometer = endOdometer,
            User = user,
            Vehicle = vehicle,
            MaintenanceHistories = [],
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    private MaintenanceHistory CreateMaintenanceHistory(WorkOrder workOrder)
    {
        return new MaintenanceHistory
        {
            ID = 1,
            WorkOrderID = workOrder.ID,
            ServiceDate = DateTime.UtcNow,
            MileageAtService = 1000,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            WorkOrder = workOrder,
            InventoryTransactions = [],
        };
    }

    [Fact]
    public async Task Handler_Should_Return_WorkOrderID_On_Success()
    {
        // Given 
        var command = new CompleteWorkOrderCommand(1);
        var workOrder = CreateWorkOrder(scheduledStartDate: DateTime.UtcNow.AddDays(-1), actualStartDate: DateTime.UtcNow.AddDays(1));
        var maintenanceHistory = CreateMaintenanceHistory(workOrder);

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(command.ID)).ReturnsAsync(workOrder);
        _mockMaintenanceHistoryRepository.Setup(r => r.AddAsync(It.IsAny<MaintenanceHistory>())).ReturnsAsync(maintenanceHistory);
        _mockWorkOrderRepository.Setup(r => r.Update(It.IsAny<WorkOrder>()));

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then 
        Assert.Equal(workOrder.ID, result);
        Assert.Equal(WorkOrderStatusEnum.COMPLETED, workOrder.Status);
        Assert.NotNull(workOrder.ActualStartDate);

        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(command.ID), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_WorkOrder_Not_Found()
    {
        // Given
        var command = new CompleteWorkOrderCommand(999);
        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(command.ID)).ReturnsAsync((WorkOrder?)null);

        // When && Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(command.ID), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
        _mockWorkOrderRepository.Verify(r => r.Update(It.IsAny<WorkOrder>()), Times.Never);
    }

    [Theory]
    [InlineData(null, null, 1500.0)]
    [InlineData("2024-01-01", null, 1500.0)]
    [InlineData("2024-01-01", "2024-01-02", null)]
    public async Task Handler_Should_Throw_IncompleteWorkOrderException_When_Required_Fields_Missing(
        string? scheduledStartDateStr,
        string? actualStartDateStr,
        double? endOdometer)
    {
        // Given
        var command = new CompleteWorkOrderCommand(1);

        DateTime? scheduledStartDate = scheduledStartDateStr != null ? DateTime.Parse(scheduledStartDateStr) : null;
        DateTime? actualStartDate = actualStartDateStr != null ? DateTime.Parse(actualStartDateStr) : null;

        var workOrder = CreateWorkOrder(
            status: WorkOrderStatusEnum.IN_PROGRESS,
            endOdometer: endOdometer,
            scheduledStartDate: scheduledStartDate,
            actualStartDate: actualStartDate
        );

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(command.ID))
                                .ReturnsAsync(workOrder);

        // When & Then
        var exception = await Assert.ThrowsAsync<IncompleteWorkOrderException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        // Verify no further processing occurs
        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(command.ID), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
        _mockWorkOrderRepository.Verify(r => r.Update(It.IsAny<WorkOrder>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_WorkOrderAlreadyCompletedException_When_WorkOrder_Is_Already_Completed()
    {
        // Given
        var command = new CompleteWorkOrderCommand(1);
        var workOrder = CreateWorkOrder(WorkOrderStatusEnum.COMPLETED);

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(command.ID))
                                .ReturnsAsync(workOrder);

        // When & Then 
        await Assert.ThrowsAsync<WorkOrderAlreadyCompletedException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        // Verify no line items or maintenance history operations
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
        _mockWorkOrderRepository.Verify(r => r.Update(It.IsAny<WorkOrder>()), Times.Never);
    }

}