using System;
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
using Xunit;

namespace Application.Test.InventoryItems.CommandTest.CreateInventoryItem;

public class CreateInventoryItemCommandHandlerTest
{
    private readonly CreateInventoryItemCommandHandler _createInventoryItemCommandHandler;
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly Mock<IValidator<CreateInventoryItemCommand>> _mockValidator;
    private readonly Mock<IAppLogger<CreateInventoryItemCommandHandler>> _mockLogger;
    private readonly IMapper _mapper;

    public CreateInventoryItemCommandHandlerTest()
    {
        _mockInventoryItemRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InventoryItemMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _createInventoryItemCommandHandler = new(
            _mockInventoryItemRepository.Object,
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

        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(true);
        _mockInventoryItemRepository.Setup(r => r.AddAsync(It.IsAny<InventoryItem>())).ReturnsAsync(expectedInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedInventoryItem.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.IsItemNumberUniqueAsync(command.ItemNumber), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_ItemNumber()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockInventoryItemRepository.Setup(r => r.IsItemNumberUniqueAsync(command.ItemNumber)).ReturnsAsync(false);

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
        _mockInventoryItemRepository.Setup(r => r.AddAsync(It.IsAny<InventoryItem>())).ReturnsAsync(expectedInventoryItem);
        _mockInventoryItemRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedInventoryItem.ID, result);
        Assert.Null(expectedInventoryItem.Description);
        Assert.Null(expectedInventoryItem.Category);
        Assert.Null(expectedInventoryItem.Manufacturer);
    }
}
