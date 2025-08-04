using System;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inventory.Query.GetInventory;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using Moq;

using Xunit;

namespace Application.Test.Inventories.QueryTest.GetInventoryTest;

public class GetInventoryHandlerTest
{
    private readonly GetInventoryQueryHandler _queryHandler;
    private readonly Mock<IInventoryRepository> _mockInventoryRepository = new();
    private readonly Mock<IAppLogger<GetInventoryQueryHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetInventoryHandlerTest()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InventoryMappingProfile>();
        });
        _mapper = config.CreateMapper();
        _queryHandler = new GetInventoryQueryHandler(
            _mockInventoryRepository.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static GetInventoryQuery CreateValidQuery(int inventoryID = 1) => new(inventoryID);

    [Fact]
    public async Task Handler_Should_Return_InventoryDetailDTO_When_Found()
    {
        // Arrange
        var query = CreateValidQuery();
        var expectedInventory = CreateInventoryEntity();

        _mockInventoryRepository.Setup(r => r.GetInventoryWithDetailsAsync(query.InventoryID))
            .ReturnsAsync(expectedInventory);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(expectedInventory.InventoryItem.ItemName, result.InventoryItemName);
        Assert.Equal(expectedInventory.InventoryItemLocation?.LocationName ?? "Unknown Location", result.LocationName);
        Assert.Equal(expectedInventory.QuantityOnHand, result.QuantityOnHand);
        Assert.Equal(expectedInventory.MinStockLevel, result.MinStockLevel);
        Assert.Equal(expectedInventory.MaxStockLevel, result.MaxStockLevel);
        Assert.Equal(expectedInventory.NeedsReorder, result.NeedsReorder);
        // Fix the date assertion - compare just the date part if the DTO only stores date
        Assert.Equal(expectedInventory.LastRestockedDate?.Date, result.LastRestockedDate?.Date);
        Assert.Equal(expectedInventory.UnitCost, result.UnitCost);

        _mockInventoryRepository.Verify(r => r.GetInventoryWithDetailsAsync(query.InventoryID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Not_Found()
    {
        // Arrange
        var query = CreateValidQuery();
        _mockInventoryRepository.Setup(r => r.GetInventoryWithDetailsAsync(query.InventoryID))
            .ReturnsAsync((Inventory?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _queryHandler.Handle(query, CancellationToken.None));

        Assert.Equal("Domain.Entities.Inventory", exception.EntityName);
        Assert.Equal(nameof(Inventory.ID), exception.PropertyName);
        Assert.Equal(query.InventoryID.ToString(), exception.PropertyValue);

        _mockInventoryRepository.Verify(r => r.GetInventoryWithDetailsAsync(query.InventoryID), Times.Once);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Handler_Should_Throw_BadRequestException_On_Invalid_InventoryID(int invalidId)
    {
        // Arrange
        var query = new GetInventoryQuery(invalidId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        Assert.Contains("Invalid inventory ID", exception.Message);
        Assert.Contains(invalidId.ToString(), exception.Message);
        _mockInventoryRepository.Verify(r => r.GetInventoryWithDetailsAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Handle_Inventory_With_Null_Location()
    {
        // Arrange
        var query = new GetInventoryQuery(1);
        var inventoryWithoutLocation = CreateInventoryEntity();
        inventoryWithoutLocation.InventoryItemLocation = null;
        inventoryWithoutLocation.InventoryItemLocationID = null;

        _mockInventoryRepository.Setup(r => r.GetInventoryWithDetailsAsync(query.InventoryID))
            .ReturnsAsync(inventoryWithoutLocation);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("Unknown Location", result.LocationName); // This should now work with the updated mapping
        _mockInventoryRepository.Verify(r => r.GetInventoryWithDetailsAsync(query.InventoryID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Inventory_With_Null_Optional_Fields()
    {
        // Arrange
        var query = new GetInventoryQuery(1);
        var inventory = CreateInventoryEntity();
        inventory.LastRestockedDate = null; // This should now be handled properly
        inventory.InventoryItem.Description = null;
        inventory.InventoryItem.Manufacturer = null;

        _mockInventoryRepository.Setup(r => r.GetInventoryWithDetailsAsync(query.InventoryID))
            .ReturnsAsync(inventory);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Null(result.LastRestockedDate); // Should be null, not "N/A" string
        Assert.NotNull(result.InventoryItemName);
        Assert.NotNull(result.LocationName);
        _mockInventoryRepository.Verify(r => r.GetInventoryWithDetailsAsync(query.InventoryID), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Information_Messages()
    {
        // Arrange
        var query = new GetInventoryQuery(1);
        var expectedInventory = CreateInventoryEntity();

        _mockInventoryRepository.Setup(r => r.GetInventoryWithDetailsAsync(query.InventoryID))
            .ReturnsAsync(expectedInventory);

        // Act
        await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Handling GetInventoryQuery(1)"))),
            Times.Once);
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Returning InventoryDetailDTO for InventoryID: 1"))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Error_When_Inventory_Not_Found()
    {
        // Arrange
        var nonExistentId = 999;
        var query = new GetInventoryQuery(nonExistentId);

        _mockInventoryRepository.Setup(r => r.GetInventoryWithDetailsAsync(nonExistentId))
            .ReturnsAsync((Inventory?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockLogger.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("Inventory with ID 999 not found."))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Error_When_Invalid_ID_Provided()
    {
        // Arrange
        var invalidId = -1;
        var query = new GetInventoryQuery(invalidId);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(
            () => _queryHandler.Handle(query, CancellationToken.None)
        );

        _mockLogger.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("Invalid inventory ID: -1"))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Inventory_States()
    {
        // Arrange
        var query = new GetInventoryQuery(1);
        var inventory = CreateInventoryEntity(
            id: 1,
            quantityOnHand: 5,
            minStockLevel: 10,
            maxStockLevel: 100,
            needsReorder: true,
            unitCost: 199.99m
        );

        _mockInventoryRepository.Setup(r => r.GetInventoryWithDetailsAsync(query.InventoryID))
            .ReturnsAsync(inventory);

        // Act
        var result = await _queryHandler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(5, result.QuantityOnHand);
        Assert.Equal(10, result.MinStockLevel);
        Assert.Equal(100, result.MaxStockLevel);
        Assert.True(result.NeedsReorder);
        Assert.Equal(199.99m, result.UnitCost);
    }

    private static Inventory CreateInventoryEntity(
        int id = 1,
        int inventoryItemId = 1,
        int? inventoryItemLocationId = 1,
        int quantityOnHand = 50,
        int minStockLevel = 10,
        int maxStockLevel = 100,
        bool needsReorder = false,
        decimal unitCost = 25.99m,
        DateTime? lastRestockedDate = null)
    {
        var inventoryItem = new InventoryItem
        {
            ID = inventoryItemId,
            ItemNumber = $"ITEM-{inventoryItemId:D3}",
            ItemName = $"Test Part {inventoryItemId}",
            Description = $"Description for part {inventoryItemId}",
            Category = InventoryItemCategoryEnum.ENGINE,
            Manufacturer = "Test Manufacturer",
            ManufacturerPartNumber = $"MPN-{inventoryItemId:D3}",
            UniversalProductCode = $"123456789{inventoryItemId:D3}",
            UnitCost = unitCost,
            UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Box,
            Supplier = "Test Supplier",
            WeightKG = 1.5,
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inventories = new List<Inventory>(),
            WorkOrderLineItems = new List<WorkOrderLineItem>()
        };

        var location = inventoryItemLocationId.HasValue ? new InventoryItemLocation
        {
            ID = inventoryItemLocationId.Value,
            LocationName = "Main Warehouse",
            Address = "123 Main St, Test City",
            Longitude = -122.4194,
            Latitude = 37.7749,
            Capacity = 1000,
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inventories = new List<Inventory>()
        } : null;

        return new Inventory
        {
            ID = id,
            InventoryItemID = inventoryItemId,
            InventoryItemLocationID = inventoryItemLocationId,
            QuantityOnHand = quantityOnHand,
            MinStockLevel = minStockLevel,
            MaxStockLevel = maxStockLevel,
            NeedsReorder = needsReorder,
            LastRestockedDate = lastRestockedDate ?? FixedDate.AddDays(-7),
            UnitCost = unitCost,
            InventoryItem = inventoryItem,
            InventoryItemLocation = location,
            InventoryTransactions = new List<InventoryTransaction>(),
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate
        };
    }
}