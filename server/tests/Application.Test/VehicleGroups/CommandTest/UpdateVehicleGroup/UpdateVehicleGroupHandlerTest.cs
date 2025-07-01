using System;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.VehicleGroups.Command.UpdateVehicleGroup;
using Application.MappingProfiles;
using AutoMapper;
using Domain.Entities;
using Application.Exceptions;
using FluentValidation;
using FluentValidation.Results;
using Moq;

namespace Application.Test.VehicleGroups.CommandTest.UpdateVehicleGroup;

public class UpdateVehicleGroupHandlerTest
{
    private readonly Mock<IVehicleGroupRepository> _mockVehicleGroupRepository;
    private readonly UpdateVehicleGroupCommandHandler _updateVehicleGroupCommandHandler;
    private readonly Mock<IAppLogger<UpdateVehicleGroupCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<UpdateVehicleGroupCommand>> _mockValidator;

    public UpdateVehicleGroupHandlerTest()
    {
        _mockVehicleGroupRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleGroupMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _updateVehicleGroupCommandHandler = new(_mockVehicleGroupRepository.Object, mapper, _mockLogger.Object, _mockValidator.Object);
    }

    private UpdateVehicleGroupCommand CreateValidCommand(
        int vehicleGroupId = 123,
        string name = "Test Group",
        string? description = "Test Description",
        bool isActive = true
    )
    {
        return new UpdateVehicleGroupCommand(vehicleGroupId, name, description, isActive);
    }

    private void SetupValidValidation(UpdateVehicleGroupCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateVehicleGroupCommand command, string propertyName, string errorMessage = "Validation failed")
    {   
        var invalidResult = new ValidationResult(
            [new ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact(Skip = "Skipping this test as it is not implemented")]
    public async Task Handle_Should_Return_VehicleGroupID_On_Success()
    {
        // Given
        var command = CreateValidCommand();

        var expectedVehicleGroup = new VehicleGroup
        {
            ID = command.VehicleGroupId,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = command.Name,
            Description = command.Description,
            IsActive = command.IsActive
        };

        _mockVehicleGroupRepository.Setup(repo => repo.GetByIdAsync(expectedVehicleGroup.ID)).ReturnsAsync(expectedVehicleGroup);
        _mockVehicleGroupRepository.Setup(repo => repo.Update(expectedVehicleGroup));
        _mockVehicleGroupRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);
        SetupValidValidation(command);
        

        // When
        var result = await _updateVehicleGroupCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedVehicleGroup.ID, result);
        _mockVehicleGroupRepository.Verify(repo => repo.GetByIdAsync(expectedVehicleGroup.ID), Times.Once);
        _mockVehicleGroupRepository.Verify(repo => repo.Update(expectedVehicleGroup), Times.Once);
        _mockVehicleGroupRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
    }

    [Fact(Skip = "Skipping this test as it is not implemented")]
    public async Task Handle_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand(name: "", description: "Test Description", isActive: true);

        SetupInvalidValidation(command, "Name", "Vehicle group name is required");

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(() => _updateVehicleGroupCommandHandler.Handle(command, CancellationToken.None));

        // Then
        Assert.Equal("Vehicle group name is required", exception.Message);
        _mockVehicleGroupRepository.Verify(repo => repo.GetByIdAsync(command.VehicleGroupId), Times.Never);
        _mockVehicleGroupRepository.Verify(repo => repo.Update(It.IsAny<VehicleGroup>()), Times.Never);
        _mockVehicleGroupRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }

    [Fact(Skip = "Skipping this test as it is not implemented")]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_Invalid_VehicleGroupID()
    {
        // Given
        var command = CreateValidCommand(vehicleGroupId: 123);

        _mockVehicleGroupRepository.Setup(repo => repo.GetByIdAsync(command.VehicleGroupId)).ReturnsAsync((VehicleGroup?)null);

        // When & Then
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() => _updateVehicleGroupCommandHandler.Handle(command, CancellationToken.None));

        // Then
        Assert.Equal("Vehicle group not found", exception.Message);
        _mockVehicleGroupRepository.Verify(repo => repo.GetByIdAsync(command.VehicleGroupId), Times.Once);
        _mockVehicleGroupRepository.Verify(repo => repo.Update(It.IsAny<VehicleGroup>()), Times.Never);
        _mockVehicleGroupRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
    }
}
