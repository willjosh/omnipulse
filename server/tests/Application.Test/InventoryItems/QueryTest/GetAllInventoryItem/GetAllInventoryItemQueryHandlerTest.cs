using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InventoryItems.Query.GetAllInventoryItem;
using Application.MappingProfiles;
using Application.Models.PaginationModels;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.InventoryItems.QueryTest.GetAllInventoryItem;

public class GetAllInventoryItemQueryHandlerTest
{
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly GetAllInventoryItemQueryHandler _getAllInventoryItemQueryHandler;
    private readonly Mock<IAppLogger<GetAllInventoryItemQueryHandler>> _mockLogger;
    private readonly Mock<IValidator<GetAllInventoryItemQuery>> _mockValidator;

    public GetAllInventoryItemQueryHandlerTest()
    {
        _mockInventoryItemRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryItemMappingProfile>());
        var mapper = config.CreateMapper();

        _getAllInventoryItemQueryHandler = new GetAllInventoryItemQueryHandler(
            _mockInventoryItemRepository.Object,
            mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    private void SetupValidValidation(GetAllInventoryItemQuery query)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(validResult);
    }

    private void SetupInvalidValidation(GetAllInventoryItemQuery query, string propertyName = "Parameters.PageNumber", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.Validate(query))
                     .Returns(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_PagedResult_On_Success()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            Search = "Filter",
            SortBy = "ItemName",
            SortDescending = false
        };
        var query = new GetAllInventoryItemQuery(parameters);
        SetupValidValidation(query);

        var expectedInventoryEntities = new List<InventoryItem>
        {
            new()
            {
                ID = 1,
                CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ItemNumber = "ITM001",
                ItemName = "Oil Filter",
                Description = "Engine oil filter",
                Category = InventoryItemCategoryEnum.FILTERS,
                Manufacturer = "ACME",
                ManufacturerPartNumber = "AC-1234",
                UniversalProductCode = "123456789012",
                UnitCost = 15.99m,
                UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
                Supplier = "PartsCo",
                WeightKG = 0.5,
                IsActive = true,
                Inventories = [],
                WorkOrderLineItems = []
            },
            new()
            {
                ID = 2,
                CreatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                UpdatedAt = new DateTime(2023, 1, 1, 0, 0, 0, DateTimeKind.Utc),
                ItemNumber = "ITM002",
                ItemName = "Brake Pad",
                Description = "Front brake pad",
                Category = InventoryItemCategoryEnum.BRAKES,
                Manufacturer = "BrakeMakers",
                ManufacturerPartNumber = "BM-5678",
                UniversalProductCode = "987654321098",
                UnitCost = 45.50m,
                UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Box,
                Supplier = "BrakeSupplies",
                WeightKG = 2.0,
                IsActive = true,
                Inventories = [],
                WorkOrderLineItems = []
            }
        };
        var pagedInventoryEntities = new PagedResult<InventoryItem>
        {
            Items = expectedInventoryEntities,
            TotalCount = 10,
            PageNumber = 1,
            PageSize = 5
        };
        _mockInventoryItemRepository.Setup(r => r.GetAllInventoryItemsPagedAsync(parameters))
            .ReturnsAsync(pagedInventoryEntities);

        // When
        var result = await _getAllInventoryItemQueryHandler.Handle(query, CancellationToken.None);

