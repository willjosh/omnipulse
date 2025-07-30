using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Issues.Command.UpdateIssue;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.Issues.CommandTest.UpdateIssue;

public class UpdateIssueCommandHandlerTest
{
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IAppLogger<UpdateIssueCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<UpdateIssueCommand>> _mockValidator;
    private readonly UpdateIssueCommandHandler _handler;
    private readonly IMapper _mapper;

    public UpdateIssueCommandHandlerTest()
    {
        _mockIssueRepository = new();
        _mockVehicleRepository = new();
        _mockUserRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<IssueMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new UpdateIssueCommandHandler(
            _mockIssueRepository.Object,
            _mockVehicleRepository.Object,
            _mockUserRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    private UpdateIssueCommand CreateValidCommand(
        int issueID = 1,
        int vehicleID = 123,
        string title = "Test Issue Title",
        string? description = "Test Issue Description",
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.CRITICAL,
        IssueCategoryEnum category = IssueCategoryEnum.BODY,
        IssueStatusEnum status = IssueStatusEnum.IN_PROGRESS,
        string reportedByUserID = "1234567890",
        DateTime? reportedDate = null,
        string? resolutionNotes = null,
        DateTime? resolvedDate = null,
        string? resolvedByUserID = null
    )
    {
        return new UpdateIssueCommand(issueID, vehicleID, title, description, priorityLevel, category, status, reportedByUserID, reportedDate, resolutionNotes, resolvedDate, resolvedByUserID);
    }

    private Issue CreateTestIssue(int issueID = 1, int vehicleID = 123, string reportedByUserID = "1234567890")
    {
        return new Issue
        {
            ID = issueID,
            VehicleID = vehicleID,
            ReportedByUserID = reportedByUserID,
            Title = "Test Issue Title",
            Description = "Test Issue Description",
            Category = IssueCategoryEnum.BODY,
            PriorityLevel = PriorityLevelEnum.CRITICAL,
            Status = IssueStatusEnum.IN_PROGRESS,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IssueNumber = 1001,
            IssueAttachments = [],
            IssueAssignments = [],
            Vehicle = null!,
            ReportedByUser = null!
        };
    }

    private Vehicle CreateTestVehicle(int vehicleId = 123)
    {
        return new Vehicle
        {
            ID = vehicleId,
            Name = "Test Vehicle",
            Make = "Toyota",
            Model = "Camry",
            Year = 2023,
            VIN = "1234567890",
            LicensePlate = "ABC123",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Base",
            Mileage = 0,
            EngineHours = 0,
            FuelCapacity = 50,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 25000,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Test Location",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleGroup = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = []
        };
    }

    private User CreateTestUser(string userId = "1234567890")
    {
        return new User
        {
            Id = userId,
            FirstName = "John",
            LastName = "Doe",
            HireDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            MaintenanceHistories = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            Vehicles = []
        };
    }

    private void SetupValidValidation(UpdateIssueCommand command)
    {
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
    }

    private void SetupInvalidValidation(UpdateIssueCommand command, string propertyName = "Title", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handle_Should_Return_IssueID_On_Success()
    {
        // Given
        var command = CreateValidCommand(resolutionNotes: "Resolved", resolvedDate: DateTime.UtcNow, resolvedByUserID: "user2");
        SetupValidValidation(command);
        var existingIssue = CreateTestIssue(command.IssueID, command.VehicleID, command.ReportedByUserID);
        _mockIssueRepository.Setup(r => r.GetByIdAsync(command.IssueID)).ReturnsAsync(existingIssue);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync(CreateTestVehicle(command.VehicleID));
        _mockUserRepository.Setup(r => r.GetByIdAsync(command.ReportedByUserID)).ReturnsAsync(CreateTestUser(command.ReportedByUserID));
        if (command.ResolvedByUserID != null)
        {
            _mockUserRepository.Setup(r => r.GetByIdAsync(command.ResolvedByUserID)).ReturnsAsync(CreateTestUser(command.ResolvedByUserID));
        }
        _mockIssueRepository.Setup(r => r.Update(existingIssue));
        _mockIssueRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingIssue.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.GetByIdAsync(command.IssueID), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(r => r.GetByIdAsync(command.ReportedByUserID), Times.Once);
        _mockIssueRepository.Verify(r => r.Update(existingIssue), Times.Once);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Invalid_Command_Is_Provided()
    {
        // Given
        var command = CreateValidCommand(resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
        SetupInvalidValidation(command, "Title", "Title is required");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockIssueRepository.Verify(r => r.Update(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Issue_Is_Not_Found()
    {
        // Given
        var command = CreateValidCommand(resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
        SetupValidValidation(command);
        _mockIssueRepository.Setup(r => r.GetByIdAsync(command.IssueID)).ReturnsAsync((Issue?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.GetByIdAsync(command.IssueID), Times.Once);
        _mockIssueRepository.Verify(r => r.Update(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Vehicle_Is_Not_Found()
    {
        // Given
        var command = CreateValidCommand(resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
        SetupValidValidation(command);
        var existingIssue = CreateTestIssue(command.IssueID, command.VehicleID, command.ReportedByUserID);
        _mockIssueRepository.Setup(r => r.GetByIdAsync(command.IssueID)).ReturnsAsync(existingIssue);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync((Vehicle?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.GetByIdAsync(command.IssueID), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockIssueRepository.Verify(r => r.Update(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_User_Is_Not_Found()
    {
        // Given
        var command = CreateValidCommand(resolutionNotes: null, resolvedDate: null, resolvedByUserID: null);
        SetupValidValidation(command);
        var existingIssue = CreateTestIssue(command.IssueID, command.VehicleID, command.ReportedByUserID);
        _mockIssueRepository.Setup(r => r.GetByIdAsync(command.IssueID)).ReturnsAsync(existingIssue);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync(CreateTestVehicle(command.VehicleID));
        _mockUserRepository.Setup(r => r.GetByIdAsync(command.ReportedByUserID)).ReturnsAsync((User?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.GetByIdAsync(command.IssueID), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(r => r.GetByIdAsync(command.ReportedByUserID), Times.Once);
        _mockIssueRepository.Verify(r => r.Update(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}