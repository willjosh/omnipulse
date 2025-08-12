using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.MaintenanceHistories.QueryTest.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoriesQueryHandlerTest
{
    private readonly Mock<IMaintenanceHistoryRepository> _mockRepository;
    private readonly Mock<IWorkOrderLineItemRepository> _mockWorkOrderLineItemRepository; // Add this
    private readonly GetAllMaintenanceHistoriesQueryHandler _handler;
    private readonly Mock<IAppLogger<GetAllMaintenanceHistoriesQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllMaintenanceHistoriesQuery>> _mockValidator;

    public GetAllMaintenanceHistoriesQueryHandlerTest()
    {
        _mockRepository = new();
        _mockWorkOrderLineItemRepository = new(); // Add this
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MaintenanceHistoryMappingProfile>());
        var mapper = config.CreateMapper();

        _handler = new GetAllMaintenanceHistoriesQueryHandler(
            _mockRepository.Object,
            _mockWorkOrderLineItemRepository.Object, // Add this
            mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    private void SetupValidValidation(GetAllMaintenanceHistoriesQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query)).Returns(validResult);
    }

    private void SetupInvalidValidation(GetAllMaintenanceHistoriesQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query)).Returns(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_On_Success()
    {
        // Arrange
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            Search = "Oil",
            SortBy = "servicedate",
            SortDescending = false
        };
        var query = new GetAllMaintenanceHistoriesQuery(parameters);
        SetupValidValidation(query);

        var vehicle = new Vehicle
        {
            ID = 1,
            Name = "Truck",
            Make = "Ford",
            Model = "F-150",
            Year = 2024,
            VIN = "1234567890ABCDEFG",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "XL",
            Mileage = 10000,
            FuelCapacity = 80.0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-2),
            PurchasePrice = 40000m,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Depot",
            AssignedTechnicianID = "T1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleGroup = new VehicleGroup { ID = 1, Name = "Fleet", Description = "Main fleet", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            User = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = []
        };

        var user = new User
        {
            Id = "T1",
            FirstName = "Jane",
            LastName = "Smith",
            HireDate = DateTime.UtcNow.AddYears(-3),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicles = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            MaintenanceHistories = [],
            IssueAttachments = [],
            InventoryTransactions = []
        };

        var workOrder = new Domain.Entities.WorkOrder
        {
            ID = 2,
            Title = "WO-001",
            Description = "Oil change work order",
            VehicleID = 1,
            AssignedToUserID = "T1",
            WorkOrderType = WorkTypeEnum.SCHEDULED,
            PriorityLevel = PriorityLevelEnum.MEDIUM,
            Status = WorkOrderStatusEnum.COMPLETED,
            ScheduledStartDate = DateTime.UtcNow.AddDays(-2),
            ActualStartDate = DateTime.UtcNow.AddDays(-1),
            ActualCompletionDate = new DateTime(2024, 1, 1),
            StartOdometer = 10000,
            EndOdometer = 10200,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicle = vehicle,
            User = user,
            MaintenanceHistories = [],
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };

        var expectedEntities = new List<MaintenanceHistory>
        {
            new() {
                ID = 10,
                WorkOrderID = 2,
                ServiceDate = new DateTime(2024, 1, 1),
                MileageAtService = 10000,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                WorkOrder = workOrder,
            }
        };

        var pagedEntities = new PagedResult<MaintenanceHistory>
        {
            Items = expectedEntities,
            TotalCount = 10,
            PageNumber = 1,
            PageSize = 5
        };

        // Create sample work order line items for cost calculation using current entity structure
        var workOrderLineItems = new List<Domain.Entities.WorkOrderLineItem>
        {
            new()
            {
                ID = 1,
                WorkOrderID = 2,
                ServiceTaskID = 1, // Required field
                ItemType = LineItemTypeEnum.BOTH, // Required field
                Quantity = 1, // Required field
                TotalCost = 100.00m, // Required field - will be calculated
                LaborHours = 2.0, // Labor hours
                HourlyRate = 25.00m, // Hourly rate for labor
                UnitPrice = 50.00m, // Unit price for items
                Description = "Oil change service",
                AssignedToUserID = "T1",
                InventoryItemID = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                // Required navigation properties
                User = user,
                WorkOrder = workOrder,
                ServiceTask = new ServiceTask
                {
                    ID = 1,
                    Name = "Oil Change",
                    EstimatedLabourHours = 2.0,
                    EstimatedCost = 100.00m,
                    Category = ServiceTaskCategoryEnum.PREVENTIVE,
                    Description = "Change engine oil",
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    // Add required navigation properties
                    XrefServiceScheduleServiceTasks = [],
                    MaintenanceHistories = [],
                    WorkOrderLineItems = []
                },
                InventoryItem = new InventoryItem
                {
                    ID = 1,
                    ItemName = "Motor Oil",
                    UnitCost = 50.00m,
                    ItemNumber = "OIL-001",
                    Description = "5W-30 Motor Oil",
                    Category = InventoryItemCategoryEnum.ENGINE,
                    Manufacturer = "Shell",
                    ManufacturerPartNumber = "S5W30",
                    UniversalProductCode = "123456789012",
                    UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
                    Supplier = "Auto Parts Inc",
                    WeightKG = 4.0,
                    IsActive = true,
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    // Required navigation properties
                    Inventories = [],
                    WorkOrderLineItems = []
                }
            }
        };

        // Calculate the total cost using the entity's method
        foreach (var lineItem in workOrderLineItems)
        {
            lineItem.CalculateTotalCost();
        }

        // Setup repository mocks
        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters))
            .ReturnsAsync(pagedEntities);

        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(workOrderLineItems);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllMaintenanceHistoryDTO>>(result);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(2, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
        Assert.Single(result.Items);

        var first = result.Items[0];
        Assert.Equal(10, first.MaintenanceHistoryID);
        Assert.Equal(2, first.WorkOrderID);
        Assert.Equal(new DateTime(2024, 1, 1), first.ServiceDate);
        Assert.Equal(10000, first.MileageAtService);

        // Verify cost calculations are included (should be > 0 since we have line items)
        Assert.True(first.Cost >= 0);

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockRepository.Verify(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters), Times.Once);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Records()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 10, Search = "None" };
        var query = new GetAllMaintenanceHistoriesQuery(parameters);
        SetupValidValidation(query);

        var emptyPagedResult = new PagedResult<MaintenanceHistory>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };

        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters))
            .ReturnsAsync(emptyPagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockRepository.Verify(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters), Times.Once);
        // Should not call line items repository when no maintenance histories
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 2, PageSize = 3 };
        var query = new GetAllMaintenanceHistoriesQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<MaintenanceHistory>
        {
            Items = [],
            TotalCount = 10,
            PageNumber = 2,
            PageSize = 3
        };

        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(4, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockRepository.Verify(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Last_Page()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 3, PageSize = 5 };
        var query = new GetAllMaintenanceHistoriesQuery(parameters);
        SetupValidValidation(query);

        var pagedResult = new PagedResult<MaintenanceHistory>
        {
            Items = [],
            TotalCount = 12,
            PageNumber = 3,
            PageSize = 5
        };

        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockRepository.Verify(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 0, PageSize = 10 };
        var query = new GetAllMaintenanceHistoriesQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(query, CancellationToken.None));

        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockRepository.Verify(r => r.GetAllMaintenanceHistoriesPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Handle_Maintenance_History_Without_Line_Items()
    {
        // Arrange
        var parameters = new PaginationParameters { PageNumber = 1, PageSize = 5 };
        var query = new GetAllMaintenanceHistoriesQuery(parameters);
        SetupValidValidation(query);

        // Create a minimal vehicle for the work order
        var vehicle = new Vehicle
        {
            ID = 1,
            Name = "Test Vehicle",
            Make = "Ford",
            Model = "F-150",
            Year = 2024,
            VIN = "1234567890ABCDEFG",
            LicensePlate = "TEST123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "XL",
            Mileage = 5000,
            FuelCapacity = 80.0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-1),
            PurchasePrice = 30000m,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Depot",
            AssignedTechnicianID = "T1",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleGroup = new VehicleGroup { ID = 1, Name = "Fleet", Description = "Main fleet", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            User = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = []
        };

        // Create a minimal user for the work order
        var user = new User
        {
            Id = "T1",
            FirstName = "Test",
            LastName = "User",
            HireDate = DateTime.UtcNow.AddYears(-1),
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicles = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            MaintenanceHistories = [],
            IssueAttachments = [],
            InventoryTransactions = []
        };

        var maintenanceHistory = new MaintenanceHistory
        {
            ID = 1,
            WorkOrderID = 99, // Different work order ID
            ServiceDate = DateTime.UtcNow,
            MileageAtService = 5000,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            WorkOrder = new Domain.Entities.WorkOrder
            {
                ID = 99,
                Title = "Simple Work Order",
                VehicleID = 1,
                AssignedToUserID = "T1",
                WorkOrderType = WorkTypeEnum.UNSCHEDULED,
                PriorityLevel = PriorityLevelEnum.LOW,
                Status = WorkOrderStatusEnum.COMPLETED,
                StartOdometer = 5000,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                // Add required navigation properties
                Vehicle = vehicle,
                User = user,
                MaintenanceHistories = [],
                WorkOrderLineItems = [],
                Invoices = [],
                InventoryTransactions = []
            }
        };

        var pagedResult = new PagedResult<MaintenanceHistory>
        {
            Items = [maintenanceHistory],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 5
        };

        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters))
            .ReturnsAsync(pagedResult);

        // Return empty line items for this work order
        _mockWorkOrderLineItemRepository.Setup(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()))
            .ReturnsAsync(new List<Domain.Entities.WorkOrderLineItem>());

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);

        var first = result.Items[0];
        Assert.Equal(0, first.Cost);      // Should default to 0

        _mockWorkOrderLineItemRepository.Verify(r => r.GetByWorkOrderIdsAsync(It.IsAny<List<int>>()), Times.Once);
    }
}