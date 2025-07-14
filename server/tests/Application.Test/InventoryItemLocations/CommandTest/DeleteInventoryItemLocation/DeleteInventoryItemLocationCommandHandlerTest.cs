
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InventoryItemLocations.Command.DeleteInventoryItemLocation;

using Domain.Entities;

using Moq;

using Xunit;

namespace Application.Test.InventoryItemLocations.CommandTest.DeleteInventoryItemLocation;

public class DeleteInventoryItemLocationCommandHandlerTest
{
    private readonly Mock<IInventoryItemLocationRepository> _mockInventoryItemLocationRepository;
    private readonly DeleteInventoryItemLocationCommandHandler _handler;
    private readonly Mock<IAppLogger<DeleteInventoryItemLocationCommandHandler>> _mockLogger;

    public DeleteInventoryItemLocationCommandHandlerTest()
    {
        _mockInventoryItemLocationRepository = new();
        _mockLogger = new();
        _handler = new DeleteInventoryItemLocationCommandHandler(_mockInventoryItemLocationRepository.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_InventoryItemLocationID_On_Success()
    {
        // Given
        var command = new DeleteInventoryItemLocationCommand(InventoryItemLocationID: 123);
        var returnedLocation = new InventoryItemLocation
        {
            ID = command.InventoryItemLocationID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            LocationName = "Test Location",
            Address = "123 Test St",
            Longitude = 100.0,
            Latitude = 50.0,
            Capacity = 10,
            IsActive = true,
            Inventories = []
        };
        _mockInventoryItemLocationRepository.Setup(repo => repo.GetByIdAsync(command.InventoryItemLocationID)).ReturnsAsync(returnedLocation);
        _mockInventoryItemLocationRepository.Setup(repo => repo.Delete(returnedLocation));
        _mockInventoryItemLocationRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.InventoryItemLocationID, result);
        _mockInventoryItemLocationRepository.Verify(repo => repo.GetByIdAsync(command.InventoryItemLocationID), Times.Once);
        _mockInventoryItemLocationRepository.Verify(repo => repo.Delete(returnedLocation), Times.Once);
        _mockInventoryItemLocationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidInventoryItemLocationID()
    {
        // Given
        var command = new DeleteInventoryItemLocationCommand(InventoryItemLocationID: 123);
        _mockInventoryItemLocationRepository.Setup(repo => repo.GetByIdAsync(command.InventoryItemLocationID)).ReturnsAsync((InventoryItemLocation?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        // Then
        _mockInventoryItemLocationRepository.Verify(repo => repo.GetByIdAsync(command.InventoryItemLocationID), Times.Once);
        _mockInventoryItemLocationRepository.Verify(repo => repo.Delete(It.IsAny<InventoryItemLocation>()), Times.Never);
        _mockInventoryItemLocationRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}