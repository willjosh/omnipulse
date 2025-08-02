using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionForms.Command.UpdateInspectionForm;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionForms.CommandTest.UpdateInspectionForm;

public class UpdateInspectionFormCommandHandlerTest
{
    private readonly UpdateInspectionFormCommandHandler _commandHandler;
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository = new();
    private readonly Mock<IValidator<UpdateInspectionFormCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<UpdateInspectionFormCommandHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const int TitleMaxLength = 200;
    private const int DescriptionMaxLength = 1000;

    public UpdateInspectionFormCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionFormMappingProfile>());
        _mapper = config.CreateMapper();
        _commandHandler = new UpdateInspectionFormCommandHandler(
            _mockInspectionFormRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static UpdateInspectionFormCommand CreateValidCommand(
        int inspectionFormID = 1,
        string title = $"Test {nameof(InspectionForm)} Title",
        string? description = $"Test {nameof(InspectionForm)} Description",
        bool isActive = true) => new(inspectionFormID, title, description, isActive);

    private void SetupValidValidation(UpdateInspectionFormCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateInspectionFormCommand command, string propertyName = nameof(UpdateInspectionFormCommand.Title), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ID_On_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var existingEntity = new InspectionForm
        {
            ID = command.InspectionFormID,
            Title = "Old Title",
            Description = "Old Description",
            IsActive = false,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID)).ReturnsAsync(existingEntity);
        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title, command.InspectionFormID)).ReturnsAsync(true);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.Update(existingEntity), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID)).ReturnsAsync((InspectionForm?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_When_Title_Exists()
    {
        // Arrange
        var command = CreateValidCommand(title: "Duplicate Title");
        SetupValidValidation(command);
        var existingEntity = new InspectionForm
        {
            ID = command.InspectionFormID,
            Title = "Old Title",
            Description = "Old Description",
            IsActive = false,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID)).ReturnsAsync(existingEntity);
        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title, command.InspectionFormID)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand(title: "");
        SetupInvalidValidation(command);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Allow_Optional_Description()
    {
        // Arrange
        var command = CreateValidCommand(description: null);
        SetupValidValidation(command);
        var existingEntity = new InspectionForm
        {
            ID = command.InspectionFormID,
            Title = "Old Title",
            Description = "Old Description",
            IsActive = false,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID)).ReturnsAsync(existingEntity);
        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title, command.InspectionFormID)).ReturnsAsync(true);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.Update(existingEntity), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Not_Check_Uniqueness_When_Title_Unchanged()
    {
        // Arrange
        var command = CreateValidCommand(title: "Same Title");
        SetupValidValidation(command);
        var existingEntity = new InspectionForm
        {
            ID = command.InspectionFormID,
            Title = "Same Title", // Same as command title
            Description = "Old Description",
            IsActive = false,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID)).ReturnsAsync(existingEntity);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingEntity.ID, result);
        _mockInspectionFormRepository.Verify(r => r.IsTitleUniqueAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Update_All_Properties_Correctly()
    {
        // Arrange
        var command = CreateValidCommand(
            inspectionFormID: 1,
            title: "Updated Title",
            description: "Updated Description",
            isActive: false);
        SetupValidValidation(command);
        var existingEntity = new InspectionForm
        {
            ID = command.InspectionFormID,
            Title = "Old Title",
            Description = "Old Description",
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID)).ReturnsAsync(existingEntity);
        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title, command.InspectionFormID)).ReturnsAsync(true);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingEntity.ID, result);
        Assert.Equal(command.Title, existingEntity.Title);
        Assert.Equal(command.Description, existingEntity.Description);
        Assert.Equal(command.IsActive, existingEntity.IsActive);
        // Verify that ID and timestamps are not changed by mapping
        Assert.Equal(1, existingEntity.ID);
        Assert.Equal(FixedDate, existingEntity.CreatedAt);
    }
}