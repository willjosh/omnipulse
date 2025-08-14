using Application.Features.InventoryItems.Query.GetAllInventoryItem;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.InventoryItems;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(InventoryItem))]
public class GetAllInventoryItemIntegrationTests : BaseIntegrationTest
{
    public GetAllInventoryItemIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnPagedInventoryItems_When_QueryIsValid()
    {
        // Arrange - Create multiple inventory items with different properties
        var itemId1 = await CreateInventoryItemAsync(
            itemNumber: "ITEM-001",
            itemName: "Oil Filter",
            unitCost: 15.99m,
            category: InventoryItemCategoryEnum.FILTERS,
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit
        );

        var itemId2 = await CreateInventoryItemAsync(
            itemNumber: "ITEM-002",
            itemName: "Engine Oil",
            unitCost: 25.50m,
            category: InventoryItemCategoryEnum.ENGINE,
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Litre
        );

        var itemId3 = await CreateInventoryItemAsync(
            itemNumber: "ITEM-003",
            itemName: "Brake Pad",
            unitCost: 45.00m,
            category: InventoryItemCategoryEnum.BRAKES,
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit
        );

        var query = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = "itemname",
            SortDescending = false
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThanOrEqualTo(3);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);

        // Verify the created items are in the result
        var createdItems = result.Items.Where(x =>
            x.ID == itemId1 || x.ID == itemId2 || x.ID == itemId3).ToList();

        createdItems.Should().HaveCount(3);

        // Verify Oil Filter item
        var oilFilter = createdItems.First(x => x.ID == itemId1);
        oilFilter.ItemNumber.Should().Be("ITEM-001");
        oilFilter.ItemName.Should().Be("Oil Filter");
        oilFilter.UnitCost.Should().Be(15.99m);
        oilFilter.Category.Should().Be(InventoryItemCategoryEnum.FILTERS);
        oilFilter.UnitCostMeasurementUnit.Should().Be(InventoryItemUnitCostMeasurementUnitEnum.Unit);
        oilFilter.IsActive.Should().BeTrue();

