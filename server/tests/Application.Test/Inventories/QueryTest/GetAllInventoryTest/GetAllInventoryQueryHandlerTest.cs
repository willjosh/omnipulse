using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inventory.Query;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Application.Test.Inventories.QueryTest.GetAllInventoryTest;

public class GetAllInventoryQueryHandlerTest
{
    private readonly Mock<IInventoryRepository> _inventoryRepositoryMock;
    private readonly GetAllInventoryQueryHandler _handler;
    private readonly Mock<IAppLogger<GetAllInventoryQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllInventoryQuery>> _mockValidator;
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public GetAllInventoryQueryHandlerTest()
    {
        _inventoryRepositoryMock = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryMappingProfile>());
        _mapper = config.CreateMapper();

        _handler = new GetAllInventoryQueryHandler(
            _inventoryRepositoryMock.Object,
            _mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    private static GetAllInventoryQuery CreateValidQuery(
        int pageNumber = 1,
        int pageSize = 10,
        string? search = null,
        string? sortBy = null,
        bool sortDescending = false)
    {
        var parameters = new PaginationParameters
        {
            PageNumber = pageNumber,
            PageSize = pageSize,
            Search = search,
            SortBy = sortBy,
            SortDescending = sortDescending
        };
        return new GetAllInventoryQuery(parameters);
    }

    private void SetupValidValidation(GetAllInventoryQuery query)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(GetAllInventoryQuery query, string propertyName = "Parameters", string errorMessage = "Invalid Parameters")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
                     .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_When_Successful()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inventories = CreateInventoryList(3);
        var pagedResult = new PagedResult<Inventory>
        {
            Items = inventories,
            TotalCount = inventories.Count,
            PageNumber = 1,
            PageSize = 10
        };
        _inventoryRepositoryMock.Setup(r => r.GetAllInventoriesPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(3, result.Items.Count);
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);

        // Verify first inventory mapping
        var firstInventory = inventories.First();
        var firstDto = result.Items.First();
        Assert.Equal(firstInventory.QuantityOnHand, firstDto.QuantityOnHand);
        Assert.Equal(firstInventory.MinStockLevel, firstDto.MinStockLevel);
        Assert.Equal(firstInventory.MaxStockLevel, firstDto.MaxStockLevel);
        Assert.Equal(firstInventory.NeedsReorder, firstDto.NeedsReorder);
        Assert.Equal(firstInventory.UnitCost, firstDto.UnitCost);

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _inventoryRepositoryMock.Verify(r => r.GetAllInventoriesPagedAsync(query.Parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_Items()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var pagedResult = new PagedResult<Inventory>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };
        _inventoryRepositoryMock.Setup(r => r.GetAllInventoriesPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: -1); // Invalid page number
        SetupInvalidValidation(query, "PageNumber", "Page number must be greater than 0");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(query, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()), Times.Once);
        _inventoryRepositoryMock.Verify(r => r.GetAllInventoriesPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }

    [Theory]
    [InlineData(1, 5)]
    [InlineData(2, 10)]
    [InlineData(3, 25)]
    public async Task Handler_Should_Handle_Different_Pagination_Parameters(int pageNumber, int pageSize)
    {
        // Arrange
        var query = CreateValidQuery(pageNumber: pageNumber, pageSize: pageSize);
        SetupValidValidation(query);
        var inventories = CreateInventoryList(2);
        var pagedResult = new PagedResult<Inventory>
        {
            Items = inventories,
            TotalCount = 50, // Simulate larger dataset
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        _inventoryRepositoryMock.Setup(r => r.GetAllInventoriesPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(pageNumber, result.PageNumber);
        Assert.Equal(pageSize, result.PageSize);
        Assert.Equal(50, result.TotalCount);
    }

    [Fact]
    public async Task Handler_Should_Handle_Search_Parameters()
    {
        // Arrange
        var query = CreateValidQuery(search: "filter");
        SetupValidValidation(query);
        var inventories = CreateInventoryList(1);
        var pagedResult = new PagedResult<Inventory>
        {
            Items = inventories,
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _inventoryRepositoryMock.Setup(r => r.GetAllInventoriesPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Single(result.Items);
        _inventoryRepositoryMock.Verify(r => r.GetAllInventoriesPagedAsync(
            It.Is<PaginationParameters>(p => p.Search == "filter")), Times.Once);
    }

    [Theory]
    [InlineData("quantityonhand", false)]
    [InlineData("quantityonhand", true)]
    [InlineData("unitcost", false)]
    [InlineData("unitcost", true)]
    public async Task Handler_Should_Handle_Sorting_Parameters(string sortBy, bool sortDescending)
    {
        // Arrange
        var query = CreateValidQuery(sortBy: sortBy, sortDescending: sortDescending);
        SetupValidValidation(query);
        var inventories = CreateInventoryList(2);
        var pagedResult = new PagedResult<Inventory>
        {
            Items = inventories,
            TotalCount = 2,
            PageNumber = 1,
            PageSize = 10
        };
        _inventoryRepositoryMock.Setup(r => r.GetAllInventoriesPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(2, result.Items.Count);
        _inventoryRepositoryMock.Verify(r => r.GetAllInventoriesPagedAsync(
            It.Is<PaginationParameters>(p => p.SortBy == sortBy && p.SortDescending == sortDescending)), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Map_All_Properties_Correctly()
    {
        // Arrange
        var query = CreateValidQuery();
        SetupValidValidation(query);
        var inventory = CreateInventoryEntity(
            id: 42,
            inventoryItemId: 1,
            quantityOnHand: 150,
            minStockLevel: 25,
            maxStockLevel: 200,
            unitCost: 99.99m,
            needsReorder: true);

        var pagedResult = new PagedResult<Inventory>
        {
            Items = [inventory],
            TotalCount = 1,
            PageNumber = 1,
            PageSize = 10
        };
        _inventoryRepositoryMock.Setup(r => r.GetAllInventoriesPagedAsync(query.Parameters))
            .ReturnsAsync(pagedResult);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        var dto = result.Items.First();
        Assert.Equal(150, dto.QuantityOnHand);
        Assert.Equal(25, dto.MinStockLevel);
        Assert.Equal(200, dto.MaxStockLevel);
        Assert.True(dto.NeedsReorder);
        Assert.Equal(99.99m, dto.UnitCost);
        Assert.Equal("Test Part 1", dto.InventoryItemName);
        Assert.Equal("Main Warehouse", dto.LocationName);
    }

    private static List<Inventory> CreateInventoryList(int count = 5)
    {
        var inventories = new List<Inventory>();

        for (int i = 1; i <= count; i++)
        {
            inventories.Add(CreateInventoryEntity(
                id: i,
                inventoryItemId: i,
                quantityOnHand: 50 + (i * 10),
                minStockLevel: 10,
                maxStockLevel: 100 + (i * 10),
                unitCost: 10.00m + (i * 5),
                needsReorder: i % 2 == 0
            ));
        }

        return inventories;
    }

    private static Inventory CreateInventoryEntity(
        int id = 1,
        int inventoryItemId = 1,
        int? inventoryItemLocationId = 1,
        int quantityOnHand = 50,
        int minStockLevel = 10,
        int maxStockLevel = 100,
        decimal unitCost = 25.00m,
        bool needsReorder = false)
    {
        var inventoryItem = new InventoryItem
        {
            ID = inventoryItemId,
            ItemNumber = $"ITEM-{inventoryItemId:D3}",
            ItemName = $"Test Part {inventoryItemId}",
            Description = $"Description for part {inventoryItemId}",
            Category = InventoryItemCategoryEnum.ENGINE, // Updated enum name
            Manufacturer = "Test Manufacturer",
            ManufacturerPartNumber = $"MPN-{inventoryItemId:D3}",
            UniversalProductCode = $"123456789{inventoryItemId:D3}",
            UnitCost = unitCost,
            UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Box, // Add appropriate enum value
            Supplier = "Test Supplier",
            WeightKG = 1.5,
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            // Required navigation properties
            Inventories = new List<Inventory>(),
            WorkOrderLineItems = new List<WorkOrderLineItem>()
        };

        var location = new InventoryItemLocation
        {
            ID = inventoryItemLocationId ?? 1,
            LocationName = "Main Warehouse", // Changed from Name to LocationName
            Address = "123 Main St, Test City", // Required property
            Longitude = -122.4194, // Required property
            Latitude = 37.7749, // Required property
            Capacity = 1000, // Required property
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            // Required navigation property
            Inventories = new List<Inventory>()
        };

        return new Inventory
        {
            ID = id,
            InventoryItemID = inventoryItemId,
            InventoryItemLocationID = inventoryItemLocationId,
            QuantityOnHand = quantityOnHand,
            MinStockLevel = minStockLevel,
            MaxStockLevel = maxStockLevel,
            UnitCost = unitCost,
            NeedsReorder = needsReorder,
            LastRestockedDate = FixedDate.AddDays(-id),
            InventoryItem = inventoryItem,
            InventoryItemLocation = location,
            InventoryTransactions = new List<InventoryTransaction>(),
            CreatedAt = FixedDate.AddDays(-id),
            UpdatedAt = FixedDate.AddDays(-id)
        };
    }
}