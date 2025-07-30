using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InventoryItems.Command.CreateInventoryItem;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.InventoryItems.CommandTest.CreateInventoryItem;

public class CreateInventoryItemCommandHandlerTest
{
    private readonly CreateInventoryItemCommandHandler _createInventoryItemCommandHandler;
    private readonly Mock<IInventoryRepository> _mockInventoryRepository;
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly Mock<IValidator<CreateInventoryItemCommand>> _mockValidator;
    private readonly Mock<IAppLogger<CreateInventoryItemCommandHandler>> _mockLogger;
    private readonly IMapper _mapper;

    public CreateInventoryItemCommandHandlerTest()
    {
        _mockInventoryItemRepository = new();
        _mockInventoryRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryItemMappingProfile>());
        _mapper = config.CreateMapper();

        _createInventoryItemCommandHandler = new(
            _mockInventoryItemRepository.Object,
            _mockInventoryRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    private CreateInventoryItemCommand CreateValidCommand(
        string itemNumber = "ITEM-001",
        string itemName = "Test Item",
        string? description = "Test Description",
        InventoryItemCategoryEnum? category = InventoryItemCategoryEnum.TIRES,
        string? manufacturer = "Test Manufacturer",
        string? manufacturerPartNumber = "MPN-001",
        string? universalProductCode = "123456789012",
        decimal? unitCost = 100.00m,
        InventoryItemUnitCostMeasurementUnitEnum? unitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
        string? supplier = "Test Supplier",
        double? weightKG = 5.5,
        bool isActive = true)
    {
        return new CreateInventoryItemCommand(
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

    private void SetupValidValidation(CreateInventoryItemCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateInventoryItemCommand command, string propertyName, string errorMessage = "Validation failed")
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
        SetupValidValidation(command);

        var expectedInventoryItem = new InventoryItem()
        {
            ID = 123,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ItemNumber = command.ItemNumber,
            ItemName = command.ItemName,
            Description = command.Description,
            Category = command.Category,
            Manufacturer = command.Manufacturer,
            ManufacturerPartNumber = command.ManufacturerPartNumber,
            UniversalProductCode = command.UniversalProductCode,
            UnitCost = command.UnitCost,
            UnitCostMeasurementUnit = command.UnitCostMeasurementUnit,
            Supplier = command.Supplier,
            WeightKG = command.WeightKG,
            IsActive = command.IsActive,
            Inventories = [],
            WorkOrderLineItems = []
        };

        var expectedInventory = new Inventory()
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InventoryItemID = expectedInventoryItem.ID,
            QuantityOnHand = 0,
            MinStockLevel = 0,
            MaxStockLevel = 0,
            ReorderPoint = 0,
            LastRestockedDate = null,
            UnitCost = command.UnitCost ?? 0,
            InventoryItemLocation = null!,
            InventoryItem = expectedInventoryItem,
            InventoryTransactions = []
        };

        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsManufacturerPartNumberUniqueAsync(command.Manufacturer, command.ManufacturerPartNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.AddAsync(It.IsAny<InventoryItem>())).ReturnsAsync(expectedInventoryItem);
        _mockInventoryRepository.Setup(r => r.AddAsync(It.IsAny<Inventory>())).ReturnsAsync(expectedInventory);
        _mockInventoryItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedInventoryItem.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(command.ItemNumber), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsManufacturerPartNumberUniqueAsync(command.Manufacturer, command.ManufacturerPartNumber), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockInventoryRepository.Verify(r => r.AddAsync(It.IsAny<Inventory>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_ItemNumber()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(false);
        // Note: Other uniqueness checks won't be reached since ItemNumber check fails first, but setting up for completeness
        _mockInventoryItemRepository.Setup(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsManufacturerPartNumberUniqueAsync(command.Manufacturer, command.ManufacturerPartNumber)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _createInventoryItemCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(command.ItemNumber), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "ItemNumber", "Item number is required");

        // Note: Uniqueness checks won't be reached since validation fails first, but setting up for completeness
        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsUniversalProductCodeUniqueAsync(command.UniversalProductCode)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.IsManufacturerPartNumberUniqueAsync(command.Manufacturer, command.ManufacturerPartNumber)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _createInventoryItemCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(It.IsAny<string>()), Times.Never);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Optional_Fields()
    {
        // Given
        var command = CreateValidCommand(
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

        var expectedInventoryItem = new InventoryItem()
        {
            ID = 456,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ItemNumber = command.ItemNumber,
            ItemName = command.ItemName,
            Description = command.Description,
            Category = command.Category,
            Manufacturer = command.Manufacturer,
            ManufacturerPartNumber = command.ManufacturerPartNumber,
            UniversalProductCode = command.UniversalProductCode,
            UnitCost = command.UnitCost,
            UnitCostMeasurementUnit = command.UnitCostMeasurementUnit,
            Supplier = command.Supplier,
            WeightKG = command.WeightKG,
            IsActive = command.IsActive,
            Inventories = [],
            WorkOrderLineItems = []
        };

        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        // No need to setup UPC and MPN uniqueness checks since they are null
        _mockInventoryItemRepository.Setup(r => r.AddAsync(It.IsAny<InventoryItem>())).ReturnsAsync(expectedInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedInventoryItem.ID, result);
        Assert.Null(expectedInventoryItem.Description);
        Assert.Null(expectedInventoryItem.Category);
        Assert.Null(expectedInventoryItem.Manufacturer);
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(command.ItemNumber), Times.Once);
        // Verify that UPC and MPN uniqueness checks are NOT called since values are null
        _mockInventoryItemRepository.Verify(r => r.IsUniversalProductCodeUniqueAsync(It.IsAny<string>()), Times.Never);
        _mockInventoryItemRepository.Verify(r => r.IsManufacturerPartNumberUniqueAsync(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
    }
}