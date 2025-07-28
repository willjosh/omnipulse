using Application.Features.InventoryItems.Command.CreateInventoryItem;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.InventoryItems;

[Trait("Entity", "Inventory Item")]
public class CreateInventoryItemIntegrationTests : BaseIntegrationTest
{
    public CreateInventoryItemIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddInventoryItemToDatabase_When_CommandIsValid()
    {
        // Arrange
        var createCommand = new CreateInventoryItemCommand(
            ItemNumber: $"ITEM-{Faker.Random.AlphaNumeric(5)}",
            ItemName: $"Test {nameof(InventoryItem)} Name {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test {nameof(InventoryItem)} Description {Faker.Random.AlphaNumeric(5)}",
            Category: InventoryItemCategoryEnum.ENGINE,
            Manufacturer: "Test Manufacturer",
            ManufacturerPartNumber: $"MPN{Faker.Random.AlphaNumeric(5)}",
            UniversalProductCode: "123456789012",
            UnitCost: 25.50m,
            UnitCostMeasurementUnit: InventoryItemUnitCostMeasurementUnitEnum.Unit,
            Supplier: "Test Supplier",
            WeightKG: 1.5,
            IsActive: true
        );

        // Act
        int createdInventoryItemId = await Sender.Send(createCommand);

        // Assert
        InventoryItem? createdInventoryItemEntity = await DbContext.InventoryItems.FindAsync(createdInventoryItemId);
        createdInventoryItemEntity.Should().NotBeNull();

        createdInventoryItemEntity.ItemNumber.Should().Be(createCommand.ItemNumber);
        createdInventoryItemEntity.ItemName.Should().Be(createCommand.ItemName);
        createdInventoryItemEntity.Description.Should().Be(createCommand.Description);
        createdInventoryItemEntity.Category.Should().Be(createCommand.Category);
        createdInventoryItemEntity.Manufacturer.Should().Be(createCommand.Manufacturer);
        createdInventoryItemEntity.ManufacturerPartNumber.Should().Be(createCommand.ManufacturerPartNumber);
        createdInventoryItemEntity.UniversalProductCode.Should().Be(createCommand.UniversalProductCode);
        createdInventoryItemEntity.UnitCost.Should().Be(createCommand.UnitCost);
        createdInventoryItemEntity.UnitCostMeasurementUnit.Should().Be(createCommand.UnitCostMeasurementUnit);
        createdInventoryItemEntity.Supplier.Should().Be(createCommand.Supplier);
        createdInventoryItemEntity.WeightKG.Should().Be(createCommand.WeightKG);
        createdInventoryItemEntity.IsActive.Should().Be(createCommand.IsActive);
    }
}