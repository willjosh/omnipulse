using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inspections.Query.GetAllInspection;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.Inspections.QueryTest.GetAllInspection;

public class GetAllInspectionQueryHandlerTest
{
    private readonly GetAllInspectionQueryHandler _queryHandler;
    private readonly Mock<IInspectionRepository> _mockInspectionRepository = new();
    private readonly Mock<IValidator<GetAllInspectionQuery>> _mockValidator = new();
    private readonly Mock<IAppLogger<GetAllInspectionQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);
    private static readonly string TechnicianGuid = "550e8400-e29b-41d4-a716-446655440000";

    public GetAllInspectionQueryHandlerTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InspectionMappingProfile>();
            cfg.AddProfile<VehicleMappingProfile>();
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<InspectionFormMappingProfile>();
        });
        _mapper = config.CreateMapper();
        _queryHandler = new GetAllInspectionQueryHandler(
            _mockInspectionRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static GetAllInspectionQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            SortDescending = sortDescending
        };
        return new GetAllInspectionQuery(parameters);
    }

    private void SetupValidValidation(GetAllInspectionQuery query)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(GetAllInspectionQuery query, string propertyName = "Parameters", string errorMessage = "Invalid Parameters")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_When_Successful()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inspections = CreateInspectionList(3);
        var pagedResult = new PagedResult<Inspection>
        {
            Items = inspections,
            TotalCount = inspections.Count,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionRepository.Setup(r => r.GetAllInspectionsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        // Verify first inspection mapping
        var firstInspection = inspections.First();
        var firstDto = result.Items.First();
        Assert.Equal(firstInspection.ID, firstDto.ID);
        Assert.Equal(firstInspection.InspectionFormID, firstDto.InspectionFormID);
        Assert.Equal(firstInspection.VehicleID, firstDto.VehicleID);
        Assert.Equal(firstInspection.TechnicianID, firstDto.TechnicianID);
        Assert.Equal(firstInspection.VehicleCondition, firstDto.VehicleCondition);
        Assert.Equal(firstInspection.SnapshotFormTitle, firstDto.SnapshotFormTitle);

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionRepository.Verify(r => r.GetAllInspectionsPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Items()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var pagedResult = new PagedResult<Inspection>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionRepository.Setup(r => r.GetAllInspectionsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: -1); // Invalid page number
        SetupInvalidValidation(query, "PageNumber", "Page number must be greater than 0");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _queryHandler.Handle(query, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionRepository.Verify(r => r.GetAllInspectionsPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(3, 25)]
    public async Task Handler_Should_Handle_Different_Pagination_Parameters(int pageNumber, int pageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: pageNumber, pageSize: pageSize);
        SetupValidValidation(query);
        var inspections = CreateInspectionList(2);
        var pagedResult = new PagedResult<Inspection>
        {
            Items = inspections,
            TotalCount = 50, // Simulate larger dataset
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        _mockInspectionRepository.Setup(r => r.GetAllInspectionsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(50, result.TotalCount);
    }

    [Fact]
    public async Task Handler_Should_Handle_Search_Parameters()
    {
        // Arrange
        var query = CreateValidQuery(search: "safety");
        SetupValidValidation(query);
        var inspections = CreateInspectionList(1);
        var pagedResult = new PagedResult<Inspection>
        {
            Items = inspections,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionRepository.Setup(r => r.GetAllInspectionsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        _mockInspectionRepository.Verify(r => r.GetAllInspectionsPagedAsync(
            It.Is<PaginationParameters>(p => p.Search == "safety")), Times.Once);
    }

    [Theory]
    [InlineData("id", false)]
    [InlineData("id", true)]
    [InlineData("createdat", false)]
    [InlineData("updatedat", true)]
    public async Task Handler_Should_Handle_Sorting_Parameters(string sortBy, bool sortDescending)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: sortBy, sortDescending: sortDescending);
        SetupValidValidation(query);
        var inspections = CreateInspectionList(2);
        var pagedResult = new PagedResult<Inspection>
        {
            Items = inspections,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionRepository.Setup(r => r.GetAllInspectionsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        _mockInspectionRepository.Verify(r => r.GetAllInspectionsPagedAsync(
            It.Is<PaginationParameters>(p => p.SortBy == sortBy && p.SortDescending == sortDescending)), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Collections_Gracefully()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inspection = CreateInspectionEntity(
            id: 1,
            inspectionFormID: 1,
            vehicleID: 1,
            technicianID: TechnicianGuid,
            vehicleCondition: VehicleConditionEnum.Excellent,
            odometerReading: 50000,
            notes: "Test inspection",
            snapshotFormTitle: "Test Form",
            snapshotFormDescription: "Test Description");

        // Set navigation collections to null
        inspection.InspectionPassFailItems = null!;

        var pagedResult = new PagedResult<Inspection>
        {
            Items = [inspection],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionRepository.Setup(r => r.GetAllInspectionsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        var dto = result.Items.First();
        Assert.Equal(0, dto.InspectionItemsCount);
        Assert.Equal(0, dto.PassedItemsCount);
        Assert.Equal(0, dto.FailedItemsCount);
    }

    [Fact]
    public async Task Handler_Should_Map_All_Properties_Correctly()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inspection = CreateInspectionEntity(
            id: 42,
            inspectionFormID: 2,
            vehicleID: 3,
            technicianID: TechnicianGuid,
            vehicleCondition: VehicleConditionEnum.HasIssuesButSafeToOperate,
            odometerReading: 75000.5,
            notes: "Custom inspection notes",
            snapshotFormTitle: "Custom Safety Inspection",
            snapshotFormDescription: "Custom description for testing");

        var pagedResult = new PagedResult<Inspection>
        {
            Items = [inspection],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInspectionRepository.Setup(r => r.GetAllInspectionsPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var dto = result.Items.First();
        Assert.Equal(42, dto.ID);
        Assert.Equal(2, dto.InspectionFormID);
        Assert.Equal(3, dto.VehicleID);
        Assert.Equal(TechnicianGuid, dto.TechnicianID);
        Assert.Equal(VehicleConditionEnum.HasIssuesButSafeToOperate, dto.VehicleCondition);
        Assert.Equal(75000.5, dto.OdometerReading);
        Assert.Equal("Custom inspection notes", dto.Notes);
        Assert.Equal("Custom Safety Inspection", dto.SnapshotFormTitle);
        Assert.Equal("Custom description for testing", dto.SnapshotFormDescription);

        // Verify related entity mapping
        Assert.Equal($"{inspection.User.FirstName} {inspection.User.LastName}", dto.TechnicianName);
        Assert.Equal(inspection.Vehicle.Name, dto.VehicleName);

        // Verify inspection item counts
        Assert.Equal(inspection.InspectionPassFailItems.Count, dto.InspectionItemsCount);
        Assert.Equal(inspection.InspectionPassFailItems.Count(i => i.Passed), dto.PassedItemsCount);
        Assert.Equal(inspection.InspectionPassFailItems.Count(i => !i.Passed), dto.FailedItemsCount);
    }

    private static List<Inspection> CreateInspectionList(int count = 5)
    {
        var inspections = new List<Inspection>();

        for (int i = 1; i <= count; i++)
        {
            inspections.Add(CreateInspectionEntity(
                id: i,
                inspectionFormID: i % 2 == 0 ? 1 : 2,
                vehicleID: i,
                technicianID: TechnicianGuid,
                vehicleCondition: i % 2 == 0 ? VehicleConditionEnum.Excellent : VehicleConditionEnum.NotSafeToOperate,
                odometerReading: 50000 + (i * 1000),
                notes: $"Test inspection {i}",
                snapshotFormTitle: i % 2 == 0 ? "Vehicle Safety Inspection" : "Maintenance Check",
                snapshotFormDescription: $"Inspection form {i} description"
            ));
        }

        return inspections;
    }

    private static Inspection CreateInspectionEntity(
        int id = 1,
        int inspectionFormID = 1,
        int vehicleID = 2,
        string? technicianID = null,
        VehicleConditionEnum vehicleCondition = VehicleConditionEnum.Excellent,
        double? odometerReading = 50000.5,
        string? notes = "Test inspection notes",
        string snapshotFormTitle = "Vehicle Safety Inspection",
        string? snapshotFormDescription = "Comprehensive vehicle safety check")
    {
        technicianID ??= TechnicianGuid;

        var vehicleGroup = new VehicleGroup
        {
            ID = 1,
            Name = "Fleet A",
            Description = "Main fleet",
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate
        };

        var vehicle = new Vehicle
        {
            ID = vehicleID,
            Name = $"Test Vehicle {vehicleID}",
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = $"VIN{vehicleID:D9}",
            LicensePlate = $"ABC{vehicleID:D3}",
            LicensePlateExpirationDate = FixedDate.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            VehicleGroup = vehicleGroup,
            Trim = "LE",
            Mileage = 50000,
            EngineHours = 2500,
            FuelCapacity = 60.0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = FixedDate.AddYears(-3),
            PurchasePrice = 25000.0m,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Main Depot",
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = [],
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate
        };

        var technician = new User
        {
            Id = technicianID,
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@test.com",
            HireDate = FixedDate.AddYears(-2),
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            MaintenanceHistories = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            Vehicles = [],
            InventoryTransactions = []
        };

        var inspectionForm = new InspectionForm
        {
            ID = inspectionFormID,
            Title = snapshotFormTitle,
            Description = snapshotFormDescription,
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };

        var inspection = new Inspection
        {
            ID = id,
            InspectionFormID = inspectionFormID,
            VehicleID = vehicleID,
            TechnicianID = technicianID,
            InspectionStartTime = FixedDate.AddHours(-2),
            InspectionEndTime = FixedDate.AddHours(-1),
            OdometerReading = odometerReading,
            VehicleCondition = vehicleCondition,
            Notes = notes,
            SnapshotFormTitle = snapshotFormTitle,
            SnapshotFormDescription = snapshotFormDescription,
            InspectionForm = inspectionForm,
            Vehicle = vehicle,
            User = technician,
            CreatedAt = FixedDate.AddDays(-id), // Different dates for sorting tests
            UpdatedAt = FixedDate.AddDays(-id),
            InspectionPassFailItems = []
        };

        // Add some inspection items for count testing
        var passFailItem1 = new InspectionPassFailItem
        {
            InspectionID = id,
            InspectionFormItemID = 1,
            Passed = true,
            Comment = "Good",
            SnapshotItemLabel = "Brakes",
            SnapshotItemDescription = "Check brake system",
            SnapshotItemInstructions = "Inspect brake pads and fluid",
            SnapshotIsRequired = true,
            SnapshotInspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            Inspection = inspection,
            InspectionFormItem = new InspectionFormItem
            {
                ID = 1,
                InspectionFormID = inspectionFormID,
                ItemLabel = "Brakes",
                ItemDescription = "Check brake system",
                ItemInstructions = "Inspect brake pads and fluid",
                IsRequired = true,
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                IsActive = true,
                InspectionForm = inspectionForm,
                CreatedAt = FixedDate,
                UpdatedAt = FixedDate
            }
        };

        var passFailItem2 = new InspectionPassFailItem
        {
            InspectionID = id,
            InspectionFormItemID = 2,
            Passed = id % 2 == 0, // Alternate pass/fail for testing
            Comment = id % 2 == 0 ? "Good" : "Needs attention",
            SnapshotItemLabel = "Tires",
            SnapshotItemDescription = "Check tire condition",
            SnapshotItemInstructions = "Inspect tread depth and pressure",
            SnapshotIsRequired = true,
            SnapshotInspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            Inspection = inspection,
            InspectionFormItem = new InspectionFormItem
            {
                ID = 2,
                InspectionFormID = inspectionFormID,
                ItemLabel = "Tires",
                ItemDescription = "Check tire condition",
                ItemInstructions = "Inspect tread depth and pressure",
                IsRequired = true,
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                IsActive = true,
                InspectionForm = inspectionForm,
                CreatedAt = FixedDate,
                UpdatedAt = FixedDate
            }
        };

        inspection.InspectionPassFailItems.Add(passFailItem1);
        inspection.InspectionPassFailItems.Add(passFailItem2);

        return inspection;
    }
}