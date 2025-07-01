using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InventoryItems.Command.DeleteInventoryItem;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using Domain.Entities.Enums;
using Moq;
using Xunit;

namespace Application.Test.InventoryItems.CommandTest.DeleteInventoryItem;

public class DeleteInventoryItemCommandHandlerTest
{
    private readonly Mock<IInventoryItemRepository> _mockInventoryItemRepository;
    private readonly DeleteInventoryItemCommandHandler _deleteInventoryItemCommandHandler;
    private readonly Mock<IAppLogger<DeleteInventoryItemCommandHandler>> _mockLogger;

    public DeleteInventoryItemCommandHandlerTest()
    {
        _mockInventoryItemRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<InventoryItemMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _deleteInventoryItemCommandHandler = new(_mockInventoryItemRepository.Object, _mockLogger.Object, mapper);
    }

    [Fact]
    public async Task Handle_Should_Return_InventoryItemID_On_Success()
    {
        // Given
        var command = new DeleteInventoryItemCommand(InventoryItemID: 123);

        var returnedInventoryItem = new InventoryItem
        {
            ID = command.InventoryItemID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ItemNumber = "TEST-001",
            ItemName = "Test Item",
            Description = "Test Description",
            Category = InventoryItemCategoryEnum.ENGINE,
            Manufacturer = "Test Manufacturer",
            ManufacturerPartNumber = "MPN-001",
            UniversalProductCode = "123456789012",
            UnitCost = 100.00m,
            UnitCostMeasurementUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
            Supplier = "Test Supplier",
            WeightKG = 5.0,
            IsActive = true,
            Inventories = [],
            WorkOrderLineItems = []
        };

        _mockInventoryItemRepository.Setup(repo => repo.GetByIdAsync(command.InventoryItemID)).ReturnsAsync(returnedInventoryItem);
        _mockInventoryItemRepository.Setup(repo => repo.Delete(returnedInventoryItem));
        _mockInventoryItemRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _deleteInventoryItemCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.InventoryItemID, result);
        _mockInventoryItemRepository.Verify(repo => repo.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(repo => repo.Delete(returnedInventoryItem), Times.Once);
        _mockInventoryItemRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidInventoryItemID()
    {
        // Given
        var command = new DeleteInventoryItemCommand(InventoryItemID: 123);

        _mockInventoryItemRepository.Setup(repo => repo.GetByIdAsync(command.InventoryItemID)).ReturnsAsync((InventoryItem?)null);

        // When
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _deleteInventoryItemCommandHandler.Handle(command, CancellationToken.None));

        // Then
        _mockInventoryItemRepository.Verify(repo => repo.GetByIdAsync(command.InventoryItemID), Times.Once);
        _mockInventoryItemRepository.Verify(repo => repo.Delete(It.IsAny<InventoryItem>()), Times.Never);
        _mockInventoryItemRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}
