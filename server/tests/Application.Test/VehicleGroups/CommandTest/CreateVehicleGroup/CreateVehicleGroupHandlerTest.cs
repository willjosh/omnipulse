using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using FluentValidation;
using Moq;

namespace Application.Test.VehicleGroups.CommandTest.CreateVehicleGroup;

public class CreateVehicleGroupHandlerTest
{
    private readonly Mock<IVehicleGroupRepository> _mockVehicleGroupRepository;
    private readonly CreateVehicleGroupCommandHandler _createVehicleGroupCommandHandler;
    private readonly Mock<IAppLogger<CreateVehicleGroupCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<CreateVehicleGroupCommand>> _mockValidator;

    public CreateVehicleGroupHandlerTest()
    {
        _mockVehicleGroupRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleGroupMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _createVehicleGroupCommandHandler = new(_mockVehicleGroupRepository.Object, mapper, _mockLogger.Object, _mockValidator.Object);
    }

    private CreateVehicleGroupCommand CreateValidCommand(
        string name = "Test Group",
        string? description = "Test Description",
        bool isActive = true
    )
    {
        return new CreateVehicleGroupCommand(name, description, isActive);
    }

    [Fact]
    public async Task Handle_Should_Return_VehicleGroupID_On_Success()
    {
        // Given
        var command = CreateValidCommand();

        var expectedVehicleGroup = new VehicleGroup
        {
            ID = 12,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = command.Name,
            Description = command.Description,
            IsActive = command.IsActive
        };

        _mockVehicleGroupRepository.Setup(repo => repo.AddAsync(It.IsAny<VehicleGroup>())).ReturnsAsync(expectedVehicleGroup);
        _mockVehicleGroupRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _createVehicleGroupCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedVehicleGroup.ID, result);
        _mockVehicleGroupRepository.Verify(repo => repo.AddAsync(It.IsAny<VehicleGroup>()), Times.Once);
        _mockVehicleGroupRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _mockValidator.Verify(validator => validator.ValidateAsync(command, CancellationToken.None), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand(name: "", description: "Test Description", isActive: true);

        // When
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _createVehicleGroupCommandHandler.Handle(command, CancellationToken.None));

        // Then
        Assert.Equal("Vehicle group name is required", exception.Message);
        _mockVehicleGroupRepository.Verify(repo => repo.AddAsync(It.IsAny<VehicleGroup>()), Times.Never);
        _mockVehicleGroupRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockValidator.Verify(validator => validator.ValidateAsync(command, CancellationToken.None), Times.Once);
    }
}
