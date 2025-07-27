using Application.Features.WorkOrderLineItem.Models;

using Domain.Entities.Enums;

namespace Application.Test.WorkOrderLineItems.Models;

public class CreateWorkOrderLineItemDTOValidatorTest
{
    private readonly CreateWorkOrderLineItemDTOValidator _validator;

    public CreateWorkOrderLineItemDTOValidatorTest()
    {
        _validator = new CreateWorkOrderLineItemDTOValidator();
    }

    private CreateWorkOrderLineItemDTO CreateValidPartsCommand(
        int workOrderID = 1,
        int? inventoryItemID = 1,
        int serviceTaskID = 1,
        string? assignedToUserID = "tech-001",
        LineItemTypeEnum itemType = LineItemTypeEnum.ITEM,
        string? description = "Test Parts Item",
        int quantity = 1,
        decimal? unitPrice = 25.99m,
        decimal? hourlyRate = null,
        double? laborHours = null)
    {
        return new CreateWorkOrderLineItemDTO
        {
            InventoryItemID = inventoryItemID,
            ServiceTaskID = serviceTaskID,
            AssignedToUserID = assignedToUserID,
            ItemType = itemType,
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice,
            HourlyRate = hourlyRate,
            LaborHours = laborHours
        };
    }

    private CreateWorkOrderLineItemDTO CreateValidItemCommand(
        int workOrderID = 1,
        int? inventoryItemID = null,
        int serviceTaskID = 2,
        string? assignedToUserID = "tech-002",
        LineItemTypeEnum itemType = LineItemTypeEnum.LABOR,
        string? description = "Test Labor Item",
        int quantity = 1,
        decimal? unitPrice = null,
        decimal? hourlyRate = 85.00m,
        double? laborHours = 2.5)
    {
        return new CreateWorkOrderLineItemDTO
        {
            InventoryItemID = inventoryItemID,
            ServiceTaskID = serviceTaskID,
            AssignedToUserID = assignedToUserID,
            ItemType = itemType,
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice,
            HourlyRate = hourlyRate,
            LaborHours = laborHours
        };
    }

    private CreateWorkOrderLineItemDTO CreateValidBothCommand(
        int workOrderID = 1,
        int? inventoryItemID = 3,
        int serviceTaskID = 3,
        string? assignedToUserID = "tech-003",
        LineItemTypeEnum itemType = LineItemTypeEnum.BOTH,
        string? description = "Test Both Item",
        int quantity = 2,
        decimal? unitPrice = 50.00m,
        decimal? hourlyRate = 85.00m,
        double? laborHours = 1.5)
    {
        return new CreateWorkOrderLineItemDTO
        {
            InventoryItemID = inventoryItemID,
            ServiceTaskID = serviceTaskID,
            AssignedToUserID = assignedToUserID,
            ItemType = itemType,
            Description = description,
            Quantity = quantity,
            UnitPrice = unitPrice,
            HourlyRate = hourlyRate,
            LaborHours = laborHours
        };
    }

