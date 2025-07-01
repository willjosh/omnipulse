using System;
using Application.Features.InventoryItems.Command.UpdateInventoryItem;
using Domain.Entities.Enums;
using Xunit;

namespace Application.Test.InventoryItems.CommandTest.UpdateInventoryItem;

public class UpdateInventoryItemCommandValidatorTest
{
    private readonly UpdateInventoryItemCommandValidator _validator;

    public UpdateInventoryItemCommandValidatorTest()
    {
        _validator = new UpdateInventoryItemCommandValidator();
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

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Given
        var command = CreateValidCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_InventoryItemID_Is_Invalid(int invalidId)
    {
        // Given
        var command = CreateValidCommand(inventoryItemID: invalidId);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "InventoryItemID");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_ItemNumber_Is_Empty(string invalidItemNumber)
    {
        // Given
        var command = CreateValidCommand(itemNumber: invalidItemNumber);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ItemNumber");
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_ItemName_Is_Empty(string invalidItemName)
    {
        // Given
        var command = CreateValidCommand(itemName: invalidItemName);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ItemName");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ItemNumber_Exceeds_MaxLength()
    {
        // Given
        var longItemNumber = new string('A', 251); // 251 characters, exceeds 250 limit
        var command = CreateValidCommand(itemNumber: longItemNumber);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ItemNumber");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ItemName_Exceeds_MaxLength()
    {
        // Given
        var longItemName = new string('A', 251); // 251 characters, exceeds 250 limit
        var command = CreateValidCommand(itemName: longItemName);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ItemName");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength()
    {
        // Given
        var longDescription = new string('A', 501); // 501 characters, exceeds 500 limit
        var command = CreateValidCommand(description: longDescription);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Description");
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Null_Optional_Fields()
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

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_UnitCost_Is_Negative()
    {
        // Given
        var command = CreateValidCommand(unitCost: -10.00m);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UnitCost");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_WeightKG_Is_Negative()
    {
        // Given
        var command = CreateValidCommand(weightKG: -5.0);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "WeightKG");
    }

    [Theory]
    [InlineData("12345678901")] // 11 characters
    [InlineData("1234567890123")] // 13 characters
    public async Task Validator_Should_Fail_When_UniversalProductCode_Is_Not_Exactly_12_Characters(string invalidUpc)
    {
        // Given
        var command = CreateValidCommand(universalProductCode: invalidUpc);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UniversalProductCode");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_UniversalProductCode_Is_Exactly_12_Characters()
    {
        // Given
        var command = CreateValidCommand(universalProductCode: "123456789012");

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(InventoryItemCategoryEnum.ENGINE)]
    [InlineData(InventoryItemCategoryEnum.TRANSMISSION)]
    [InlineData(InventoryItemCategoryEnum.BRAKES)]
    [InlineData(InventoryItemCategoryEnum.TIRES)]
    [InlineData(InventoryItemCategoryEnum.ELECTRICAL)]
    [InlineData(InventoryItemCategoryEnum.BODY)]
    [InlineData(InventoryItemCategoryEnum.INTERIOR)]
    [InlineData(InventoryItemCategoryEnum.FLUIDS)]
    [InlineData(InventoryItemCategoryEnum.FILTERS)]
    public async Task Validator_Should_Pass_With_All_Valid_Categories(InventoryItemCategoryEnum category)
    {
        // Given
        var command = CreateValidCommand(category: category);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Unit)]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Litre)]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Kilogram)]
    [InlineData(InventoryItemUnitCostMeasurementUnitEnum.Box)]
    public async Task Validator_Should_Pass_With_All_Valid_MeasurementUnits(InventoryItemUnitCostMeasurementUnitEnum unit)
    {
        // Given
        var command = CreateValidCommand(unitCostMeasurementUnit: unit);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Manufacturer_Exceeds_MaxLength()
    {
        // Given
        var longManufacturer = new string('A', 101); // 101 characters, exceeds 100 limit
        var command = CreateValidCommand(manufacturer: longManufacturer);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Manufacturer");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ManufacturerPartNumber_Exceeds_MaxLength()
    {
        // Given
        var longPartNumber = new string('A', 101); // 101 characters, exceeds 100 limit
        var command = CreateValidCommand(manufacturerPartNumber: longPartNumber);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "ManufacturerPartNumber");
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Supplier_Exceeds_MaxLength()
    {
        // Given
        var longSupplier = new string('A', 101); // 101 characters, exceeds 100 limit
        var command = CreateValidCommand(supplier: longSupplier);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "Supplier");
    }
}