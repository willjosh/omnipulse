using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InventoryItems.Query.GetInventoryItem;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

namespace Application.Test.InventoryItems.QueryTest.GetInventoryItem;

public class GetInventoryItemQueryHandlerTest
{
    private readonly GetInventoryItemQueryHandler _getInventoryItemQueryHandler;
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly Mock<IAppLogger<GetInventoryItemQueryHandler>> _mockLogger;

    public GetInventoryItemQueryHandlerTest()
    {
        _mockInventoryItemRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryItemMappingProfile>());
        var mapper = config.CreateMapper();

        _getInventoryItemQueryHandler = new GetInventoryItemQueryHandler(
            _mockInventoryItemRepository.Object,
            _mockLogger.Object,
            mapper
        );
    }

    [Fact]
    public async Task Handler_Should_Return_GetInventoryItemDTO_On_Success()
    {
        // Given
        var query = new GetInventoryItemQuery(1);

        var expectedInventoryItem = new InventoryItem
        {
            ID = 1,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ItemNumber = "INV-001",
            ItemName = "Test Inventory Item",
            Description = "Test Description",
            Category = InventoryItemCategoryEnum.ENGINE,
            Manufacturer = "Test Manufacturer",
            ManufacturerPartNumber = "MPN-001",
            UniversalProductCode = "123456789012",
            UnitCost = 100.00m,
            UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
            Supplier = "Test Supplier",
            WeightKG = 5.0,
            IsActive = true,
            Inventories = [],
            WorkOrderLineItems = []
        };

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(query.InventoryItemID))
            .ReturnsAsync(expectedInventoryItem);

        // When
        var result = await _getInventoryItemQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<GetInventoryItemDTO>(result);
        Assert.Equal(1, result.ID);
        Assert.Equal("INV-001", result.ItemNumber);
        Assert.Equal("Test Inventory Item", result.ItemName);
        Assert.Equal("Test Description", result.Description);
        Assert.Equal(InventoryItemCategoryEnum.ENGINE, result.Category);
        Assert.Equal("Test Manufacturer", result.Manufacturer);
        Assert.Equal("MPN-001", result.ManufacturerPartNumber);
        Assert.Equal("123456789012", result.UniversalProductCode);
        Assert.Equal(100.00m, result.UnitCost);
        Assert.Equal(InventoryItemUnitCostMeasurementUnitEnum.Unit, result.UnitCostMeasurementUnit);
        Assert.Equal("Test Supplier", result.Supplier);
        Assert.Equal(5.0, result.WeightKG);
        Assert.True(result.IsActive);

        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(query.InventoryItemID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_GetInventoryItemDTO_With_Null_Optional_Fields()
    {
        // Given
        var query = new GetInventoryItemQuery(2);

        var expectedInventoryItem = new InventoryItem
        {
            ID = 2,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ItemNumber = "INV-002",
            ItemName = "Minimal Inventory Item",
            Description = null,
            Category = null,
            Manufacturer = null,
            ManufacturerPartNumber = null,
            UniversalProductCode = null,
            UnitCost = null,
            UnitCostMeasurementUnit = null,
            Supplier = null,
            WeightKG = null,
            IsActive = false,
            Inventories = [],
            WorkOrderLineItems = []
        };

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(query.InventoryItemID))
            .ReturnsAsync(expectedInventoryItem);

        // When
        var result = await _getInventoryItemQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<GetInventoryItemDTO>(result);
        Assert.Equal(2, result.ID);
        Assert.Equal("INV-002", result.ItemNumber);
        Assert.Equal("Minimal Inventory Item", result.ItemName);
        Assert.Null(result.Description);
        Assert.Null(result.Category);
        Assert.Null(result.Manufacturer);
        Assert.Null(result.ManufacturerPartNumber);
        Assert.Null(result.UniversalProductCode);
        Assert.Null(result.UnitCost);
        Assert.Null(result.UnitCostMeasurementUnit);
        Assert.Null(result.Supplier);
        Assert.Null(result.WeightKG);
        Assert.False(result.IsActive);

        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(query.InventoryItemID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_NonExistent_InventoryItemID()
    {
        // Given
        var nonExistentInventoryItemId = 999;
        var query = new GetInventoryItemQuery(nonExistentInventoryItemId);

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(nonExistentInventoryItemId))
            .ReturnsAsync((InventoryItem?)null);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _getInventoryItemQueryHandler.Handle(query, CancellationToken.None)
        );

        Assert.Contains("InventoryItem", exception.Message);
        Assert.Contains("InventoryItemID", exception.Message);
        Assert.Contains(nonExistentInventoryItemId.ToString(), exception.Message);

        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(nonExistentInventoryItemId), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Various_Categories()
    {
        // Given
        var query = new GetInventoryItemQuery(3);

        var expectedInventoryItem = new InventoryItem
        {
            ID = 3,
            CreatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2024, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            ItemNumber = "INV-003",
            ItemName = "Brake Part",
            Description = "High quality brake part",
            Category = InventoryItemCategoryEnum.BRAKES,
            Manufacturer = "Brake Corp",
            ManufacturerPartNumber = "BRK-001",
            UniversalProductCode = null,
            UnitCost = 50.75m,
            UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
            Supplier = "Auto Parts Supplier",
            WeightKG = 2.5,
            IsActive = true,
            Inventories = [],
            WorkOrderLineItems = []
        };

        _mockInventoryItemRepository.Setup(r => r.GetByIdAsync(query.InventoryItemID))
            .ReturnsAsync(expectedInventoryItem);

        // When
        var result = await _getInventoryItemQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.Equal(InventoryItemCategoryEnum.BRAKES, result.Category);
        Assert.Equal(50.75m, result.UnitCost);
        Assert.Equal(2.5, result.WeightKG);

        _mockInventoryItemRepository.Verify(r => r.GetByIdAsync(query.InventoryItemID), Times.Once);
    }
}