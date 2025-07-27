using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InventoryItemLocations.Command;
using Application.Features.InventoryItemLocations.Command.CreateInventoryItemLocation;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.InventoryItemLocations.CommandTest.CreateInventoryItemLocation;

public class CreateInventoryItemLocationCommandHandlerTest
{
    private readonly Mock<IInventoryItemLocationRepository> _mockRepository;
    private readonly Mock<IMapper> _mockMapper;
    private readonly Mock<IAppLogger<CreateInventoryItemLocationCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<CreateInventoryItemLocationCommand>> _mockValidator;
    private readonly CreateInventoryItemLocationCommandHandler _handler;

    public CreateInventoryItemLocationCommandHandlerTest()
    {
        _mockRepository = new();
        _mockMapper = new();
        _mockLogger = new();
        _mockValidator = new();
        _handler = new(
            _mockRepository.Object,
            _mockMapper.Object,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    private CreateInventoryItemLocationCommand CreateValidCommand(
        string locationName = "Warehouse A",
        string address = "123 Main St",
        double longitude = 100.0,
        double latitude = 10.0,
        int capacity = 50)
    {
        return new CreateInventoryItemLocationCommand(
            locationName,
            address,
            longitude,
            latitude,
            capacity
        );
    }

    private void SetupValidValidation(CreateInventoryItemLocationCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateInventoryItemLocationCommand command, string propertyName = "LocationName", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_LocationID_On_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var expectedEntity = new InventoryItemLocation
        {
            ID = 42,
            LocationName = command.LocationName,
            Address = command.Address,
            Longitude = command.Longitude,
            Latitude = command.Latitude,
            Capacity = command.Capacity,
            IsActive = true,
            Inventories = new List<Inventory>(),
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        _mockMapper.Setup(m => m.Map<InventoryItemLocation>(command)).Returns(expectedEntity);
        _mockRepository.Setup(r => r.AddAsync(It.IsAny<InventoryItemLocation>())).ReturnsAsync(expectedEntity);
        _mockRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<InventoryItemLocation>()), Times.Once);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "LocationName", "Location name is required");

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockRepository.Verify(r => r.AddAsync(It.IsAny<InventoryItemLocation>()), Times.Never);
        _mockRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

}