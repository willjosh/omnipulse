using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inventory.Command.DeleteInventory;

using Domain.Entities;

using Microsoft.Extensions.Logging;

using Moq;

namespace Application.Test.Inventories.CommandTest.DeleteInventoryTest;

public class DeleteInventoryCommandHandlerTest
{
    private readonly Mock<IInventoryRepository> _mockInventoryRepository;
    private readonly DeleteInventoryCommandHandler _handler;
    private readonly Mock<ILogger<DeleteInventoryCommandHandler>> _mockLogger; // Change to ILogger

    public DeleteInventoryCommandHandlerTest()
    {
        _mockInventoryRepository = new();
        _mockLogger = new(); // This will now mock ILogger instead of IAppLogger
        _handler = new DeleteInventoryCommandHandler(_mockInventoryRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_InventoryID_On_Success()
    {
        // Given
        var command = new DeleteInventoryCommand(InventoryID: 42);
        var returnedInventory = new Inventory
        {
            ID = command.InventoryID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            InventoryItemID = 1,
            QuantityOnHand = 100,
            MinStockLevel = 20,
            MaxStockLevel = 150,
            NeedsReorder = false,
            LastRestockedDate = DateTime.UtcNow.AddDays(-10),
            UnitCost = 50.00m,
            InventoryItemLocationID = 1,
            InventoryItem = null!,
            InventoryItemLocation = null!,
            InventoryTransactions = []
        };
        _mockInventoryRepository.Setup(repo => repo.GetByIdAsync(command.InventoryID)).ReturnsAsync(returnedInventory);
        _mockInventoryRepository.Setup(repo => repo.Delete(returnedInventory));
        _mockInventoryRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.InventoryID, result);
        _mockInventoryRepository.Verify(repo => repo.GetByIdAsync(command.InventoryID), Times.Once);
        _mockInventoryRepository.Verify(repo => repo.Delete(returnedInventory), Times.Once);
        _mockInventoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidInventoryID()
    {
        // Given
        var command = new DeleteInventoryCommand(InventoryID: 42);
        _mockInventoryRepository.Setup(repo => repo.GetByIdAsync(command.InventoryID)).ReturnsAsync((Inventory?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        // Then
        _mockInventoryRepository.Verify(repo => repo.GetByIdAsync(command.InventoryID), Times.Once);
        _mockInventoryRepository.Verify(repo => repo.Delete(It.IsAny<Inventory>()), Times.Never);
        _mockInventoryRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}