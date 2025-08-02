using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionFormItems.Command.UpdateInspectionFormItem;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionFormItems.CommandTest.UpdateInspectionFormItem;

public class UpdateInspectionFormItemCommandHandlerTest
{
    private readonly UpdateInspectionFormItemCommandHandler _commandHandler;
    private readonly Mock<IInspectionFormItemRepository> _mockInspectionFormItemRepository = new();
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository = new();
    private readonly Mock<IValidator<UpdateInspectionFormItemCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<UpdateInspectionFormItemCommandHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);
    private const int ItemLabelMaxLength = 200;
    private const int ItemDescriptionMaxLength = 500;
    private const int ItemInstructionsMaxLength = 4000;

    public UpdateInspectionFormItemCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionFormItemMappingProfile>());
        _mapper = config.CreateMapper();
        _commandHandler = new UpdateInspectionFormItemCommandHandler(
            _mockInspectionFormItemRepository.Object,
            _mockInspectionFormRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static UpdateInspectionFormItemCommand CreateValidCommand(
        int inspectionFormItemID = 1,
        string itemLabel = "Updated Engine Oil Check",
        string? itemDescription = "Updated description",
        string? itemInstructions = "Updated instructions",
        bool isRequired = true) => new(inspectionFormItemID, itemLabel, itemDescription, itemInstructions, isRequired);

    private static InspectionForm CreateInspectionFormEntity(
        int id = 1,
        string title = "Vehicle Safety Inspection",
        bool isActive = true)
    {
        return new InspectionForm
        {
            ID = id,
            Title = title,
            Description = "Test inspection form",
            IsActive = isActive,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = [],
            InspectionFormItems = []
        };
    }

    private static InspectionFormItem CreateInspectionFormItemEntity(
        int id = 1,
        int inspectionFormID = 1,
        string itemLabel = "Check Engine Oil")
    {
        return new InspectionFormItem
        {
            ID = id,
            InspectionFormID = inspectionFormID,
            ItemLabel = itemLabel,
            ItemDescription = "Original description",
            ItemInstructions = "Original instructions",
            InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            IsRequired = true,
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            InspectionForm = null!
        };
    }

    private void SetupValidValidation(UpdateInspectionFormItemCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateInspectionFormItemCommand command, string propertyName = nameof(UpdateInspectionFormItemCommand.ItemLabel), string errorMessage = "Invalid Validation")
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
        var existingItem = CreateInspectionFormItemEntity();
        var inspectionForm = CreateInspectionFormEntity();

        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(existingItem);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(existingItem.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingItem.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.GetByIdAsync(command.InspectionFormItemID), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.Update(existingItem), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
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
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Equal(nameof(InspectionFormItem), exception.EntityName);
        Assert.Equal(nameof(InspectionFormItem.ID), exception.PropertyName);
        Assert.Equal(command.InspectionFormItemID.ToString(), exception.PropertyValue);

        _mockInspectionFormItemRepository.Verify(r => r.Update(It.IsAny<InspectionFormItem>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand(itemLabel: ""); // Invalid empty label
        SetupInvalidValidation(command);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.Update(It.IsAny<InspectionFormItem>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Current_InspectionForm_Is_Inactive()
    {
        // Arrange
        var command = CreateValidCommand(); // Same form ID
        SetupValidValidation(command);
        var existingItem = CreateInspectionFormItemEntity();
        var inactiveInspectionForm = CreateInspectionFormEntity(isActive: false); // Current form is inactive

        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(existingItem);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(existingItem.InspectionFormID))
            .ReturnsAsync(inactiveInspectionForm);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Contains("Cannot update items in inactive", exception.Message);
        Assert.Contains(existingItem.InspectionFormID.ToString(), exception.Message);
    }

    [Fact]
    public async Task Handler_Should_Update_All_Properties_Correctly()
    {
        // Arrange
        var command = CreateValidCommand(
            inspectionFormItemID: 1,
            itemLabel: "Updated Brake Check",
            itemDescription: "Updated brake description",
            itemInstructions: "Updated brake instructions",
            isRequired: false);
        SetupValidValidation(command);
        var existingItem = CreateInspectionFormItemEntity();
        var inspectionForm = CreateInspectionFormEntity();

        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(existingItem);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(existingItem.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingItem.ID, result);
        Assert.Equal(command.ItemLabel, existingItem.ItemLabel);
        Assert.Equal(command.ItemDescription, existingItem.ItemDescription);
        Assert.Equal(command.ItemInstructions, existingItem.ItemInstructions);
        Assert.Equal(command.IsRequired, existingItem.IsRequired);
        // Verify that InspectionFormItemTypeEnum was NOT changed (should remain original value)
        Assert.Equal(InspectionFormItemTypeEnum.PassFail, existingItem.InspectionFormItemTypeEnum);
        // Verify that InspectionFormID was NOT changed (items belong to one form permanently)
        Assert.Equal(1, existingItem.InspectionFormID);
        // Verify that ID and timestamps are not changed by mapping
        Assert.Equal(1, existingItem.ID);
        Assert.Equal(FixedDate, existingItem.CreatedAt);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Optional_Fields()
    {
        // Arrange
        var command = CreateValidCommand(
            itemDescription: null,
            itemInstructions: null);
        SetupValidValidation(command);
        var existingItem = CreateInspectionFormItemEntity();
        var inspectionForm = CreateInspectionFormEntity();

        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(existingItem);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(existingItem.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingItem.ID, result);
        Assert.Null(existingItem.ItemDescription);
        Assert.Null(existingItem.ItemInstructions);
    }

    [Theory]
    [InlineData(1, 1)]
    [InlineData(100, 100)]
    [InlineData(999, 999)]
    public async Task Handler_Should_Work_With_Different_IDs(int inspectionFormItemID, int inspectionFormID)
    {
        // Arrange
        var command = CreateValidCommand(inspectionFormItemID: inspectionFormItemID);
        var existingItem = CreateInspectionFormItemEntity(id: inspectionFormItemID, inspectionFormID: inspectionFormID);
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity(id: inspectionFormID);

        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(inspectionFormItemID))
            .ReturnsAsync(existingItem);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(inspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(inspectionFormItemID, result);
    }

    [Fact]
    public async Task Handler_Should_Log_Information_Messages()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var existingItem = CreateInspectionFormItemEntity();
        var inspectionForm = CreateInspectionFormEntity();

        _mockInspectionFormItemRepository.Setup(r => r.GetByIdAsync(command.InspectionFormItemID))
            .ReturnsAsync(existingItem);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(existingItem.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Handling UpdateInspectionFormItemCommand"))),
            Times.Once);
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Successfully updated"))),
            Times.Once);
    }
}