    // VALID COMMAND TESTS
    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Parts_Command()
    {
        // Given
        var command = CreateValidPartsCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Item_Command()
    {
        // Given
        var command = CreateValidItemCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_With_Valid_Both_Command()
    {
        // Given
        var command = CreateValidBothCommand();

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_ServiceTaskID_Is_Invalid(int invalidServiceTaskID)
    {
        // Given
        var command = CreateValidPartsCommand(serviceTaskID: invalidServiceTaskID);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.ServiceTaskID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_ItemType_Is_Invalid()
    {
        // Given
        var command = CreateValidPartsCommand(itemType: (LineItemTypeEnum)999);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.ItemType));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_Quantity_Is_Invalid(int invalidQuantity)
    {
        // Given
        var command = CreateValidPartsCommand(quantity: invalidQuantity);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.Quantity));
    }

    // OPTIONAL FIELDS VALIDATION TESTS
    [Fact]
    public async Task Validator_Should_Fail_When_Description_Exceeds_MaxLength()
    {
        // Given
        var command = CreateValidPartsCommand(description: new string('A', 501)); // 501 characters

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.Description));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public async Task Validator_Should_Fail_When_AssignedToUserID_Is_Empty_String(string emptyUserID)
    {
        // Given
        var command = CreateValidPartsCommand(assignedToUserID: emptyUserID);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.AssignedToUserID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_InventoryItemID_Is_Invalid(int invalidInventoryItemID)
    {
        // Given
        var command = CreateValidPartsCommand(inventoryItemID: invalidInventoryItemID);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.InventoryItemID));
    }

    // PRICING FIELDS VALIDATION TESTS
    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_UnitPrice_Is_Invalid(decimal invalidUnitPrice)
    {
        // Given
        var command = CreateValidPartsCommand(unitPrice: invalidUnitPrice);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.UnitPrice));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async Task Validator_Should_Fail_When_HourlyRate_Is_Invalid(decimal invalidHourlyRate)
    {
        // Given
        var command = CreateValidItemCommand(hourlyRate: invalidHourlyRate);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.HourlyRate));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(25)] // Over 24 hours
    public async Task Validator_Should_Fail_When_LaborHours_Is_Invalid(double invalidLaborHours)
    {
        // Given
        var command = CreateValidItemCommand(laborHours: invalidLaborHours);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.LaborHours));
    }

    // PARTS-ONLY VALIDATION TESTS
    [Fact]
    public async Task Validator_Should_Fail_When_Parts_Missing_InventoryItemID()
    {
        // Given
        var command = CreateValidPartsCommand(inventoryItemID: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.InventoryItemID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parts_Missing_UnitPrice()
    {
        // Given
        var command = CreateValidPartsCommand(unitPrice: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.UnitPrice));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parts_Missing_AssignedToUserID()
    {
        // Given
        var command = CreateValidPartsCommand(assignedToUserID: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.AssignedToUserID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parts_Has_HourlyRate()
    {
        // Given
        var command = CreateValidPartsCommand(hourlyRate: 85.00m);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.HourlyRate));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Parts_Has_LaborHours()
    {
        // Given
        var command = CreateValidPartsCommand(laborHours: 2.5);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.LaborHours));
    }

    // ITEM-ONLY VALIDATION TESTS
    [Fact]
    public async Task Validator_Should_Fail_When_Item_Missing_HourlyRate()
    {
        // Given
        var command = CreateValidItemCommand(hourlyRate: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.HourlyRate));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Item_Missing_LaborHours()
    {
        // Given
        var command = CreateValidItemCommand(laborHours: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.LaborHours));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Item_Missing_AssignedToUserID()
    {
        // Given
        var command = CreateValidItemCommand(assignedToUserID: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.AssignedToUserID));
    }

    [Theory]
    [InlineData(0)]
    [InlineData(2)]
    [InlineData(5)]
    public async Task Validator_Should_Fail_When_Item_Quantity_Is_Not_One(int invalidQuantity)
    {
        // Given
        var command = CreateValidItemCommand(quantity: invalidQuantity);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.Quantity));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Item_Has_InventoryItemID()
    {
        // Given
        var command = CreateValidItemCommand(inventoryItemID: 1);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.InventoryItemID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Item_Has_UnitPrice()
    {
        // Given
        var command = CreateValidItemCommand(unitPrice: 25.99m);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.UnitPrice));
    }

    // BOTH VALIDATION TESTS
    [Fact]
    public async Task Validator_Should_Fail_When_Both_Missing_InventoryItemID()
    {
        // Given
        var command = CreateValidBothCommand(inventoryItemID: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.InventoryItemID));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Both_Missing_UnitPrice()
    {
        // Given
        var command = CreateValidBothCommand(unitPrice: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.UnitPrice));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Both_Missing_HourlyRate()
    {
        // Given
        var command = CreateValidBothCommand(hourlyRate: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.HourlyRate));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Both_Missing_LaborHours()
    {
        // Given
        var command = CreateValidBothCommand(laborHours: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.LaborHours));
    }

    [Fact]
    public async Task Validator_Should_Fail_When_Both_Missing_AssignedToUserID()
    {
        // Given
        var command = CreateValidBothCommand(assignedToUserID: null);

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.PropertyName == nameof(CreateWorkOrderLineItemDTO.AssignedToUserID));
    }

    // CROSS-VALIDATION TESTS
    [Fact]
    public async Task Validator_Should_Fail_With_Invalid_Field_Combination_Parts_With_Labor()
    {
        // Given - Parts with labor fields (should fail cross-validation)
        var command = new CreateWorkOrderLineItemDTO
        {
            ServiceTaskID = 1,
            ItemType = LineItemTypeEnum.LABOR,
            Quantity = 1,
            InventoryItemID = 1,
            UnitPrice = 25.99m,
            HourlyRate = 85.00m, // Should not be provided for PARTS
            LaborHours = 2.5, // Should not be provided for PARTS
            AssignedToUserID = "tech-001"
        };

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        // Should fail on both individual field validation AND cross-validation
        Assert.True(result.Errors.Count >= 2);
    }

    [Fact]
    public async Task Validator_Should_Fail_With_Invalid_Field_Combination_Item_With_Parts()
    {
        // Given - Item with parts fields (should fail cross-validation)
        var command = new CreateWorkOrderLineItemDTO
        {
            ServiceTaskID = 1,
            ItemType = LineItemTypeEnum.ITEM,
            Quantity = 1,
            InventoryItemID = 1, // Should not be provided for ITEM
            UnitPrice = 25.99m, // Should not be provided for ITEM
            HourlyRate = 85.00m,
            LaborHours = 2.5,
            AssignedToUserID = "tech-001"
        };

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.False(result.IsValid);
        // Should fail on both individual field validation AND cross-validation
        Assert.True(result.Errors.Count >= 2);
    }

    // EDGE CASES
    [Fact]
    public async Task Validator_Should_Pass_When_Optional_Fields_Are_Null()
    {
        // Given
        var command = new CreateWorkOrderLineItemDTO
        {
            ServiceTaskID = 1,
            ItemType = LineItemTypeEnum.ITEM,
            Quantity = 1,
            InventoryItemID = 1,
            UnitPrice = 25.99m,
            AssignedToUserID = "tech-001",
            Description = null, // Optional
            HourlyRate = null,
            LaborHours = null
        };

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_Description_Is_At_Max_Length()
    {
        // Given
        var command = CreateValidPartsCommand(description: new string('A', 500)); // Exactly 500 characters

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public async Task Validator_Should_Pass_When_LaborHours_Is_At_Max_Limit()
    {
        // Given
        var command = CreateValidItemCommand(laborHours: 24.0); // Exactly 24 hours

        // When
        var result = await _validator.ValidateAsync(command);

        // Then
        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }
}