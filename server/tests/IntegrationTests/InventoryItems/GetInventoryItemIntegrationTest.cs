using Application.Exceptions;
using Application.Features.InventoryItems.Query.GetInventoryItem;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.InventoryItems;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(InventoryItem))]
public class GetInventoryItemIntegrationTest : BaseIntegrationTest
{
    public GetInventoryItemIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnInventoryItemDetail_When_ItemExists()
    {
        // Arrange
        var itemId = await CreateInventoryItemAsync(
            itemNumber: "TEST-001",
            itemName: "Test Engine Part",
            unitCost: 125.50m,
            category: InventoryItemCategoryEnum.ENGINE,
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit
        );

        var query = new GetInventoryItemQuery(itemId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.ID.Should().Be(itemId);
        result.ItemNumber.Should().Be("TEST-001");
        result.ItemName.Should().Be("Test Engine Part");
        result.UnitCost.Should().Be(125.50m);
        result.Category.Should().Be(InventoryItemCategoryEnum.ENGINE);
        result.UnitCostMeasurementUnit.Should().Be(InventoryItemUnitCostMeasurementUnitEnum.Unit);
        result.IsActive.Should().BeTrue();

        // Verify all required properties are populated
        result.Description.Should().NotBeNullOrEmpty();
        result.Manufacturer.Should().NotBeNullOrEmpty();
        result.ManufacturerPartNumber.Should().NotBeNullOrEmpty();
        result.UniversalProductCode.Should().NotBeNullOrEmpty();
        result.Supplier.Should().NotBeNullOrEmpty();
        result.WeightKG.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(InventoryItemCategoryEnum.ENGINE)]
    [InlineData(InventoryItemCategoryEnum.BRAKES)]
    [InlineData(InventoryItemCategoryEnum.FILTERS)]
    [InlineData(InventoryItemCategoryEnum.TRANSMISSION)]
    [InlineData(InventoryItemCategoryEnum.ELECTRICAL)]
    [InlineData(InventoryItemCategoryEnum.TIRES)]
    [InlineData(InventoryItemCategoryEnum.FLUIDS)]
    public async Task Should_ReturnInventoryItemWithCategory_When_ItemHasSpecificCategory(InventoryItemCategoryEnum category)
    {
        // Arrange
        var itemId = await CreateInventoryItemAsync(
            itemName: $"{category} Test Item",
            category: category
        );

        var query = new GetInventoryItemQuery(itemId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.ID.Should().Be(itemId);
        result.Category.Should().Be(category);
        result.ItemName.Should().Be($"{category} Test Item");
    }

    [Theory]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Unit)]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Litre)]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Box)]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Kilogram)]
    public async Task Should_ReturnInventoryItemWithUnitMeasurement_When_ItemHasSpecificUnit(InventoryItemUnitCostMeasurementUnitEnum unitMeasurement)
    {
        // Arrange
        var itemId = await CreateInventoryItemAsync(
            itemName: $"Test Item Per {unitMeasurement}",
            unitCostUnit: unitMeasurement
        );

        var query = new GetInventoryItemQuery(itemId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.ID.Should().Be(itemId);
        result.UnitCostMeasurementUnit.Should().Be(unitMeasurement);
        result.ItemName.Should().Be($"Test Item Per {unitMeasurement}");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_ReturnInventoryItemWithCorrectActiveStatus_When_ItemHasSpecificActiveStatus(bool isActive)
    {
        // Arrange
        var itemId = await CreateInventoryItemAsync(
            itemName: isActive ? "Active Test Item" : "Inactive Test Item",
            isActive: isActive
        );

        var query = new GetInventoryItemQuery(itemId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.ID.Should().Be(itemId);
        result.IsActive.Should().Be(isActive);
        result.ItemName.Should().Be(isActive ? "Active Test Item" : "Inactive Test Item");
    }

    [Theory]
    [InlineData(0.1)]
    [InlineData(10.50)]
    [InlineData(100.00)]
    [InlineData(999.99)]
    [InlineData(1500.75)]
    public async Task Should_ReturnInventoryItemWithCorrectUnitCost_When_ItemHasSpecificCost(decimal unitCost)
    {
        // Arrange
        var itemId = await CreateInventoryItemAsync(
            itemName: $"Item Cost {unitCost:C}",
            unitCost: unitCost
        );

        var query = new GetInventoryItemQuery(itemId);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.ID.Should().Be(itemId);
        result.UnitCost.Should().Be(unitCost);
        result.ItemName.Should().Be($"Item Cost {unitCost:C}");
    }

    [Fact]
    public async Task Should_ReturnInventoryItemWithUniqueProperties_When_MultipleItemsExist()
    {
        // Arrange - Create multiple items to ensure we get the right one
        var item1Id = await CreateInventoryItemAsync(
            itemNumber: "UNIQUE-001",
            itemName: "First Unique Item",
            unitCost: 25.00m,
            category: InventoryItemCategoryEnum.ENGINE
        );

        var item2Id = await CreateInventoryItemAsync(
            itemNumber: "UNIQUE-002",
            itemName: "Second Unique Item",
            unitCost: 50.00m,
            category: InventoryItemCategoryEnum.BRAKES
        );

        var item3Id = await CreateInventoryItemAsync(
            itemNumber: "UNIQUE-003",
            itemName: "Third Unique Item",
            unitCost: 75.00m,
            category: InventoryItemCategoryEnum.FILTERS
        );

        // Act - Get the second item
        var query = new GetInventoryItemQuery(item2Id);
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.ID.Should().Be(item2Id);
        result.ItemNumber.Should().Be("UNIQUE-002");
        result.ItemName.Should().Be("Second Unique Item");
        result.UnitCost.Should().Be(50.00m);
        result.Category.Should().Be(InventoryItemCategoryEnum.BRAKES);

        // Verify it's not the other items
        result.ID.Should().NotBe(item1Id);
        result.ID.Should().NotBe(item3Id);
        result.ItemNumber.Should().NotBe("UNIQUE-001");
        result.ItemNumber.Should().NotBe("UNIQUE-003");
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Should_ThrowEntityNotFoundException_When_ItemIdIsInvalid(int invalidId)
    {
        // Arrange
        var query = new GetInventoryItemQuery(invalidId);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(
            () => Sender.Send(query)
        );

        exception.Message.Should().Contain(invalidId.ToString());
        exception.Message.Should().Contain(nameof(InventoryItem));
    }

    [Fact]
    public async Task Should_ReturnInventoryItemWithCompleteDetails_When_ItemHasAllProperties()
    {
        // Arrange - Create an item with all possible details
        var itemId = await CreateInventoryItemAsync(
            itemNumber: "COMPLETE-001",
            itemName: "Complete Test Item",
            unitCost: 199.99m,
            category: InventoryItemCategoryEnum.ENGINE,
            unitCostUnit: InventoryItemUnitCostMeasurementUnitEnum.Litre,
            isActive: true
        );

        var query = new GetInventoryItemQuery(itemId);

        // Act
        var result = await Sender.Send(query);

        // Assert - Verify all properties are correctly mapped
        result.Should().NotBeNull();
        result.ID.Should().Be(itemId);
        result.ItemNumber.Should().Be("COMPLETE-001");
        result.ItemName.Should().Be("Complete Test Item");
        result.UnitCost.Should().Be(199.99m);
        result.Category.Should().Be(InventoryItemCategoryEnum.ENGINE);
        result.UnitCostMeasurementUnit.Should().Be(InventoryItemUnitCostMeasurementUnitEnum.Litre);
        result.IsActive.Should().BeTrue();

        // Verify required string properties are not null or empty
        result.Description.Should().NotBeNullOrWhiteSpace();
        result.Manufacturer.Should().NotBeNullOrWhiteSpace();
        result.ManufacturerPartNumber.Should().NotBeNullOrWhiteSpace();
        result.UniversalProductCode.Should().NotBeNullOrWhiteSpace();
        result.Supplier.Should().NotBeNullOrWhiteSpace();

        // Verify numeric properties are valid
        result.WeightKG.Should().BeGreaterThan(0);

    }

    [Fact]
    public async Task Should_ReturnConsistentData_When_CalledMultipleTimes()
    {
        // Arrange
        var itemId = await CreateInventoryItemAsync(
            itemNumber: "CONSISTENT-001",
            itemName: "Consistency Test Item",
            unitCost: 89.99m
        );

        var query = new GetInventoryItemQuery(itemId);

        // Act - Call multiple times
        var result1 = await Sender.Send(query);
        var result2 = await Sender.Send(query);
        var result3 = await Sender.Send(query);

        // Assert - All results should be identical
        result1.Should().BeEquivalentTo(result2);
        result2.Should().BeEquivalentTo(result3);
        result1.Should().BeEquivalentTo(result3);

        // Verify specific properties are consistent
        result1.ID.Should().Be(result2.ID).And.Be(result3.ID);
        result1.ItemNumber.Should().Be(result2.ItemNumber).And.Be(result3.ItemNumber);
        result1.UnitCost.Should().Be(result2.UnitCost).And.Be(result3.UnitCost);
    }

    [Fact]
    public async Task Should_ReturnDifferentItems_When_DifferentIdsProvided()
    {
        // Arrange - Create two different items
        var item1Id = await CreateInventoryItemAsync(
            itemNumber: "DIFF-001",
            itemName: "First Different Item",
            unitCost: 10.00m,
            category: InventoryItemCategoryEnum.ENGINE
        );

        var item2Id = await CreateInventoryItemAsync(
            itemNumber: "DIFF-002",
            itemName: "Second Different Item",
            unitCost: 20.00m,
            category: InventoryItemCategoryEnum.BRAKES
        );

        var query1 = new GetInventoryItemQuery(item1Id);
        var query2 = new GetInventoryItemQuery(item2Id);

        // Act
        var result1 = await Sender.Send(query1);
        var result2 = await Sender.Send(query2);

        // Assert - Results should be different
        result1.Should().NotBeEquivalentTo(result2);
        result1.ID.Should().NotBe(result2.ID);
        result1.ItemNumber.Should().NotBe(result2.ItemNumber);
        result1.ItemName.Should().NotBe(result2.ItemName);
        result1.UnitCost.Should().NotBe(result2.UnitCost);
        result1.Category.Should().NotBe(result2.Category);

        // Verify each result matches its respective item
        result1.ID.Should().Be(item1Id);
        result1.ItemNumber.Should().Be("DIFF-001");
        result1.ItemName.Should().Be("First Different Item");

        result2.ID.Should().Be(item2Id);
        result2.ItemNumber.Should().Be("DIFF-002");
        result2.ItemName.Should().Be("Second Different Item");
    }
}