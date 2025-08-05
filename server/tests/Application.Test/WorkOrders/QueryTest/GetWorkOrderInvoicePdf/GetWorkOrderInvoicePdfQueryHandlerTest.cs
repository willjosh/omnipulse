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
    private readonly Mock<IUserRepository> _mockUserRepository; // ✅ Added missing dependency
    private readonly Mock<IWorkOrderLineItemRepository> _mockWorkOrderLineItemRepository;
    private readonly Mock<IAppLogger<GetWorkOrderInvoicePdfQueryHandler>> _mockLogger;

    public GetWorkOrderInvoicePdfQueryHandlerTest()
    {
        _mockWorkOrderRepository = new Mock<IWorkOrderRepository>();
        _mockUserRepository = new Mock<IUserRepository>(); // ✅ Added missing dependency
        _mockWorkOrderLineItemRepository = new Mock<IWorkOrderLineItemRepository>();
        _mockInvoicePdfService = new Mock<IInvoicePdfService>();
        _mockLogger = new Mock<IAppLogger<GetWorkOrderInvoicePdfQueryHandler>>();

        _queryHandler = new GetWorkOrderInvoicePdfQueryHandler(
            _mockInvoicePdfService.Object,
            _mockWorkOrderRepository.Object,
            _mockUserRepository.Object, // ✅ Added missing dependency
            _mockWorkOrderLineItemRepository.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_PdfBytes_When_WorkOrder_Exists()
    {
        // Arrange
        var workOrderId = 1;
        var issuedByUserId = "eabdab35-3fd3-4ef5-af28-3a51f934251c";
        var workOrder = CreateSampleWorkOrder(workOrderId);
        var issuedByUser = CreateSampleUser(issuedByUserId);
        var lineItems = CreateSampleLineItems();
        var query = new GetWorkOrderInvoicePdfQuery(workOrderId, issuedByUserId);
        var expectedPdfBytes = "%PDF"u8.ToArray();

        _mockUserRepository.Setup(r => r.GetByIdAsync(issuedByUserId)) // ✅ Added missing setup
            .ReturnsAsync(issuedByUser);
        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(workOrderId))
            .ReturnsAsync(workOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdAsync(workOrderId)) // ✅ Fixed method name (singular)
            .ReturnsAsync(lineItems);

        _mockInvoicePdfService.Setup(s => s.GenerateInvoicePdfAsync(It.IsAny<GetWorkOrderInvoicePdfDTO>()))
            .ReturnsAsync(expectedPdfBytes);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedPdfBytes, result);

        _mockUserRepository.Verify(r => r.GetByIdAsync(issuedByUserId), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(workOrderId), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdAsync(workOrderId), Times.Once); // ✅ Fixed method name
        _mockInvoicePdfService.Verify(s => s.GenerateInvoicePdfAsync(It.IsAny<GetWorkOrderInvoicePdfDTO>()), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_User_Not_Found()
    {
        // Arrange
        var workOrderId = 1;
        var issuedByUserId = "invalid-user-id";
        var query = new GetWorkOrderInvoicePdfQuery(workOrderId, issuedByUserId);

        _mockUserRepository.Setup(r => r.GetByIdAsync(issuedByUserId))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _queryHandler.Handle(query, CancellationToken.None));

        Assert.Equal(nameof(User), exception.EntityName);
        Assert.Equal(nameof(GetWorkOrderInvoicePdfQuery.IssuedByUserID), exception.PropertyName);
        Assert.Equal(issuedByUserId, exception.PropertyValue);

        // Verify that work order repository is not called when user is not found
        _mockWorkOrderRepository.Verify(r => r.GetWorkOrderWithDetailsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_WorkOrder_Not_Found()
    {
        // Arrange
        var workOrderId = 9999;
        var issuedByUserId = "eabdab35-3fd3-4ef5-af28-3a51f934251c";
        var issuedByUser = CreateSampleUser(issuedByUserId);
        var query = new GetWorkOrderInvoicePdfQuery(workOrderId, issuedByUserId);

        _mockUserRepository.Setup(r => r.GetByIdAsync(issuedByUserId))
            .ReturnsAsync(issuedByUser);
        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(workOrderId))
            .ReturnsAsync((WorkOrder?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _queryHandler.Handle(query, CancellationToken.None));

        Assert.Equal(nameof(WorkOrder), exception.EntityName);
        Assert.Equal(nameof(GetWorkOrderInvoicePdfQuery.WorkOrderId), exception.PropertyName);
        Assert.Equal(workOrderId.ToString(), exception.PropertyValue);

        // Verify that line items repository is not called when work order is not found
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Generate_Pdf_With_Correct_Data()
    {
        // Arrange
        var workOrderId = 1;
        var issuedByUserId = "eabdab35-3fd3-4ef5-af28-3a51f934251c";
        var workOrder = CreateSampleWorkOrder(workOrderId);
        var issuedByUser = CreateSampleUser(issuedByUserId);
        var lineItems = CreateSampleLineItems();
        var query = new GetWorkOrderInvoicePdfQuery(workOrderId, issuedByUserId);
        var expectedPdfBytes = "%PDF"u8.ToArray();

        GetWorkOrderInvoicePdfDTO? capturedDto = null;

        _mockUserRepository.Setup(r => r.GetByIdAsync(issuedByUserId))
            .ReturnsAsync(issuedByUser);
        _mockWorkOrderRepository.Setup(r => r.GetWorkOrderWithDetailsAsync(workOrderId))
            .ReturnsAsync(workOrder);
        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdAsync(workOrderId))
            .ReturnsAsync(lineItems);

        _mockInvoicePdfService.Setup(s => s.GenerateInvoicePdfAsync(It.IsAny<GetWorkOrderInvoicePdfDTO>()))
            .Callback<GetWorkOrderInvoicePdfDTO>(dto => capturedDto = dto)
            .ReturnsAsync(expectedPdfBytes);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(capturedDto);
        Assert.Equal(workOrderId, capturedDto.WorkOrderID);
        Assert.Equal("Test Work Order", capturedDto.WorkOrderTitle);
        Assert.Equal("Test Vehicle", capturedDto.VehicleName);
        Assert.Equal("John Doe", capturedDto.IssuedByUserName);
        Assert.Equal("John Doe", capturedDto.AssignedToUserName);
        Assert.Single(capturedDto.WorkOrderLineItems);
    }

    private static User CreateSampleUser(string userId)
    {
        return new User
        {
            Id = userId,
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
            Inspections = [],
            InventoryTransactions = []
        };
    }

    private static WorkOrder CreateSampleWorkOrder(int id)
    {
        var vehicleGroup = new VehicleGroup
        {
            ID = 1,
            Name = "Fleet",
            Description = "Main fleet",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
        };

        var user = new User
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
            Inspections = [],
            InventoryTransactions = []
        };

        var vehicle = new Vehicle
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
            AssignedTechnicianID = "eabdab35-3fd3-4ef5-af28-3a51f934251c",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleGroup = vehicleGroup,
            User = user,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = []
        };

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
            ActualCompletionDate = DateTime.UtcNow,
            StartOdometer = 1000,
            EndOdometer = 1100,
            VehicleID = 1,
            AssignedToUserID = "eabdab35-3fd3-4ef5-af28-3a51f934251c",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicle = vehicle,
            User = user,
            WorkOrderLineItems = [],
            MaintenanceHistories = [],
            Invoices = [],
            InventoryTransactions = []
        };
    }

    private static List<WorkOrderLineItem> CreateSampleLineItems()
    {
        var serviceTask = new ServiceTask
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
        };

        var user = new User
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
            Inspections = [],
            InventoryTransactions = []
        };

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
                AssignedToUserID = "eabdab35-3fd3-4ef5-af28-3a51f934251c",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ServiceTask = serviceTask,
                User = user,
                WorkOrder = null!,
                InventoryItem = null
            }
        ];
    }
}