using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.VehicleGroups.Command.DeleteVehicleGroup;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using Moq;

namespace Application.Test.VehicleGroups.CommandTest.DeleteVehicleGroup;

public class DeleteVehicleGroupHandlerTest
{
    private readonly Mock<IVehicleGroupRepository> _mockVehicleGroupRepository;
    private readonly DeleteVehicleGroupCommandHandler _deleteVehicleGroupCommandHandler;
    private readonly Mock<IAppLogger<DeleteVehicleGroupCommandHandler>> _mockLogger;

    public DeleteVehicleGroupHandlerTest()
    {
        _mockVehicleGroupRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<VehicleGroupMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _deleteVehicleGroupCommandHandler = new(_mockVehicleGroupRepository.Object, _mockLogger.Object, mapper);
    }

    [Fact]
    public async Task Handle_Should_Return_VehicleGroupID_On_Success()
    {
        // Given
        var command = new DeleteVehicleGroupCommand(VehicleGroupID: 123);

        var returnedVehicleGroup = new VehicleGroup
        {
            ID = command.VehicleGroupID,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = "Test Group",
            Description = "Test Description",
            IsActive = true,
        };

        _mockVehicleGroupRepository.Setup(repo => repo.GetByIdAsync(command.VehicleGroupID)).ReturnsAsync(returnedVehicleGroup);
        _mockVehicleGroupRepository.Setup(repo => repo.Delete(returnedVehicleGroup));
        _mockVehicleGroupRepository.Setup(repo => repo.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _deleteVehicleGroupCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.VehicleGroupID, result);
        _mockVehicleGroupRepository.Verify(repo => repo.GetByIdAsync(command.VehicleGroupID), Times.Once);
        _mockVehicleGroupRepository.Verify(repo => repo.Delete(returnedVehicleGroup), Times.Once);
        _mockVehicleGroupRepository.Verify(repo => repo.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_On_InvalidVehicleGroupID()
    {
        // Given
        var command = new DeleteVehicleGroupCommand(VehicleGroupID: 123);

        _mockVehicleGroupRepository.Setup(repo => repo.GetByIdAsync(command.VehicleGroupID)).ReturnsAsync((VehicleGroup?)null);

        // When
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _deleteVehicleGroupCommandHandler.Handle(command, CancellationToken.None));

        // Then
        _mockVehicleGroupRepository.Verify(repo => repo.GetByIdAsync(command.VehicleGroupID), Times.Once);
        _mockVehicleGroupRepository.Verify(repo => repo.Delete(It.IsAny<VehicleGroup>()), Times.Never);
        _mockVehicleGroupRepository.Verify(repo => repo.SaveChangesAsync(), Times.Never);
    }
}