using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inspections.Query.GetInspection;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.Inspections.QueryTest.GetInspection;

public class GetInspectionQueryHandlerTest
{
    private readonly GetInspectionQueryHandler _queryHandler;
    private readonly Mock<IInspectionRepository> _mockInspectionRepository = new();
    private readonly Mock<IAppLogger<GetInspectionQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);
    private static readonly string TechnicianGuid = "550e8400-e29b-41d4-a716-446655440000";

    public GetInspectionQueryHandlerTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InspectionMappingProfile>();
            cfg.AddProfile<VehicleMappingProfile>();
            cfg.AddProfile<UserMappingProfile>();
            cfg.AddProfile<InspectionFormMappingProfile>();
        });
        _mapper = config.CreateMapper();
        _queryHandler = new GetInspectionQueryHandler(
            _mockInspectionRepository.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static GetInspectionQuery CreateValidQuery(int inspectionID = 1) => new(inspectionID);

    [Fact]
    public async Task Handler_Should_Return_InspectionDTO_When_Found()
    {
        // Arrange
        var query = CreateValidQuery();
        var expectedInspection = CreateInspectionEntity();

        _mockInspectionRepository.Setup(r => r.GetInspectionWithDetailsAsync(query.InspectionID))
            .ReturnsAsync(expectedInspection);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedInspection.ID, result.ID);
        Assert.Equal(expectedInspection.InspectionFormID, result.InspectionFormID);
        Assert.Equal(expectedInspection.VehicleID, result.VehicleID);
        Assert.Equal(expectedInspection.TechnicianID, result.TechnicianID);
        Assert.Equal(expectedInspection.VehicleCondition, result.VehicleCondition);
        Assert.Equal(expectedInspection.OdometerReading, result.OdometerReading);
        Assert.Equal(expectedInspection.Notes, result.Notes);
        Assert.Equal(expectedInspection.SnapshotFormTitle, result.SnapshotFormTitle);
        Assert.Equal(expectedInspection.SnapshotFormDescription, result.SnapshotFormDescription);
        Assert.Equal(expectedInspection.CreatedAt, result.CreatedAt);
        Assert.Equal(expectedInspection.UpdatedAt, result.UpdatedAt);

        // Verify related entities
        Assert.NotNull(result.Vehicle);
        Assert.Equal("Test Vehicle", result.Vehicle.Name);
        Assert.Equal("ABC123", result.Vehicle.LicensePlate);

        Assert.NotNull(result.Technician);
        Assert.Equal("John", result.Technician.FirstName);
        Assert.Equal("Doe", result.Technician.LastName);
        Assert.Equal("john.doe@test.com", result.Technician.Email);

        Assert.NotNull(result.InspectionForm);
        Assert.Equal("Vehicle Safety Inspection", result.InspectionForm.Title);
        Assert.True(result.InspectionForm.IsActive);

        // Verify inspection items
        Assert.Equal(2, result.InspectionItems.Count);
        var firstItem = result.InspectionItems.First(i => i.InspectionFormItemID == 1);
        Assert.True(firstItem.Passed);
        Assert.Equal("Brakes", firstItem.SnapshotItemLabel);
        Assert.Equal("Check brake system", firstItem.SnapshotItemDescription);
        Assert.Equal("Inspect brake pads and fluid", firstItem.SnapshotItemInstructions);
        Assert.True(firstItem.SnapshotIsRequired);
        Assert.Equal(InspectionFormItemTypeEnum.PassFail, firstItem.SnapshotInspectionFormItemType);
        Assert.Equal("All good", firstItem.Comment);

        var secondItem = result.InspectionItems.First(i => i.InspectionFormItemID == 2);
        Assert.False(secondItem.Passed);
        Assert.Equal("Tires", secondItem.SnapshotItemLabel);
        Assert.Equal("Check tire condition", secondItem.SnapshotItemDescription);
        Assert.Equal("Inspect tread depth and pressure", secondItem.SnapshotItemInstructions);
        Assert.True(secondItem.SnapshotIsRequired);
        Assert.Equal(InspectionFormItemTypeEnum.PassFail, secondItem.SnapshotInspectionFormItemType);
        Assert.Equal("Needs replacement", secondItem.Comment);

        _mockInspectionRepository.Verify(r => r.GetInspectionWithDetailsAsync(query.InspectionID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Not_Found()
    {
        // Arrange
        var query = CreateValidQuery();
        _mockInspectionRepository.Setup(r => r.GetInspectionWithDetailsAsync(query.InspectionID))
            .ReturnsAsync((Inspection?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _queryHandler.Handle(query, CancellationToken.None));

        Assert.Equal("Domain.Entities.Inspection", exception.EntityName);
        Assert.Equal(nameof(Inspection.ID), exception.PropertyName);
        Assert.Equal(query.InspectionID.ToString(), exception.PropertyValue);

        _mockInspectionRepository.Verify(r => r.GetInspectionWithDetailsAsync(query.InspectionID), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Handler_Should_Throw_BadRequestException_On_Invalid_InspectionID(int invalidId)
    {
        // Arrange
        var query = new GetInspectionQuery(invalidId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        Assert.Contains("Invalid inspection ID", exception.Message);
        Assert.Contains(invalidId.ToString(), exception.Message);
        _mockInspectionRepository.Verify(r => r.GetInspectionWithDetailsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Handle_Inspection_With_No_Items()
    {
        // Arrange
        var query = new GetInspectionQuery(1);
        var inspectionWithNoItems = CreateInspectionEntity();
        inspectionWithNoItems.InspectionPassFailItems = [];

        _mockInspectionRepository.Setup(r => r.GetInspectionWithDetailsAsync(query.InspectionID))
            .ReturnsAsync(inspectionWithNoItems);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.InspectionItems);
        _mockInspectionRepository.Verify(r => r.GetInspectionWithDetailsAsync(query.InspectionID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Inspection_With_Null_Optional_Fields()
    {
        // Arrange
        var query = new GetInspectionQuery(1);
        var inspection = CreateInspectionEntity();
        inspection.OdometerReading = null;
        inspection.Notes = null;
        inspection.SnapshotFormDescription = null;
        inspection.Vehicle.LicensePlate = null!;
        inspection.Vehicle.VIN = null!;

        _mockInspectionRepository.Setup(r => r.GetInspectionWithDetailsAsync(query.InspectionID))
            .ReturnsAsync(inspection);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.OdometerReading);
        Assert.Null(result.Notes);
        Assert.Null(result.SnapshotFormDescription);
        Assert.Null(result.Vehicle!.LicensePlate);
        Assert.Null(result.Vehicle.VIN);
        _mockInspectionRepository.Verify(r => r.GetInspectionWithDetailsAsync(query.InspectionID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Information_Messages()
    {
        // Arrange
        var query = new GetInspectionQuery(1);
        var expectedInspection = CreateInspectionEntity();

        _mockInspectionRepository.Setup(r => r.GetInspectionWithDetailsAsync(query.InspectionID))
            .ReturnsAsync(expectedInspection);

        // Act
        await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Handling GetInspectionQuery(1)"))),
            Times.Once);
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Returning InspectionDTO for InspectionID: 1"))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Error_When_Inspection_Not_Found()
    {
        // Arrange
        var nonExistentId = 999;
        var query = new GetInspectionQuery(nonExistentId);

        _mockInspectionRepository.Setup(r => r.GetInspectionWithDetailsAsync(nonExistentId))
            .ReturnsAsync((Inspection?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockLogger.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("Inspection with ID 999 not found."))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Error_When_Invalid_ID_Provided()
    {
        // Arrange
        var invalidId = -1;
        var query = new GetInspectionQuery(invalidId);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockLogger.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("Invalid inspection ID: -1"))),
            Times.Once);
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var vehicle = new Vehicle
        {
            ID = 2,
            Name = "Test Vehicle",
            Make = "Toyota",
            Model = "Camry",
            Year = 2020,
            VIN = "VIN123456789",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            VehicleGroup = vehicleGroup,
            Trim = "LE",
            Mileage = 50000,
            EngineHours = 2500,
            FuelCapacity = 60.0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-3),
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
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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
            ID = 1,
            Title = "Vehicle Safety Inspection",
            Description = "Comprehensive vehicle safety check",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Inspections = [],
            InspectionFormItems = []
        };

        var formItem1 = new InspectionFormItem
        {
            ID = 1,
            InspectionFormID = 1,
            ItemLabel = "Brakes",
            ItemDescription = "Check brake system",
            ItemInstructions = "Inspect brake pads and fluid",
            IsRequired = true,
            InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            IsActive = true,
            InspectionForm = inspectionForm,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var formItem2 = new InspectionFormItem
        {
            ID = 2,
            InspectionFormID = 1,
            ItemLabel = "Tires",
            ItemDescription = "Check tire condition",
            ItemInstructions = "Inspect tread depth and pressure",
            IsRequired = true,
            InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            IsActive = true,
            InspectionForm = inspectionForm,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
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
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            InspectionPassFailItems = []
        };

        var passFailItem1 = new InspectionPassFailItem
        {
            InspectionID = 1,
            InspectionFormItemID = 1,
            Passed = true,
            Comment = "All good",
            SnapshotItemLabel = "Brakes",
            SnapshotItemDescription = "Check brake system",
            SnapshotItemInstructions = "Inspect brake pads and fluid",
            SnapshotIsRequired = true,
            SnapshotInspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            Inspection = inspection,
            InspectionFormItem = formItem1
        };

        var passFailItem2 = new InspectionPassFailItem
        {
            InspectionID = 1,
            InspectionFormItemID = 2,
            Passed = false,
            Comment = "Needs replacement",
            SnapshotItemLabel = "Tires",
            SnapshotItemDescription = "Check tire condition",
            SnapshotItemInstructions = "Inspect tread depth and pressure",
            SnapshotIsRequired = true,
            SnapshotInspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            Inspection = inspection,
            InspectionFormItem = formItem2
        };

        inspection.InspectionPassFailItems.Add(passFailItem1);
        inspection.InspectionPassFailItems.Add(passFailItem2);

        return inspection;
    }
}