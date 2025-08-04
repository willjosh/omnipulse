using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inventory.Command.UpdateInventory;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

using Xunit;

namespace Application.Test.Inventories.CommandTest.UpdateInventoryTest;

public class UpdateInventoryCommandHandlerTest
{
    private readonly UpdateInventoryCommandHandler _updateInventoryCommandHandler;
    private readonly Mock<IInventoryRepository> _mockInventoryRepository;
    private readonly Mock<IInventoryTransactionRepository> _mockInventoryTransactionRepository;
    private readonly Mock<IValidator<UpdateInventoryCommand>> _mockValidator;
    private readonly Mock<IAppLogger<UpdateInventoryCommandHandler>> _mockLogger;
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public UpdateInventoryCommandHandlerTest()
    {
        _mockInventoryRepository = new();
        _mockInventoryTransactionRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InventoryMappingProfile>());
        _mapper = config.CreateMapper();

        _updateInventoryCommandHandler = new(
            _mockInventoryRepository.Object,
            _mockInventoryTransactionRepository.Object,
            _mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    private static UpdateInventoryCommand CreateValidCommand(
        int inventoryID = 123,
        int quantityOnHand = 150,
        decimal unitCost = 99.99m,
        int minStockLevel = 25,
        int maxStockLevel = 200,
        bool isAdjustment = false,
        string? performedByUserID = null)
    {
        return new UpdateInventoryCommand(
            inventoryID,
            quantityOnHand,
            unitCost,
            minStockLevel,
            maxStockLevel,
            isAdjustment,
            performedByUserID ?? Guid.NewGuid().ToString() // Convert Guid to string
        );
    }

    private static Inventory CreateExistingInventory()
    {
        var inventoryItem = new InventoryItem
        {
            ID = 1,
            ItemNumber = "ITEM-001",
            ItemName = "Test Part",
            Description = "Test Description",
            Category = InventoryItemCategoryEnum.ENGINE,
            Manufacturer = "Test Manufacturer",
            ManufacturerPartNumber = "MPN-001",
            UniversalProductCode = "123456789012",
            UnitCost = 50.00m,
            UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Box,
            Supplier = "Test Supplier",
            WeightKG = 1.5,
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inventories = new List<Inventory>(),
            WorkOrderLineItems = new List<WorkOrderLineItem>()
        };

        var location = new InventoryItemLocation
        {
            ID = 1,
            LocationName = "Main Warehouse",
            Address = "123 Main St",
            Longitude = -122.4194,
            Latitude = 37.7749,
            Capacity = 1000,
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inventories = new List<Inventory>()
        };

        return new Inventory
        {
            ID = 123,
            InventoryItemID = 1,
            InventoryItemLocationID = 1,
            QuantityOnHand = 100,
            MinStockLevel = 20,
            MaxStockLevel = 150,
            NeedsReorder = false,
            LastRestockedDate = FixedDate.AddDays(-10),
            UnitCost = 50.00m,
            InventoryItem = inventoryItem,
            InventoryItemLocation = location,
            InventoryTransactions = new List<InventoryTransaction>(),
            CreatedAt = FixedDate.AddDays(-30),
            UpdatedAt = FixedDate.AddDays(-1)
        };
    }

    private void SetupValidValidation(UpdateInventoryCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateInventoryCommand command, string propertyName = "InventoryID", string errorMessage = "Validation failed")
    {
        var invalidResult = new ValidationResult(
            [new ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_InventoryID_On_Success()
    {
        // Given
        var command = CreateValidCommand();
        var existingInventory = CreateExistingInventory();
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        var result = await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingInventory.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInventoryRepository.Verify(r => r.GetByIdAsync(command.InventoryID), Times.Once);
        _mockInventoryRepository.Verify(r => r.Update(existingInventory), Times.Once);
        _mockInventoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Inventory_Does_Not_Exist()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync((Inventory?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateInventoryCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInventoryRepository.Verify(r => r.GetByIdAsync(command.InventoryID), Times.Once);
        _mockInventoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "QuantityOnHand", "Quantity on hand must be zero or greater");

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(
            () => _updateInventoryCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInventoryRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInventoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Create_Transaction_When_Quantity_Changes()
    {
        // Given
        var command = CreateValidCommand(quantityOnHand: 200); // Changed from 100 to 200
        var existingInventory = CreateExistingInventory();
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        var result = await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingInventory.ID, result);
        _mockInventoryTransactionRepository.Verify(r => r.AddAsync(It.IsAny<InventoryTransaction>()), Times.Once);
        _mockInventoryTransactionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Not_Create_Transaction_When_No_Changes()
    {
        // Given
        var existingInventory = CreateExistingInventory();
        var command = CreateValidCommand(
            inventoryID: existingInventory.ID,
            quantityOnHand: existingInventory.QuantityOnHand, // Same quantity
            unitCost: existingInventory.UnitCost, // Same unit cost
            minStockLevel: 25, // Different but doesn't affect transaction
            maxStockLevel: 200
        );
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        var result = await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingInventory.ID, result);
        _mockInventoryTransactionRepository.Verify(r => r.AddAsync(It.IsAny<InventoryTransaction>()), Times.Never);
        _mockInventoryTransactionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Update_LastRestockedDate_When_Quantity_Changes()
    {
        // Given
        var command = CreateValidCommand(quantityOnHand: 200);
        var existingInventory = CreateExistingInventory();
        var originalLastRestockedDate = existingInventory.LastRestockedDate;
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.NotEqual(originalLastRestockedDate, existingInventory.LastRestockedDate);
        Assert.True(existingInventory.LastRestockedDate > originalLastRestockedDate);
    }

    [Fact]
    public async Task Handler_Should_Not_Update_LastRestockedDate_When_Quantity_Unchanged()
    {
        // Given
        var existingInventory = CreateExistingInventory();
        var command = CreateValidCommand(
            inventoryID: existingInventory.ID,
            quantityOnHand: existingInventory.QuantityOnHand, // Same quantity
            minStockLevel: 25 // Different min stock level
        );
        var originalLastRestockedDate = existingInventory.LastRestockedDate;
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(originalLastRestockedDate, existingInventory.LastRestockedDate);
    }

    [Fact]
    public async Task Handler_Should_Update_NeedsReorder_Status()
    {
        // Given
        var command = CreateValidCommand(
            quantityOnHand: 5, // Below min stock level of 25
            minStockLevel: 25
        );
        var existingInventory = CreateExistingInventory();
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.True(existingInventory.NeedsReorder);
    }

    [Fact]
    public async Task Handler_Should_Create_Transaction_When_UnitCost_Changes()
    {
        // Given
        var existingInventory = CreateExistingInventory();
        var command = CreateValidCommand(
            inventoryID: existingInventory.ID,
            quantityOnHand: existingInventory.QuantityOnHand, // Same quantity
            unitCost: 75.00m // Different unit cost
        );
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        _mockInventoryTransactionRepository.Verify(r => r.AddAsync(It.IsAny<InventoryTransaction>()), Times.Once);
    }

    [Theory]
    [InlineData(50, 150, false)] // Increase quantity, not adjustment
    [InlineData(150, 50, false)] // Decrease quantity, not adjustment
    [InlineData(50, 150, true)]  // Increase quantity, adjustment
    [InlineData(150, 50, true)]  // Decrease quantity, adjustment
    public async Task Handler_Should_Handle_Different_Transaction_Scenarios(int oldQuantity, int newQuantity, bool isAdjustment)
    {
        // Given
        var existingInventory = CreateExistingInventory();
        existingInventory.QuantityOnHand = oldQuantity;

        var command = CreateValidCommand(
            inventoryID: existingInventory.ID,
            quantityOnHand: newQuantity,
            isAdjustment: isAdjustment
        );
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        var result = await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(existingInventory.ID, result);
        Assert.Equal(newQuantity, existingInventory.QuantityOnHand);
        _mockInventoryTransactionRepository.Verify(r => r.AddAsync(It.IsAny<InventoryTransaction>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Information_Messages()
    {
        // Given
        var command = CreateValidCommand();
        var existingInventory = CreateExistingInventory();
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Handling UpdateInventoryCommand"))),
            Times.Once);
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Successfully updated inventory"))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Error_When_Inventory_Not_Found()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync((Inventory?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateInventoryCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockLogger.Verify(
            x => x.LogError(It.Is<string>(s => s.Contains("not found"))),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Update_All_Properties_Correctly()
    {
        // Given
        var command = CreateValidCommand(
            quantityOnHand: 250,
            unitCost: 125.50m,
            minStockLevel: 30,
            maxStockLevel: 300
        );
        var existingInventory = CreateExistingInventory();
        SetupValidValidation(command);

        _mockInventoryRepository.Setup(r => r.GetByIdAsync(command.InventoryID))
            .ReturnsAsync(existingInventory);
        _mockInventoryTransactionRepository.Setup(r => r.AddAsync(It.IsAny<InventoryTransaction>()))
            .ReturnsAsync(It.IsAny<InventoryTransaction>());
        _mockInventoryTransactionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
        _mockInventoryRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // When
        await _updateInventoryCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.QuantityOnHand, existingInventory.QuantityOnHand);
        Assert.Equal(command.UnitCost, existingInventory.UnitCost);
        Assert.Equal(command.MinStockLevel, existingInventory.MinStockLevel);
        Assert.Equal(command.MaxStockLevel, existingInventory.MaxStockLevel);
        // ID and timestamps should not be changed by mapping
        Assert.Equal(123, existingInventory.ID);
        Assert.Equal(FixedDate.AddDays(-30), existingInventory.CreatedAt);
    }
}