using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionFormItems.Command.DeactivateInspectionFormItem;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionFormItems.CommandTest.DeactivateInspectionFormItem;

public class DeactivateInspectionFormItemCommandHandlerTest
{
    private readonly DeactivateInspectionFormItemCommandHandler _commandHandler;
    private readonly Mock<IInspectionFormItemRepository> _mockInspectionFormItemRepository = new();
    private readonly Mock<IValidator<DeactivateInspectionFormItemCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<DeactivateInspectionFormItemCommandHandler>> _mockLogger = new();

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public DeactivateInspectionFormItemCommandHandlerTest()
    {
        _commandHandler = new DeactivateInspectionFormItemCommandHandler(
            _mockInspectionFormItemRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    private static DeactivateInspectionFormItemCommand CreateValidCommand(int inspectionFormItemID = 1) => new(inspectionFormItemID);

    private static InspectionFormItem CreateInspectionFormItem(
        int id = 1,
        int inspectionFormID = 1,
        string itemLabel = "Test Inspection Item",
        string? itemDescription = "Test description",
        string? itemInstructions = "Test instructions",
        InspectionFormItemTypeEnum itemType = InspectionFormItemTypeEnum.PassFail,
        bool isRequired = true,
        bool isActive = true)
    {
        return new InspectionFormItem
        {
            ID = id,
            InspectionFormID = inspectionFormID,
            ItemLabel = itemLabel,
            ItemDescription = itemDescription,
            ItemInstructions = itemInstructions,
            InspectionFormItemTypeEnum = itemType,
            IsRequired = isRequired,
            IsActive = isActive,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            InspectionForm = null! // Navigation property
        };
    }

    private void SetupValidValidation(DeactivateInspectionFormItemCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(DeactivateInspectionFormItemCommand command, string propertyName = nameof(DeactivateInspectionFormItemCommand.InspectionFormItemID), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Deactivate_InspectionFormItem_Successfully()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionFormItem = CreateInspectionFormItem();
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(inspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.InspectionFormItemID, result);
        Assert.False(inspectionFormItem.IsActive); // Verify soft delete
        _mockInspectionFormItemRepository.Verify(r => r.Update(inspectionFormItem), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupInvalidValidation(command, nameof(DeactivateInspectionFormItemCommand.InspectionFormItemID), "InspectionFormItemID must be a positive integer");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));
        Assert.Contains("InspectionFormItemID must be a positive integer", exception.Message);
        _mockInspectionFormItemRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.Update(It.IsAny<InspectionFormItem>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_InspectionFormItem_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync((InspectionFormItem?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
        Assert.Equal(nameof(InspectionFormItem), exception.EntityName);
        Assert.Equal(nameof(InspectionFormItem.ID), exception.PropertyName);
        Assert.Equal(command.InspectionFormItemID.ToString(), exception.PropertyValue);
        _mockInspectionFormItemRepository.Verify(r => r.Update(It.IsAny<InspectionFormItem>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_InspectionFormItem_Already_Inactive()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionFormItem = CreateInspectionFormItem(isActive: false); // Already inactive
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(inspectionFormItem);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));
        Assert.Contains("already inactive", exception.Message);
        Assert.Contains("already deactivated", exception.Message);
        _mockInspectionFormItemRepository.Verify(r => r.Update(It.IsAny<InspectionFormItem>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Deactivate_Required_InspectionFormItem_Successfully()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionFormItem = CreateInspectionFormItem(isRequired: true);
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(inspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.InspectionFormItemID, result);
        Assert.False(inspectionFormItem.IsActive); // Verify soft delete
        _mockInspectionFormItemRepository.Verify(r => r.Update(inspectionFormItem), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Deactivate_Optional_InspectionFormItem_Successfully()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionFormItem = CreateInspectionFormItem(isRequired: false);
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(inspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.InspectionFormItemID, result);
        Assert.False(inspectionFormItem.IsActive); // Verify soft delete
        _mockInspectionFormItemRepository.Verify(r => r.Update(inspectionFormItem), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Information_Messages()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionFormItem = CreateInspectionFormItem();
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(inspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Handling DeactivateInspectionFormItemCommand"))),
            Times.Once);
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Successfully soft deleted (deactivated)"))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Warning_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        var errorMessage = "InspectionFormItemID must be a positive integer";
        SetupInvalidValidation(command, nameof(DeactivateInspectionFormItemCommand.InspectionFormItemID), errorMessage);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));

        _mockLogger.Verify(
            x => x.LogWarning(It.Is<string>(s => s.Contains("Validation failed") && s.Contains(errorMessage))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Error_When_InspectionFormItem_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync((InspectionFormItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));

        _mockLogger.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("not found"))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Error_When_InspectionFormItem_Already_Inactive()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionFormItem = CreateInspectionFormItem(isActive: false);
        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(inspectionFormItem);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));

        _mockLogger.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("already inactive"))),
            Times.Once);
    }
}