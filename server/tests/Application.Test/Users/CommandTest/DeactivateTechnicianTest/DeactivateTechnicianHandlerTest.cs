using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Exceptions.UserException;
using Application.Features.Users.Command.DeactivateTechnician;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using Microsoft.AspNetCore.Identity;

using Moq;

namespace Application.Test.Users.CommandTest.DeactivateTechnicianTest;

public class DeactivateTechnicianHandlerTest
{

    readonly Mock<IUserRepository> _mockUserRepository;
    readonly Mock<IAppLogger<DeactivateTechnicianCommandHandler>> _mockLogger;
    readonly DeactivateTechnicianCommandHandler _handler;

    public DeactivateTechnicianHandlerTest()
    {
        _mockUserRepository = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        var mapper = config.CreateMapper();

        _handler = new DeactivateTechnicianCommandHandler(_mockUserRepository.Object, mapper, _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_ShouldDeactivateTechnician_WhenTechnicianExists()
    {

        // Given
        var command = new DeactivateTechnicianCommand
        (
            Id: Guid.NewGuid().ToString()
        );

        var expectedUser = new User
        {
            Id = command.Id,
            FirstName = "John",
            LastName = "Doe",
            Email = "John@gmail.com",
            HireDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            MaintenanceHistories = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            Vehicles = []
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(expectedUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Success);

        // // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // // Then
        Assert.NotNull(result);
        Assert.Equal(command.Id, result);
        _mockUserRepository.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
            u.Id == expectedUser.Id &&
            u.FirstName == expectedUser.FirstName &&
            u.LastName == expectedUser.LastName &&
            u.HireDate == expectedUser.HireDate &&
            !u.IsActive // Ensure IsActive is set to false
        )), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Technician_Does_Not_Exist()
    {
        // Given
        var command = new DeactivateTechnicianCommand(Id: Guid.NewGuid().ToString());
        _mockUserRepository.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync((User?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _handler.Handle(command, CancellationToken.None));

        _mockUserRepository.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_UpdateUserException_When_RepositoryUpdate_Fails()
    {
        // Given
        var command = new DeactivateTechnicianCommand(Id: Guid.NewGuid().ToString());
        var expectedUser = new User
        {
            Id = command.Id,
            FirstName = "John",
            LastName = "Doe",
            Email = "John@gmail.com",
            HireDate = DateTime.UtcNow,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            MaintenanceHistories = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            Vehicles = []
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(command.Id)).ReturnsAsync(expectedUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Update failed" }));

        // When & Then
        await Assert.ThrowsAsync<UpdateUserException>(() => _handler.Handle(command, CancellationToken.None));

        _mockUserRepository.Verify(r => r.GetByIdAsync(command.Id), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Once);
    }
}