        // Then
        Assert.NotNull(result);
        Assert.IsType<PagedResult<GetAllInventoryItemDTO>>(result);
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(2, result.Items.Count);
        Assert.Equal(2, result.Items.Count); // 2 items in this page
        var firstItem = result.Items[0];
        Assert.Equal("ITM001", firstItem.ItemNumber);
        Assert.Equal("Oil Filter", firstItem.ItemName);
        Assert.Equal("Engine oil filter", firstItem.Description);
        Assert.Equal(InventoryItemCategoryEnum.FILTERS, firstItem.Category);
        Assert.Equal("ACME", firstItem.Manufacturer);
        Assert.Equal("AC-1234", firstItem.ManufacturerPartNumber);
        Assert.Equal("123456789012", firstItem.UniversalProductCode);
        Assert.Equal(15.99m, firstItem.UnitCost);
        Assert.Equal(InventoryItemUnitCostMeasurementUnitEnum.Unit, firstItem.UnitCostMeasurementUnit);
        Assert.Equal("PartsCo", firstItem.Supplier);
        Assert.Equal(0.5, firstItem.WeightKG);
        Assert.True(firstItem.IsActive);
        // Verify mocks
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetAllInventoryItemsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Return_Empty_Result_When_No_InventoryItems()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "NonExistent"
        };
        var query = new GetAllInventoryItemQuery(parameters);
        SetupValidValidation(query);
        var emptyPagedResult = new PagedResult<InventoryItem>
        {
            Items = [],
            TotalCount = 0,
            PageNumber = 1,
            PageSize = 10
        };
        _mockInventoryItemRepository.Setup(r => r.GetAllInventoryItemsPagedAsync(parameters))
            .ReturnsAsync(emptyPagedResult);
        // When
        var result = await _getAllInventoryItemQueryHandler.Handle(query, CancellationToken.None);
        // Then
        Assert.NotNull(result);
        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalCount);
        Assert.Equal(1, result.PageNumber);
        Assert.Equal(10, result.PageSize);
        Assert.Equal(0, result.TotalPages);
        Assert.False(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetAllInventoryItemsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Different_Page_Sizes()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 3
        };
        var query = new GetAllInventoryItemQuery(parameters);
        SetupValidValidation(query);
        var pagedResult = new PagedResult<InventoryItem>
        {
            Items = [],
            TotalCount = 10,
            PageNumber = 2,
            PageSize = 3
        };
        _mockInventoryItemRepository.Setup(r => r.GetAllInventoryItemsPagedAsync(parameters))
            .ReturnsAsync(pagedResult);
        // When
        var result = await _getAllInventoryItemQueryHandler.Handle(query, CancellationToken.None);
        // Then
        Assert.Equal(10, result.TotalCount);
        Assert.Equal(2, result.PageNumber);
        Assert.Equal(3, result.PageSize);
        Assert.Equal(4, result.TotalPages); // 10 / 3 = 4 pages
        Assert.True(result.HasPreviousPage);
        Assert.True(result.HasNextPage);
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetAllInventoryItemsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Last_Page()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 3,
            PageSize = 5
        };
        var query = new GetAllInventoryItemQuery(parameters);
        SetupValidValidation(query);
        var pagedResult = new PagedResult<InventoryItem>
        {
            Items = [],
            TotalCount = 12,
            PageNumber = 3,
            PageSize = 5
        };
        _mockInventoryItemRepository.Setup(r => r.GetAllInventoryItemsPagedAsync(parameters))
            .ReturnsAsync(pagedResult);
        // When
        var result = await _getAllInventoryItemQueryHandler.Handle(query, CancellationToken.None);
        // Then
        Assert.Equal(12, result.TotalCount);
        Assert.Equal(3, result.PageNumber);
        Assert.Equal(5, result.PageSize);
        Assert.Equal(3, result.TotalPages);
        Assert.True(result.HasPreviousPage);
        Assert.False(result.HasNextPage);
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetAllInventoryItemsPagedAsync(parameters), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var parameters = new PaginationParameters
        {
            PageNumber = 0,
            PageSize = 10
        };
        var query = new GetAllInventoryItemQuery(parameters);
        SetupInvalidValidation(query, "Parameters.PageNumber", "Page number must be greater than 0");
        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _getAllInventoryItemQueryHandler.Handle(query, CancellationToken.None)
        );
        _mockValidator.Verify(v => v.Validate(query), Times.Once);
        _mockInventoryItemRepository.Verify(r => r.GetAllInventoryItemsPagedAsync(It.IsAny<PaginationParameters>()), Times.Never);
    }


}