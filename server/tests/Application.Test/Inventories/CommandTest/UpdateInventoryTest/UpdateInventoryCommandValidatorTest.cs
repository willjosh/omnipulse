using System;
using System.Threading.Tasks;

using Application.Features.Inventory.Command.UpdateInventory;

using FluentValidation;

using Xunit;

namespace Application.Test.Inventories.CommandTest.UpdateInventoryTest;

public class UpdateInventoryCommandValidatorTest
{
    private readonly UpdateInventoryCommandValidator _validator;

    public UpdateInventoryCommandValidatorTest()
    {
        _validator = new UpdateInventoryCommandValidator();
    }

    private UpdateInventoryCommand CreateValidCommand(
        int inventoryID = 1,
        int quantityOnHand = 100,
        decimal unitCost = 25.99m,
        int minStockLevel = 10,
        int maxStockLevel = 200,
        bool isAdjustment = false,
        Guid? performedByUserID = null)
    {
        return new UpdateInventoryCommand(
            inventoryID,
            quantityOnHand,
            unitCost,
            minStockLevel,
            maxStockLevel,
            isAdjustment,
            (performedByUserID ?? Guid.NewGuid()).ToString() // Convert the entire expression to string
        );
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Command()
    {
        // Arrange
        var command = CreateValidCommand();

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-999)]
    public async Task Validator_Should_Fail_When_InventoryID_Is_Invalid(int invalidInventoryID)
    {
        // Arrange
        var command = CreateValidCommand(inventoryID: invalidInventoryID);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "InventoryID" && e.ErrorMessage == "Inventory ID is required");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-999)]
    public async Task Validator_Should_Fail_When_QuantityOnHand_Is_Negative(int invalidQuantity)
    {
        // Arrange
        var command = CreateValidCommand(quantityOnHand: invalidQuantity);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "QuantityOnHand" && e.ErrorMessage == "Quantity on hand must be zero or greater");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_QuantityOnHand_Is_Zero()
    {
        // Arrange
        var command = CreateValidCommand(quantityOnHand: 0);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.50)]
    public async Task Validator_Should_Fail_When_UnitCost_Is_Zero_Or_Negative(decimal invalidUnitCost)
    {
        // Arrange
        var command = CreateValidCommand(unitCost: invalidUnitCost);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "UnitCost" && e.ErrorMessage == "Unit cost must be greater than zero");
    }

    [Theory]
    [InlineData(-1)]
    [InlineData(-10)]
    [InlineData(-999)]
    public async Task Validator_Should_Fail_When_MinStockLevel_Is_Negative(int invalidMinStock)
    {
        // Arrange
        var command = CreateValidCommand(minStockLevel: invalidMinStock);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MinStockLevel" && e.ErrorMessage == "Minimum stock level must be zero or greater");
    }

    [Fact]
    public async Task Validator_Should_Pass_When_MinStockLevel_Is_Zero()
    {
        // Arrange
        var command = CreateValidCommand(minStockLevel: 0);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(10, 5)]   // Max < Min
    [InlineData(50, 25)]  // Max < Min
    [InlineData(100, 99)] // Max < Min
    public async Task Validator_Should_Fail_When_MaxStockLevel_Is_Less_Than_MinStockLevel(int minStock, int maxStock)
    {
        // Arrange
        var command = CreateValidCommand(minStockLevel: minStock, maxStockLevel: maxStock);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "MaxStockLevel" && e.ErrorMessage == "Maximum stock level must be greater than or equal to minimum stock level");
    }

    [Theory]
    [InlineData(10, 10)]   // Max = Min (valid)
    [InlineData(25, 50)]   // Max > Min (valid)
    [InlineData(0, 100)]   // Max > Min (valid)
    public async Task Validator_Should_Pass_When_MaxStockLevel_Is_Greater_Than_Or_Equal_To_MinStockLevel(int minStock, int maxStock)
    {
        // Arrange
        var command = CreateValidCommand(minStockLevel: minStock, maxStockLevel: maxStock);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Validator_Should_Pass_With_Valid_IsAdjustment_Values(bool isAdjustment)
    {
        // Arrange
        var command = CreateValidCommand(isAdjustment: isAdjustment);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Fail_When_PerformedByUserID_Is_Empty()
    {
        // Given
        var command = new UpdateInventoryCommand(
            InventoryID: 1,
            QuantityOnHand: 100,
            UnitCost: 25.99m,
            MinStockLevel: 10,
            MaxStockLevel: 200,
            IsAdjustment: false,
            PerformedByUserID: string.Empty  // Use string.Empty instead of Guid.Empty.ToString()
        );

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == "PerformedByUserID" && e.ErrorMessage == "Performed by user ID is required");
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_PerformedByUserID()
    {
        // Arrange
        var validGuid = Guid.NewGuid();
        var command = CreateValidCommand(performedByUserID: validGuid);

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task Validator_Should_Handle_Multiple_Validation_Errors()
    {
        // Given
        var command = new UpdateInventoryCommand(
            InventoryID: -1,           // Invalid
            QuantityOnHand: -5,        // Invalid
            UnitCost: 0,               // Invalid
            MinStockLevel: -10,        // Invalid
            MaxStockLevel: 5,          // Invalid (less than MinStockLevel after fixing it)
            IsAdjustment: false,       // Valid
            PerformedByUserID: string.Empty  // Invalid - use string.Empty instead
        );

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count >= 4); // Should have multiple errors

        // Verify specific errors exist
        Assert.Contains(result.Errors, e => e.PropertyName == "InventoryID");
        Assert.Contains(result.Errors, e => e.PropertyName == "QuantityOnHand");
        Assert.Contains(result.Errors, e => e.PropertyName == "UnitCost");
        Assert.Contains(result.Errors, e => e.PropertyName == "MinStockLevel");
        Assert.Contains(result.Errors, e => e.PropertyName == "PerformedByUserID");
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Realistic_Inventory_Values()
    {
        // Arrange
        var command = new UpdateInventoryCommand(
            InventoryID: 42,
            QuantityOnHand: 150,
            UnitCost: 89.99m,
            MinStockLevel: 25,
            MaxStockLevel: 500,
            IsAdjustment: true,
            PerformedByUserID: Guid.NewGuid().ToString()
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Edge_Case_Values()
    {
        // Arrange
        var command = new UpdateInventoryCommand(
            InventoryID: 1,
            QuantityOnHand: 0,         // Edge case: zero quantity
            UnitCost: 0.01m,           // Edge case: very small cost
            MinStockLevel: 0,          // Edge case: zero min stock
            MaxStockLevel: 0,          // Edge case: max equals min
            IsAdjustment: false,
            PerformedByUserID: Guid.NewGuid().ToString()
        );

        // Act
        var result = await _validator.ValidateAsync(command);

        // Assert
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}