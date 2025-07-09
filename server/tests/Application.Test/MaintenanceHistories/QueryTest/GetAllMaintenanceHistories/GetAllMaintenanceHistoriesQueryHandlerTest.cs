using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.MaintenanceHistories.Query.GetAllMaintenanceHistories;
using Application.MappingProfiles;
using Application.Models;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

using Xunit;

namespace Application.Test.MaintenanceHistories.QueryTest.GetAllMaintenanceHistories;

public class GetAllMaintenanceHistoriesQueryHandlerTest
{
    private readonly Mock<IMaintenanceHistoryRepository> _mockRepository;
    private readonly GetAllMaintenanceHistoriesQueryHandler _handler;
    private readonly Mock<IAppLogger<GetAllMaintenanceHistoriesQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllMaintenanceHistoriesQuery>> _mockValidator;

    public GetAllMaintenanceHistoriesQueryHandlerTest()
    {
        _mockRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<MaintenanceHistoryMappingProfile>());
        var mapper = config.CreateMapper();

        _handler = new GetAllMaintenanceHistoriesQueryHandler(
            _mockRepository.Object,
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
            EngineHours = 500,
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
            VehicleServicePrograms = [],
            ServiceReminders = [],
            Issues = [],
            VehicleInspections = []
        };
        var workOrder = new Domain.Entities.WorkOrder
        {
            ID = 2,
            Title = "WO-001",
            Description = "Oil change work order",
            VehicleID = 1,
            ServiceReminderID = 1,
            AssignedToUserID = "T1",
            WorkOrderType = WorkTypeEnum.SCHEDULED,
            PriorityLevel = PriorityLevelEnum.MEDIUM,
            Status = WorkOrderStatusEnum.CREATED,
            EstimatedCost = 100,
            ActualCost = 100,
            EstimatedHours = 2,
            ActualHours = 2,
            ScheduledStartDate = DateTime.UtcNow.AddDays(-2),
            ActualStartDate = DateTime.UtcNow.AddDays(-1),
            StartOdometer = 10000,
            EndOdometer = 10200,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicle = null!,
            MaintenanceHistories = [],
            ServiceReminder = null!,
            User = null!,
            WorkOrderLineItems = [],
            Invoices = [],
            InventoryTransactions = []
        };
        var serviceTask = new ServiceTask
        {
            ID = 3,
            Name = "Oil Change",
            Description = "Change engine oil",
            Category = ServiceTaskCategoryEnum.PREVENTIVE,
            EstimatedLabourHours = 1.5,
            EstimatedCost = 80,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            MaintenanceHistories = [],
            WorkOrderLineItems = [],
            ServiceScheduleTasks = []
        };
        var user = new Domain.Entities.User
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
            VehicleInspections = [],
            MaintenanceHistories = [],
            IssueAttachments = []
        };

        var expectedEntities = new List<MaintenanceHistory>
        {
            new() {
                ID = 10,
                VehicleID = 1,
                WorkOrderID = 2,
                ServiceTaskID = 3,
                TechnicianID = "T1",
                ServiceDate = new DateTime(2024, 1, 1),
                MileageAtService = 10000,
                Description = "Changed oil",
                Cost = 100,
                LabourHours = 2,
                Notes = "N/A",
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Vehicle = vehicle,
                WorkOrder = workOrder,
                ServiceTask = serviceTask,
                User = user,
                InventoryTransactions = []
            }
        };
        var pagedEntities = new PagedResult<MaintenanceHistory>
        {
            Items = expectedEntities,
            TotalCount = 10,
            PageNumber = 1,
            PageSize = 5
        };
        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters)).ReturnsAsync(pagedEntities);

        var result = await _handler.Handle(query, CancellationToken.None);

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
        Assert.Equal(1, first.VehicleID);
        Assert.Equal("Truck", first.VehicleName);
        Assert.Equal(2, first.WorkOrderID);
        Assert.Equal("WO-001", first.WorkOrderNumber);
        Assert.Equal(3, first.ServiceTaskID);
        Assert.Equal("Oil Change", first.ServiceTaskName);
        Assert.Equal("T1", first.TechnicianID);
        Assert.Equal("Jane Smith", first.TechnicianName);
        Assert.Equal(new DateTime(2024, 1, 1), first.ServiceDate);
        Assert.Equal(10000, first.MileageAtService);
        Assert.Equal("Changed oil", first.Description);
        Assert.Equal(100, first.Cost);
        Assert.Equal(2, first.LabourHours);
        Assert.Equal("N/A", first.Notes);
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockRepository.Verify(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Records()
    {
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
        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters)).ReturnsAsync(emptyPagedResult);
        var result = await _handler.Handle(query, CancellationToken.None);
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
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
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
        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters)).ReturnsAsync(pagedResult);
        var result = await _handler.Handle(query, CancellationToken.None);
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
        _mockRepository.Setup(r => r.GetAllMaintenanceHistoriesPagedAsync(parameters)).ReturnsAsync(pagedResult);
        var result = await _handler.Handle(query, CancellationToken.None);
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
        var parameters = new PaginationParameters { PageNumber = 0, PageSize = 10 };
        var query = new GetAllMaintenanceHistoriesQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(query, CancellationToken.None));
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockRepository.Verify(r => r.GetAllMaintenanceHistoriesPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }
}