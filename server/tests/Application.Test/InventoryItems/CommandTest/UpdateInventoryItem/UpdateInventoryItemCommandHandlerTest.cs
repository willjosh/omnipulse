using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InventoryItems.Command.UpdateInventoryItem;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.InventoryItems.CommandTest.UpdateInventoryItem;

public class UpdateInventoryItemCommandHandlerTest
{
    private readonly UpdateInventoryItemCommandHandler _updateInventoryItemCommandHandler;
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly Mock<IValidator<UpdateInventoryItemCommand>> _mockValidator;
    private readonly Mock<IAppLogger<UpdateInventoryItemCommandHandler>> _mockLogger;
    private readonly IMapper _mapper;

    public UpdateInventoryItemCommandHandlerTest()
    {
        _mockInventoryItemRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryItemMappingProfile>());
        _mapper = config.CreateMapper();

        _updateInventoryItemCommandHandler = new(
            _mockInventoryItemRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    private static UpdateInventoryItemCommand CreateValidCommand(
        int inventoryItemID = 123,
        string itemNumber = "ITEM-001-UPDATED",
        string itemName = "Updated Test Item",
        string? description = "Updated Test Description",
        InventoryItemCategoryEnum? category = InventoryItemCategoryEnum.BRAKES,
        string? manufacturer = "Updated Test Manufacturer",
        string? manufacturerPartNumber = "MPN-001-UPDATED",
        string? universalProductCode = "123456789999",
        decimal? unitCost = 777.00m,
        InventoryItemUnitCostMeasurementUnitEnum? unitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Kilogram,
        string? supplier = "Updated Test Supplier",
        double? weightKG = 7.5,
        bool isActive = true)
    {
        return new UpdateInventoryItemCommand(
            InventoryItemID: inventoryItemID,
            ItemNumber: itemNumber,
            ItemName: itemName,
            Description: description,
            Category: category,
            Manufacturer: manufacturer,
            ManufacturerPartNumber: manufacturerPartNumber,
            UniversalProductCode: universalProductCode,
            UnitCost: unitCost,
            UnitCostMeasurementUnit: unitCostMeasurementUnit,
            Supplier: supplier,
            WeightKG: weightKG,
            IsActive: isActive
        );
    }

    private static InventoryItem CreateExistingInventoryItem()
    {
        return new InventoryItem()
        {
            ID = 123,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            ItemNumber = "ITEM-001",
            ItemName = "Original Test Item",
            Description = "Original Test Description",
            Category = InventoryItemCategoryEnum.TIRES,
            Manufacturer = "Original Test Manufacturer",
            ManufacturerPartNumber = "MPN-001",
            UniversalProductCode = "123456789012",
            UnitCost = 100.00m,
            UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
            Supplier = "Original Test Supplier",
            WeightKG = 5.5,
            IsActive = true,
            Inventories = [],
            WorkOrderLineItems = []
        };
    }

    private void SetupValidValidation(UpdateInventoryItemCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateInventoryItemCommand command, string propertyName, string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_InventoryItemID_On_Success()
    {
        // Given
        var command = CreateValidCommand();
        var existingInventoryItem = CreateExistingInventoryItem();
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(command.InventoryItemID)).ReturnsAsync(existingInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsManufacturerPartNumberUniqueAsync(command.Manufacturer, command.ManufacturerPartNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingInventoryItem.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.Update(existingInventoryItem), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_InventoryItem_Does_Not_Exist()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(command.InventoryItemID)).ReturnsAsync((InventoryItem?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "ItemNumber", "Item number is required");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_ItemNumber()
    {
        // Given
        var command = CreateValidCommand();
        var existingInventoryItem = CreateExistingInventoryItem();
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(command.InventoryItemID)).ReturnsAsync(existingInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(command.ItemNumber), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_UniversalProductCode()
    {
        // Given
        var command = CreateValidCommand();
        var existingInventoryItem = CreateExistingInventoryItem();
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(command.InventoryItemID)).ReturnsAsync(existingInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_ManufacturerPartNumber()
    {
        // Given
        var command = CreateValidCommand();
        var existingInventoryItem = CreateExistingInventoryItem();
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(command.InventoryItemID)).ReturnsAsync(existingInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsManufacturerPartNumberUniqueAsync(command.Manufacturer, command.ManufacturerPartNumber)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsManufacturerPartNumberUniqueAsync(command.Manufacturer, command.ManufacturerPartNumber), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Not_Check_Uniqueness_When_Values_Are_Not_Changed()
    {
        // Given
        var existingInventoryItem = CreateExistingInventoryItem();
        var command = new UpdateInventoryItemCommand(
            InventoryItemID: existingInventoryItem.ID,
            ItemNumber: existingInventoryItem.ItemNumber, // Same as existing
            ItemName: "Updated Item Name", // Different from existing
            Description: existingInventoryItem.Description,
            Category: existingInventoryItem.Category,
            Manufacturer: existingInventoryItem.Manufacturer, // Same as existing
            ManufacturerPartNumber: existingInventoryItem.ManufacturerPartNumber, // Same as existing
            UniversalProductCode: existingInventoryItem.UniversalProductCode, // Same as existing
            UnitCost: existingInventoryItem.UnitCost,
            UnitCostMeasurementUnit: existingInventoryItem.UnitCostMeasurementUnit,
            Supplier: existingInventoryItem.Supplier,
            WeightKG: existingInventoryItem.WeightKG,
            IsActive: existingInventoryItem.IsActive
        );
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(command.InventoryItemID)).ReturnsAsync(existingInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingInventoryItem.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(command.InventoryItemID), Times.Once);

        // Verify uniqueness checks are NOT called since values haven't changed
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(It.IsAny<string>()), Times.Never);
        _mockInventoryItemRepository.Verify(r => r.IsUniversalProductCodeUniqueAsync(It.IsAny<string>()), Times.Never);
        _mockInventoryItemRepository.Verify(r => r.IsManufacturerPartNumberUniqueAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _mockInventoryItemRepository.Verify(r => r.Update(existingInventoryItem), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Optional_Fields()
    {
        // Given
        var existingInventoryItem = CreateExistingInventoryItem();
        var command = CreateValidCommand(
            inventoryItemID: existingInventoryItem.ID,
            description: null,
            category: null,
            manufacturer: null,
            manufacturerPartNumber: null,
            universalProductCode: null,
            unitCost: null,
            unitCostMeasurementUnit: null,
            supplier: null,
            weightKG: null
        );
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(command.InventoryItemID)).ReturnsAsync(existingInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingInventoryItem.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(command.ItemNumber), Times.Once);

        // Verify that UPC and MPN uniqueness checks are NOT called since values are null
        _mockInventoryItemRepository.Verify(r => r.IsUniversalProductCodeUniqueAsync(It.IsAny<string>()), Times.Never);
        _mockInventoryItemRepository.Verify(r => r.IsManufacturerPartNumberUniqueAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

        _mockInventoryItemRepository.Verify(r => r.Update(existingInventoryItem), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}