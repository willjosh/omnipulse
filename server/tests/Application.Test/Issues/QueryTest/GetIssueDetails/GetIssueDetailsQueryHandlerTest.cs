using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Issues.Query.GetIssueDetails;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.Issues.QueryTest.GetIssueDetails;

public class GetIssueDetailsQueryHandlerTest
{
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly GetIssueDetailsQueryHandler _getIssueDetailsQueryHandler;
    private readonly Mock<IAppLogger<GetIssueDetailsQueryHandler>> _mockLogger;

    public GetIssueDetailsQueryHandlerTest()
    {
        _mockIssueRepository = new();
        _mockLogger = new();
        var config = new MapperConfiguration(cfg => cfg.AddProfile<IssueMappingProfile>());
        var mapper = config.CreateMapper();
        _getIssueDetailsQueryHandler = new GetIssueDetailsQueryHandler(
            _mockIssueRepository.Object,
            mapper,
            _mockLogger.Object
        );
    }

    [Fact]
    public async Task Handler_Should_Return_GetIssueDetailsDTO_On_Success()
    {
        // Given
        var query = new GetIssueDetailsQuery(1);
        var expectedUser = new User
        {
            Id = "USER123",
            FirstName = "Alice",
            LastName = "Smith",
            HireDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            MaintenanceHistories = new List<MaintenanceHistory>(),
            IssueAttachments = new List<IssueAttachment>(),
            VehicleAssignments = new List<VehicleAssignment>(),
            VehicleDocuments = new List<VehicleDocument>(),
            VehicleInspections = new List<VehicleInspection>(),
            Vehicles = new List<Vehicle>(),
            InventoryTransactions = new List<InventoryTransaction>()
        };
        var expectedVehicle = new Vehicle
        {
            ID = 2,
            Name = "Test Vehicle",
            Make = "TestMake",
            Model = "TestModel",
            Year = 2020,
            VIN = "VIN123456789",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Base",
            Mileage = 10000,
            EngineHours = 500,
            FuelCapacity = 40.0,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow.AddYears(-2),
            PurchasePrice = 20000.0m,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "TestLocation",
            VehicleGroup = new VehicleGroup { ID = 1, Name = "Group1", Description = "", IsActive = true, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            VehicleImages = new List<VehicleImage>(),
            VehicleAssignments = new List<VehicleAssignment>(),
            VehicleDocuments = new List<VehicleDocument>(),
            XrefServiceProgramVehicles = new List<XrefServiceProgramVehicle>(),
            ServiceReminders = new List<ServiceReminder>(),
            Issues = new List<Issue>(),
            VehicleInspections = new List<VehicleInspection>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        var expectedIssue = new Issue
        {
            ID = 1,
            IssueNumber = 1001,
            Title = "Engine Noise",
            Description = "Strange noise from engine",
            Status = IssueStatusEnum.OPEN,
            PriorityLevel = PriorityLevelEnum.HIGH,
            Category = IssueCategoryEnum.ENGINE,
            VehicleID = 2,
            Vehicle = expectedVehicle,
            ReportedByUserID = "USER123",
            ReportedByUser = expectedUser,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IssueAttachments = new List<IssueAttachment>(),
            IssueAssignments = new List<IssueAssignment>()
        };
        _mockIssueRepository.Setup(r => r.GetIssueWithDetailsAsync(query.IssueID))
            .ReturnsAsync(expectedIssue);
        // When
        var result = await _getIssueDetailsQueryHandler.Handle(query, CancellationToken.None);
        // Then
        Assert.NotNull(result);
        Assert.IsType<GetIssueDetailsDTO>(result);
        Assert.Equal(1, result.ID);
        Assert.Equal(1001, result.IssueNumber);
        Assert.Equal("Engine Noise", result.Title);
        Assert.Equal("Strange noise from engine", result.Description);
        Assert.Equal(IssueStatusEnum.OPEN, result.Status);
        Assert.Equal(PriorityLevelEnum.HIGH, result.PriorityLevel);
        Assert.Equal(IssueCategoryEnum.ENGINE, result.Category);
        Assert.Equal(2, result.VehicleID);
        Assert.Equal("Test Vehicle", result.VehicleName);
        Assert.Equal("USER123", result.ReportedByUserID);
        Assert.Equal("Alice Smith", result.ReportedByUserName);
        _mockIssueRepository.Verify(r => r.GetIssueWithDetailsAsync(query.IssueID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_NonExistent_IssueID()
    {
        // Given
        var nonExistentIssueId = 999;
        var query = new GetIssueDetailsQuery(nonExistentIssueId);
        _mockIssueRepository.Setup(r => r.GetIssueWithDetailsAsync(nonExistentIssueId))
            .ReturnsAsync((Issue?)null);
        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _getIssueDetailsQueryHandler.Handle(query, CancellationToken.None)
        );
        _mockIssueRepository.Verify(r => r.GetIssueWithDetailsAsync(nonExistentIssueId), Times.Once);
    }
}