        // Verify Engine Oil item
        var engineOil = createdItems.First(x => x.ID == itemId2);
        engineOil.ItemNumber.Should().Be("ITEM-002");
        engineOil.ItemName.Should().Be("Engine Oil");
        engineOil.UnitCost.Should().Be(25.50m);
        engineOil.Category.Should().Be(InventoryItemCategoryEnum.ENGINE);
        engineOil.UnitCostMeasurementUnit.Should().Be(InventoryItemUnitCostMeasurementUnitEnum.Litre);
    }

    [Fact]
    public async Task Should_ReturnEmptyResult_When_NoInventoryItemsExist()
    {
        // Arrange - Clear existing inventory items
        var existingItems = await DbContext.InventoryItems.ToListAsync();
        DbContext.InventoryItems.RemoveRange(existingItems);
        await DbContext.SaveChangesAsync();

        var query = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Should_HandlePagination_When_MultiplePages()
    {
        // Arrange - Create 15 inventory items
        var itemIds = new List<int>();
        for (int i = 1; i <= 15; i++)
        {
            var itemId = await CreateInventoryItemAsync(
                itemNumber: $"BULK-{i:D3}",
                itemName: $"Bulk Item {i}",
                unitCost: i * 10.00m,
                category: InventoryItemCategoryEnum.ENGINE
            );
            itemIds.Add(itemId);
        }

        // Act - Get first page (5 items per page)
        var firstPageQuery = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            SortBy = "itemnumber",
            SortDescending = false
        });

        var firstPageResult = await Sender.Send(firstPageQuery);

        // Act - Get second page
        var secondPageQuery = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 5,
            SortBy = "itemnumber",
            SortDescending = false
        });

        var secondPageResult = await Sender.Send(secondPageQuery);

        // Assert First Page
        firstPageResult.Should().NotBeNull();
        firstPageResult.Items.Should().HaveCount(5);
        firstPageResult.PageNumber.Should().Be(1);
        firstPageResult.PageSize.Should().Be(5);
        firstPageResult.TotalCount.Should().BeGreaterThanOrEqualTo(15);
        firstPageResult.HasPreviousPage.Should().BeFalse();
        firstPageResult.HasNextPage.Should().BeTrue();

        // Assert Second Page
        secondPageResult.Should().NotBeNull();
        secondPageResult.Items.Should().HaveCount(5);
        secondPageResult.PageNumber.Should().Be(2);
        secondPageResult.PageSize.Should().Be(5);
        secondPageResult.HasPreviousPage.Should().BeTrue();

        // Verify no duplicate items between pages
        var firstPageIds = firstPageResult.Items.Select(x => x.ID).ToList();
        var secondPageIds = secondPageResult.Items.Select(x => x.ID).ToList();
        firstPageIds.Should().NotIntersectWith(secondPageIds);
    }

    [Fact]
    public async Task Should_FilterBySearch_When_SearchParameterProvided()
    {
        // Arrange
        var engineItemId = await CreateInventoryItemAsync(
            itemNumber: "ENGINE-001",
            itemName: "Engine Component",
            category: InventoryItemCategoryEnum.ENGINE
        );

        var brakeItemId = await CreateInventoryItemAsync(
            itemNumber: "BRAKE-001",
            itemName: "Brake Component",
            category: InventoryItemCategoryEnum.BRAKES
        );

        var filterItemId = await CreateInventoryItemAsync(
            itemNumber: "FILTER-001",
            itemName: "Filter Component",
            category: InventoryItemCategoryEnum.FILTERS
        );

        // Act - Search for "engine"
        var engineSearchQuery = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "engine"
        });

        var engineSearchResult = await Sender.Send(engineSearchQuery);

        // Act - Search for "brake"
        var brakeSearchQuery = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "brake"
        });

        var brakeSearchResult = await Sender.Send(brakeSearchQuery);

        // Assert Engine Search
        engineSearchResult.Should().NotBeNull();
        engineSearchResult.Items.Should().Contain(x => x.ID == engineItemId);
        engineSearchResult.Items.Should().NotContain(x => x.ID == brakeItemId || x.ID == filterItemId);

        // Assert Brake Search
        brakeSearchResult.Should().NotBeNull();
        brakeSearchResult.Items.Should().Contain(x => x.ID == brakeItemId);
        brakeSearchResult.Items.Should().NotContain(x => x.ID == engineItemId || x.ID == filterItemId);
    }

    [Theory]
    [InlineData("itemname", false)]
    [InlineData("itemname", true)]
    [InlineData("unitcost", false)]
    [InlineData("unitcost", true)]
    [InlineData("category", false)]
    [InlineData("category", true)]
    public async Task Should_SortCorrectly_When_SortParametersProvided(string sortBy, bool sortDescending)
    {
        // Arrange
        await CreateInventoryItemAsync(
            itemNumber: "SORT-001",
            itemName: "Alpha Item",
            unitCost: 10.00m,
            category: InventoryItemCategoryEnum.ENGINE
        );

        await CreateInventoryItemAsync(
            itemNumber: "SORT-002",
            itemName: "Beta Item",
            unitCost: 20.00m,
            category: InventoryItemCategoryEnum.BRAKES
        );

        await CreateInventoryItemAsync(
            itemNumber: "SORT-003",
            itemName: "Charlie Item",
            unitCost: 15.00m,
            category: InventoryItemCategoryEnum.FILTERS
        );

        var query = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = sortBy,
            SortDescending = sortDescending
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThanOrEqualTo(3);

        // Verify sorting based on the sort field
        var sortedItems = result.Items.Where(x => x.ItemNumber.StartsWith("SORT-")).ToList();
        sortedItems.Should().HaveCount(3);

        switch (sortBy.ToLowerInvariant())
        {
            case "itemname":
                if (sortDescending)
                {
                    sortedItems.Should().BeInDescendingOrder(x => x.ItemName);
                }
                else
                {
                    sortedItems.Should().BeInAscendingOrder(x => x.ItemName);
                }
                break;

            case "unitcost":
                if (sortDescending)
                {
                    sortedItems.Should().BeInDescendingOrder(x => x.UnitCost);
                }
                else
                {
                    sortedItems.Should().BeInAscendingOrder(x => x.UnitCost);
                }
                break;

            case "category":
                if (sortDescending)
                {
                    sortedItems.Should().BeInDescendingOrder(x => x.Category);
                }
                else
                {
                    sortedItems.Should().BeInAscendingOrder(x => x.Category);
                }
                break;
        }
    }

    [Fact]
    public async Task Should_HandleDifferentCategories_When_ItemsHaveVariousCategories()
    {
        // Arrange - Create items with all different categories
        var engineItemId = await CreateInventoryItemAsync(
            category: InventoryItemCategoryEnum.ENGINE,
            itemName: "Engine Part"
        );

        var brakeItemId = await CreateInventoryItemAsync(
            category: InventoryItemCategoryEnum.BRAKES,
            itemName: "Brake Part"
        );

        var filterItemId = await CreateInventoryItemAsync(
            category: InventoryItemCategoryEnum.FILTERS,
            itemName: "Filter Part"
        );

        var transmissionItemId = await CreateInventoryItemAsync(
            category: InventoryItemCategoryEnum.TRANSMISSION,
            itemName: "Transmission Part"
        );

        var electricalItemId = await CreateInventoryItemAsync(
            category: InventoryItemCategoryEnum.ELECTRICAL,
            itemName: "Electrical Part"
        );

        var query = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20,
            SortBy = "category",
            SortDescending = false
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThanOrEqualTo(5);

        var createdItems = result.Items.Where(x =>
            x.ID == engineItemId || x.ID == brakeItemId || x.ID == filterItemId ||
            x.ID == transmissionItemId || x.ID == electricalItemId).ToList();

        createdItems.Should().HaveCount(5);

        // Verify each category exists
        createdItems.Should().Contain(x => x.Category == InventoryItemCategoryEnum.ENGINE);
        createdItems.Should().Contain(x => x.Category == InventoryItemCategoryEnum.BRAKES);
        createdItems.Should().Contain(x => x.Category == InventoryItemCategoryEnum.FILTERS);
        createdItems.Should().Contain(x => x.Category == InventoryItemCategoryEnum.TRANSMISSION);
        createdItems.Should().Contain(x => x.Category == InventoryItemCategoryEnum.ELECTRICAL);
    }

    [Fact]
    public async Task Should_HandleDifferentUnitCostMeasurements_When_ItemsHaveVariousUnits()
    {
        // Arrange
        var unitItemId = await CreateInventoryItemAsync(
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
            itemName: "Per Unit Item"
        );

        var litreItemId = await CreateInventoryItemAsync(
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Litre,
            itemName: "Per Litre Item"
        );

        var boxItemId = await CreateInventoryItemAsync(
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Box,
            itemName: "Per Box Item"
        );

        var kilogramItemId = await CreateInventoryItemAsync(
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Kilogram,
            itemName: "Per Kilogram Item"
        );

        var query = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdItems = result.Items.Where(x =>
            x.ID == unitItemId || x.ID == litreItemId ||
            x.ID == boxItemId || x.ID == kilogramItemId).ToList();

        createdItems.Should().HaveCount(4);

        // Verify each unit measurement exists
        createdItems.Should().Contain(x => x.UnitCostMeasurementUnit == InventoryItemUnitCostMeasurementUnitEnum.Unit);
        createdItems.Should().Contain(x => x.UnitCostMeasurementUnit == InventoryItemUnitCostMeasurementUnitEnum.Litre);
        createdItems.Should().Contain(x => x.UnitCostMeasurementUnit == InventoryItemUnitCostMeasurementUnitEnum.Box);
        createdItems.Should().Contain(x => x.UnitCostMeasurementUnit == InventoryItemUnitCostMeasurementUnitEnum.Kilogram);
    }

    [Fact]
    public async Task Should_HandleActiveAndInactiveItems_When_IsActiveVaries()
    {
        // Arrange
        var activeItemId = await CreateInventoryItemAsync(
            itemName: "Active Item",
            isActive: true
        );

        var inactiveItemId = await CreateInventoryItemAsync(
            itemName: "Inactive Item",
            isActive: false
        );

        var query = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdItems = result.Items.Where(x =>
            x.ID == activeItemId || x.ID == inactiveItemId).ToList();

        createdItems.Should().HaveCount(2);

        var activeItem = createdItems.First(x => x.ID == activeItemId);
        var inactiveItem = createdItems.First(x => x.ID == inactiveItemId);

        activeItem.IsActive.Should().BeTrue();
        inactiveItem.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Should_IncludeAllRequiredProperties_When_ReturningInventoryItems()
    {
        // Arrange
        var itemId = await CreateInventoryItemAsync(
            itemNumber: "PROP-001",
            itemName: "Property Test Item",
            unitCost: 99.99m,
            category: InventoryItemCategoryEnum.ENGINE,
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit
        );

        var query = new GetAllInventoryItemQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        var testItem = result.Items.First(x => x.ID == itemId);

        // Verify all required properties are populated
        testItem.ID.Should().Be(itemId);
        testItem.ItemNumber.Should().NotBeNullOrEmpty();
        testItem.ItemName.Should().NotBeNullOrEmpty();
        testItem.Description.Should().NotBeNullOrEmpty();
        testItem.Category.Should().BeDefined();
        testItem.Manufacturer.Should().NotBeNullOrEmpty();
        testItem.ManufacturerPartNumber.Should().NotBeNullOrEmpty();
        testItem.UniversalProductCode.Should().NotBeNullOrEmpty();
        testItem.UnitCost.Should().BeGreaterThan(0);
        testItem.UnitCostMeasurementUnit.Should().BeDefined();
        testItem.Supplier.Should().NotBeNullOrEmpty();
        testItem.WeightKG.Should().BeGreaterThan(0);
    }
}