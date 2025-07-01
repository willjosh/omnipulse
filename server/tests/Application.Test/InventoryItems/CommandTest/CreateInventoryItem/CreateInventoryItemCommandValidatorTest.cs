using System;
using Application.Features.InventoryItems.Command.CreateInventoryItem;
using Domain.Entities.Enums;
using Xunit;

namespace Application.Test.InventoryItems.CommandTest.CreateInventoryItem;

public class CreateInventoryItemCommandValidatorTest
{
    private readonly CreateInventoryItemCommandValidator _validator;

    public CreateInventoryItemCommandValidatorTest()
    {
        _validator = new CreateInventoryItemCommandValidator();
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
}
