using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Issues.Command.CreateIssue;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.Issues.CommandTest.CreateIssue;

public class CreateIssueCommandHandlerTest
{
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateIssueCommandHandler _createIssueCommandHandler;
    private readonly Mock<IAppLogger<CreateIssueCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<CreateIssueCommand>> _mockValidator;

    public CreateIssueCommandHandlerTest()
    {
        _mockIssueRepository = new();
        _mockVehicleRepository = new();
        _mockUserRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<IssueMappingProfile>());

        var mapper = config.CreateMapper();

        _createIssueCommandHandler = new CreateIssueCommandHandler(_mockIssueRepository.Object, _mockVehicleRepository.Object, _mockUserRepository.Object, mapper, _mockLogger.Object, _mockValidator.Object);
    }

    private CreateIssueCommand CreateValidCommand(
        int vehicleID = 123,
        string title = "Test Issue Title",
        string? description = "Test Issue Description",
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.CRITICAL,
        IssueCategoryEnum category = IssueCategoryEnum.BODY,
        IssueStatusEnum status = IssueStatusEnum.IN_PROGRESS,
        string reportedByUserID = "1234567890",
        DateTime? reportedDate = null
    )
    {
        return new CreateIssueCommand(vehicleID, title, description, priorityLevel, category, status, reportedByUserID, reportedDate);
    }

    private void SetupValidValidation(CreateIssueCommand command)
    {
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());
    }

    private void SetupInvalidValidation(CreateIssueCommand command, string propertyName = "Title", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
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
            VehicleType = Domain.Entities.Enums.VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Base",
            Mileage = 0,
            FuelCapacity = 50,
            FuelType = Domain.Entities.Enums.FuelTypeEnum.PETROL,
            PurchaseDate = DateTime.UtcNow,
            PurchasePrice = 25000,
            Status = Domain.Entities.Enums.VehicleStatusEnum.ACTIVE,
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
            Vehicles = [],
            InventoryTransactions = []
        };
    }

    [Fact]
    public async Task Handle_Should_Return_IssueID_On_Success()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);


        var expectedIssue = new Issue
        {
            ID = 12,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleID = command.VehicleID,
            ReportedByUserID = command.ReportedByUserID,
            ReportedDate = new(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            Title = command.Title,
            Description = command.Description,
            Category = command.Category,
            PriorityLevel = command.PriorityLevel,
            Status = command.Status,
            ResolvedDate = null,
            ResolvedByUserID = null,
            ResolutionNotes = null,
            IssueAttachments = [],
            IssueAssignments = [],
            Vehicle = null!, // Required but not used in test
            ReportedByUser = null! // Required but not used in test
        };

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync(CreateTestVehicle(command.VehicleID));
        _mockUserRepository.Setup(r => r.GetByIdAsync(command.ReportedByUserID)).ReturnsAsync(CreateTestUser(command.ReportedByUserID));
        _mockIssueRepository.Setup(r => r.AddAsync(It.IsAny<Issue>())).ReturnsAsync(expectedIssue);
        _mockIssueRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createIssueCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedIssue.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(r => r.GetByIdAsync(command.ReportedByUserID), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(It.IsAny<Issue>()), Times.Once);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Invalid_Command_Is_Provided()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "Title", "Title is required");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createIssueCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Vehicle_Is_Not_Found()
    {
        // Given
        var command = CreateValidCommand();

        SetupValidValidation(command);

        _mockVehicleRepository.Setup(repo => repo.GetByIdAsync(command.VehicleID)).ReturnsAsync((Vehicle?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _createIssueCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_User_Is_Not_Found()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        // Setup valid vehicle but invalid user
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID)).ReturnsAsync(CreateTestVehicle(command.VehicleID));
        _mockUserRepository.Setup(r => r.GetByIdAsync(command.ReportedByUserID)).ReturnsAsync((User?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _createIssueCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(r => r.GetByIdAsync(command.ReportedByUserID), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }


}