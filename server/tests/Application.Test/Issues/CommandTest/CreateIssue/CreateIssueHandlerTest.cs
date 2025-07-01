using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.Issues.Command.CreateIssue;
using Application.MappingProfiles;
using Domain.Entities.Enums;
using AutoMapper;
using FluentValidation;
using Moq;
using Domain.Entities;
using Application.Exceptions;

namespace Application.Test.Issues.CommandTest.CreateIssue;

public class CreateIssueHandlerTest
{
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly CreateIssueCommandHandler _createIssueCommandHandler;
    private readonly Mock<IAppLogger<CreateIssueCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<CreateIssueCommand>> _mockValidator;

    public CreateIssueHandlerTest()
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

        var mapper = config.CreateMapper();

        _createIssueCommandHandler = new CreateIssueCommandHandler(_mockIssueRepository.Object, _mockVehicleRepository.Object, _mockUserRepository.Object, mapper, _mockLogger.Object, _mockValidator.Object);
    }

    private CreateIssueCommand CreateValidCommand(
        int vehicleID = 123,
        string reportedByUserID = "1234567890",
        string title = "Test Issue Title",
        string? description = "Test Issue Description",
        IssueCategoryEnum category = IssueCategoryEnum.BODY,
        PriorityLevelEnum priorityLevel = PriorityLevelEnum.CRITICAL,
        IssueStatusEnum status = IssueStatusEnum.IN_PROGRESS
    )
    {
        return new CreateIssueCommand(vehicleID, reportedByUserID, title, description, category, priorityLevel, status);
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

    [Fact(Skip = "Skipping this test for now")]
    public async Task Handle_Should_Return_IssueID_On_Success()
    {
        // Given
        var command = CreateValidCommand();

        var expectedIssue = new Issue
        {
            ID = 12,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleID = command.VehicleID,
            IssueNumber = 1001, // Auto-generated in real scenario
            ReportedByUserID = command.ReportedByUserID,
            Title = command.Title,
            Description = command.Description,
            Category = command.Category,
            PriorityLevel = command.PriorityLevel,
            Status = command.Status,
            ResolvedDate = null,
            ResolvedBy = null,
            ResolutionNotes = null,
            IssueAttachments = [],
            Vehicle = null!, // Required but not used in test
            User = null! // Required but not used in test
        };

        _mockIssueRepository.Setup(repo => repo.AddAsync(expectedIssue)).ReturnsAsync(expectedIssue);
        _mockIssueRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateIssueCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        // When
        var result = await _createIssueCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedIssue.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(expectedIssue), Times.Once);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact(Skip = "Skipping this test for now")]
    public async Task Handle_Should_Throw_ValidationException_When_Invalid_Command_Is_Provided()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "Title", "Title is required");

        // When & Then
        await Assert.ThrowsAsync<ValidationException>(
            () => _createIssueCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "Skipping this test for now")]
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

    [Fact(Skip = "Skipping this test for now")]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_User_Is_Not_Found()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockUserRepository.Setup(repo => repo.GetByIdAsync(command.ReportedByUserID)).ReturnsAsync((User?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _createIssueCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.GetByIdAsync(command.ReportedByUserID), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(It.IsAny<Issue>()), Times.Never);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "Skipping this test for now")]
    public async Task Handle_Should_Create_Multiple_Issues_On_Same_Vehicle()
    {
        // Given
        var command1 = CreateValidCommand(vehicleID: 123, reportedByUserID: "1234567890", title: "First Issue");
        var command2 = CreateValidCommand(vehicleID: 123, reportedByUserID: "1234567890", title: "Second Issue");
        var command3 = CreateValidCommand(vehicleID: 123, reportedByUserID: "1234567890", title: "Third Issue");
        SetupValidValidation(command1);
        SetupValidValidation(command2);
        SetupValidValidation(command3);

        var expectedIssue1 = new Issue
        {
            ID = 12,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleID = command1.VehicleID,
            IssueNumber = 1001, // Auto-generated in real scenario
            ReportedByUserID = command1.ReportedByUserID,
            Title = command1.Title,
            Description = command1.Description,
            Category = command1.Category,
            PriorityLevel = command1.PriorityLevel,
            Status = command1.Status,
            ResolvedDate = null,
            ResolvedBy = null,
            ResolutionNotes = null,
            IssueAttachments = [],
            Vehicle = null!, // Required but not used in test
            User = null! // Required but not used in test
        };

        var expectedIssue2 = new Issue
        {
            ID = 13,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleID = command2.VehicleID,
            IssueNumber = 1002, // Auto-generated in real scenario
            ReportedByUserID = command2.ReportedByUserID,
            Title = command2.Title,
            Description = command2.Description,
            Category = command2.Category,
            PriorityLevel = command2.PriorityLevel,
            Status = command2.Status,
            ResolvedDate = null,
            ResolvedBy = null,
            ResolutionNotes = null,
            IssueAttachments = [],
            Vehicle = null!, // Required but not used in test
            User = null! // Required but not used in test
        };

        var expectedIssue3 = new Issue
        {
            ID = 14,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            VehicleID = command3.VehicleID,
            IssueNumber = 1003, // Auto-generated in real scenario
            ReportedByUserID = command3.ReportedByUserID,
            Title = command3.Title,
            Description = command3.Description,
            Category = command3.Category,
            PriorityLevel = command3.PriorityLevel,
            Status = command3.Status,
            ResolvedDate = null,
            ResolvedBy = null,
            ResolutionNotes = null,
            IssueAttachments = [],
            Vehicle = null!, // Required but not used in test
            User = null! // Required but not used in test
        };

        _mockIssueRepository.Setup(repo => repo.AddAsync(expectedIssue1)).ReturnsAsync(expectedIssue1);
        _mockIssueRepository.Setup(repo => repo.AddAsync(expectedIssue2)).ReturnsAsync(expectedIssue2);
        _mockIssueRepository.Setup(repo => repo.AddAsync(expectedIssue3)).ReturnsAsync(expectedIssue3);
        _mockIssueRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(3);

        // When
        var result1 = await _createIssueCommandHandler.Handle(command1, CancellationToken.None);
        var result2 = await _createIssueCommandHandler.Handle(command2, CancellationToken.None);
        var result3 = await _createIssueCommandHandler.Handle(command3, CancellationToken.None);

        // Then
        Assert.Equal(expectedIssue1.ID, result1);
        Assert.Equal(expectedIssue2.ID, result2);
        Assert.Equal(expectedIssue3.ID, result3);

        _mockValidator.Verify(v => v.ValidateAsync(command1, CancellationToken.None), Times.Once);
        _mockValidator.Verify(v => v.ValidateAsync(command2, CancellationToken.None), Times.Once);
        _mockValidator.Verify(v => v.ValidateAsync(command3, CancellationToken.None), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(expectedIssue1), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(expectedIssue2), Times.Once);
        _mockIssueRepository.Verify(r => r.AddAsync(expectedIssue3), Times.Once);
        _mockIssueRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
