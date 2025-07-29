using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Contracts.Services;
using Application.Exceptions;
using Application.Features.WorkOrders.Query.GetWorkOrderInvoicePdf;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.WorkOrders.QueryTest.GetWorkOrderInvoicePdf;

public class GetWorkOrderInvoicePdfQueryHandlerTest
{
    private readonly GetWorkOrderInvoicePdfQueryHandler _queryHandler;
    private readonly Mock<IInvoicePdfService> _mockInvoicePdfService;
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IWorkOrderLineItemRepository> _mockWorkOrderLineItemRepository;
    private readonly Mock<IAppLogger<GetWorkOrderInvoicePdfQueryHandler>> _mockLogger;

    public GetWorkOrderInvoicePdfQueryHandlerTest()
    {
        _mockWorkOrderRepository = new Mock<IWorkOrderRepository>();
        _mockWorkOrderLineItemRepository = new Mock<IWorkOrderLineItemRepository>();
        _mockInvoicePdfService = new Mock<IInvoicePdfService>();
        _mockLogger = new Mock<IAppLogger<GetWorkOrderInvoicePdfQueryHandler>>();
        _queryHandler = new GetWorkOrderInvoicePdfQueryHandler(
            _mockInvoicePdfService.Object,
            _mockWorkOrderRepository.Object,
            _mockWorkOrderLineItemRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_PdfBytes_When_WorkOrder_Exists()
    {
        // Arrange
        var workOrderId = 1;
        var workOrder = CreateSampleWorkOrder(workOrderId);
        var lineItems = CreateSampleLineItems();
        var query = new GetWorkOrderInvoicePdfQuery(workOrderId);
        var expectedPdfBytes = "%PDF"u8.ToArray(); // PDF header

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(workOrderId))
            .ReturnsAsync(workOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdAsync(workOrderId))
            .ReturnsAsync(lineItems);

        _mockInvoicePdfService.Setup(s => s.GenerateInvoicePdfAsync(It.IsAny<GetWorkOrderInvoicePdfDTO>()))
            .ReturnsAsync(expectedPdfBytes);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPdfBytes, result);

        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(workOrderId), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdAsync(workOrderId), Times.Once);
        _mockInvoicePdfService.Verify(s => s.GenerateInvoicePdfAsync(It.IsAny<GetWorkOrderInvoicePdfDTO>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_WorkOrder_Not_Found()
    {
        // Arrange
        var workOrderId = 9999;
        var query = new GetWorkOrderInvoicePdfQuery(workOrderId);

        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(workOrderId))
            .ReturnsAsync((WorkOrder?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _queryHandler.Handle(query, CancellationToken.None));

        Assert.Equal(typeof(WorkOrder).ToString(), exception.EntityName);
        Assert.Equal("WorkOrderID", exception.PropertyName);
        Assert.Equal(workOrderId.ToString(), exception.PropertyValue);
    }

    private static WorkOrder CreateSampleWorkOrder(int id)
    {
        return new WorkOrder
        {
            ID = id,
            Title = "Test Work Order",
            Description = "Test Description",
            WorkOrderType = WorkTypeEnum.SCHEDULED,
            PriorityLevel = PriorityLevelEnum.HIGH,
            Status = WorkOrderStatusEnum.COMPLETED,
            ScheduledStartDate = DateTime.UtcNow.AddDays(-1),
            ActualStartDate = DateTime.UtcNow.AddDays(-1),
            StartOdometer = 1000,
            EndOdometer = 1100,
            VehicleID = 1,
            AssignedToUserID = "eabdab35-3fd3-4ef5-af28-3a51f934251c",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicle = new Vehicle
            {
                ID = 1,
                Name = "Test Vehicle",
                Make = "Toyota",
                Model = "Camry",
                VIN = "12345678901234567",
                LicensePlate = "ABC123",
                Year = 2020,
                Status = VehicleStatusEnum.ACTIVE,
                VehicleGroupID = 1,
                LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
                VehicleType = VehicleTypeEnum.CAR,
                Trim = "SE",
                Mileage = 50000,
                EngineHours = 1000,
                FuelCapacity = 50.0,
                FuelType = FuelTypeEnum.PETROL,
                PurchaseDate = DateTime.UtcNow.AddYears(-2),
                PurchasePrice = 25000.00m,
                Location = "Sydney",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                VehicleGroup = null!,
                User = null,
                VehicleImages = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                XrefServiceProgramVehicles = [],
                ServiceReminders = [],
                Issues = [],
                VehicleInspections = []
            },
            User = new User
            {
                Id = "eabdab35-3fd3-4ef5-af28-3a51f934251c",
                FirstName = "John",
                LastName = "Doe",
                Email = "john.doe@example.com",
                UserName = "johndoe",
                HireDate = DateTime.UtcNow.AddYears(-1),
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                MaintenanceHistories = [],
                Vehicles = [],
                IssueAttachments = [],
                VehicleAssignments = [],
                VehicleDocuments = [],
                VehicleInspections = []
            },
            WorkOrderLineItems = [],
            MaintenanceHistories = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    private static List<WorkOrderLineItem> CreateSampleLineItems()
    {
        return
        [
            new WorkOrderLineItem
            {
                ID = 1,
                WorkOrderID = 1,
                ServiceTaskID = 1,
                ItemType = LineItemTypeEnum.LABOR,
                Quantity = 2,
                TotalCost = 100.00m,
                InventoryItemID = null,
                Description = "Labour hours",
                LaborHours = 2.0,
                UnitPrice = 50.00m,
                HourlyRate = 50.00m,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ServiceTask = new ServiceTask
                {
                    ID = 1,
                    Name = "Oil Change",
                    Description = "Change engine oil",
                    Category = ServiceTaskCategoryEnum.PREVENTIVE,
                    EstimatedLabourHours = 2.0,
                    EstimatedCost = 100.00m,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    XrefServiceScheduleServiceTasks = [],
                    MaintenanceHistories = [],
                    WorkOrderLineItems = []
                },
                User = null,
                WorkOrder = null!
            }
        ];
    }
}