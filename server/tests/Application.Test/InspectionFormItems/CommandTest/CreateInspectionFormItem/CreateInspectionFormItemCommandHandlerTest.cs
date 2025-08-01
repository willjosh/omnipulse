using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionFormItems.Command.CreateInspectionFormItem;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionFormItems.CommandTest.CreateInspectionFormItem;

public class CreateInspectionFormItemCommandHandlerTest
{
    private readonly CreateInspectionFormItemCommandHandler _commandHandler;
    private readonly Mock<IInspectionFormItemRepository> _mockInspectionFormItemRepository = new();
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository = new();
    private readonly Mock<IValidator<CreateInspectionFormItemCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<CreateInspectionFormItemCommandHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);
    private const int ItemLabelMaxLength = 200;
    private const int ItemDescriptionMaxLength = 500;
    private const int ItemInstructionsMaxLength = 4000;

    public CreateInspectionFormItemCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionFormItemMappingProfile>());
        _mapper = config.CreateMapper();
        _commandHandler = new CreateInspectionFormItemCommandHandler(
            _mockInspectionFormItemRepository.Object,
            _mockInspectionFormRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static CreateInspectionFormItemCommand CreateValidCommand(
        int inspectionFormID = 1,
        string itemLabel = "Test Inspection Item Label",
        string? itemDescription = "Verify engine oil level and quality",
        string? itemInstructions = "Remove dipstick, check oil level between min/max marks",
        InspectionFormItemTypeEnum itemType = InspectionFormItemTypeEnum.PassFail,
        bool isRequired = true) => new(inspectionFormID, itemLabel, itemDescription, itemInstructions, itemType, isRequired);

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
        string itemLabel = "")
    {
        return new InspectionFormItem
        {
            ID = id,
            InspectionFormID = inspectionFormID,
            ItemLabel = itemLabel,
            ItemDescription = "Test description",
            ItemInstructions = "Test instructions",
            InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
            IsRequired = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            InspectionForm = null! // Required property
        };
    }

    private void SetupValidValidation(CreateInspectionFormItemCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateInspectionFormItemCommand command, string propertyName = nameof(CreateInspectionFormItemCommand.ItemLabel), string errorMessage = "Invalid Validation")
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
        var inspectionForm = CreateInspectionFormEntity();
        var newInspectionFormItem = CreateInspectionFormItemEntity();

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.AddAsync(It.IsAny<InspectionFormItem>()))
            .ReturnsAsync(newInspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newInspectionFormItem.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.AddAsync(It.IsAny<InspectionFormItem>()), Times.Once);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_InspectionForm_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync((InspectionForm?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Equal(nameof(InspectionForm), exception.EntityName);
        Assert.Equal(nameof(InspectionForm.ID), exception.PropertyName);
        Assert.Equal(command.InspectionFormID.ToString(), exception.PropertyValue);

        _mockInspectionFormItemRepository.Verify(r => r.AddAsync(It.IsAny<InspectionFormItem>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_InspectionForm_Is_Inactive()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inactiveInspectionForm = CreateInspectionFormEntity(isActive: false);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inactiveInspectionForm);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        _mockInspectionFormItemRepository.Verify(r => r.AddAsync(It.IsAny<InspectionFormItem>()), Times.Never);
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
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.AddAsync(It.IsAny<InspectionFormItem>()), Times.Never);
        _mockInspectionFormItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Theory]
    [InlineData(InspectionFormItemTypeEnum.PassFail)]
    public async Task Handler_Should_Handle_Different_Item_Types(InspectionFormItemTypeEnum itemType)
    {
        // Arrange
        var command = CreateValidCommand(itemType: itemType);
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity();
        var newInspectionFormItem = CreateInspectionFormItemEntity();

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.AddAsync(It.IsAny<InspectionFormItem>()))
            .ReturnsAsync(newInspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newInspectionFormItem.ID, result);
        _mockInspectionFormItemRepository.Verify(r => r.AddAsync(
            It.Is<InspectionFormItem>(item => item.InspectionFormItemTypeEnum == itemType)), Times.Once);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Handler_Should_Handle_Required_And_Optional_Items(bool isRequired)
    {
        // Arrange
        var command = CreateValidCommand(isRequired: isRequired);
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity();
        var newInspectionFormItem = CreateInspectionFormItemEntity();

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.AddAsync(It.IsAny<InspectionFormItem>()))
            .ReturnsAsync(newInspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newInspectionFormItem.ID, result);
        _mockInspectionFormItemRepository.Verify(r => r.AddAsync(
            It.Is<InspectionFormItem>(item => item.IsRequired == isRequired)), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Optional_Fields()
    {
        // Arrange
        var command = CreateValidCommand(
            itemDescription: null,
            itemInstructions: null);
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity();
        var newInspectionFormItem = CreateInspectionFormItemEntity();

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.AddAsync(It.IsAny<InspectionFormItem>()))
            .ReturnsAsync(newInspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newInspectionFormItem.ID, result);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Handler_Should_Work_With_Different_InspectionForm_IDs(int inspectionFormID)
    {
        // Arrange
        var command = CreateValidCommand(inspectionFormID: inspectionFormID);
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity(id: inspectionFormID);
        var newInspectionFormItem = CreateInspectionFormItemEntity();

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(inspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormItemRepository.Setup(r => r.AddAsync(It.IsAny<InspectionFormItem>()))
            .ReturnsAsync(newInspectionFormItem);
        _mockInspectionFormItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(newInspectionFormItem.ID, result);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(inspectionFormID), Times.Once);
    }
}