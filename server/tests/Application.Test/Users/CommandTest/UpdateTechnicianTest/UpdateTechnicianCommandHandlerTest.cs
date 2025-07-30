using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Users.Command.UpdateTechnician;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Moq;

namespace Application.Test.Users.CommandTest.UpdateTechnicianTest;

public class UpdateTechnicianCommandHandlerTest
{

    readonly Mock<IUserRepository> _mockUserRepository;
    readonly Mock<IAppLogger<UpdateTechnicianCommandHandler>> _mockLogger;
    readonly Mock<IValidator<UpdateTechnicianCommand>> _mockValidator;
    readonly UpdateTechnicianCommandHandler _updateTechnicianCommandHandler;

    public UpdateTechnicianCommandHandlerTest()
    {
        _mockUserRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _updateTechnicianCommandHandler = new UpdateTechnicianCommandHandler(_mockUserRepository.Object, mapper, _mockLogger.Object, _mockValidator.Object);
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(UpdateTechnicianCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(UpdateTechnicianCommand command, string propertyName = "Name", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(invalidResult);
    }

    static UpdateTechnicianCommand CreateValidCommand(
        string? id = null,
        string? firstName = null,
        string? lastName = null,
        DateTime? hireDate = null,
        bool? isActive = null
    )
    {
        return new UpdateTechnicianCommand
        (
            id ?? Guid.NewGuid().ToString(),
            firstName,
            lastName,
            hireDate,
            isActive
        );
    }

    private IdentityResult CreateSuccessResult()
    {
        return IdentityResult.Success;
    }

    [Theory]
    [InlineData("Clarence", "Drifter", "2023-10-01", true)]
    [InlineData("Bolso Rojo", null, "2023-10-01", true)]
    [InlineData("Mamba", "Viper", null, true)]
    [InlineData("FullOfJoy", "Happiness", "2023-10-01", null)]
    [InlineData(null, null, null, null)]
    public async Task Handle_ValidCommand_ShouldUpdateTechnician(
        string? firstName,
        string? lastName,
        string? hireDate,
        bool? isActive
    )
    {
        // Given
        var command = CreateValidCommand(
            id: Guid.NewGuid().ToString(),
            firstName: firstName,
            lastName: lastName,
            hireDate: hireDate is not null ? DateTime.Parse(hireDate) : null,
            isActive: isActive
        );

        SetupValidValidation(command);

        var returnedUser = new User
        {
            Id = command.Id,
            FirstName = "OriginalFirstName",
            LastName = "OriginalLastName",
            HireDate = new DateTime(2020, 1, 1),
            IsActive = false,
            CreatedAt = DateTime.UtcNow.AddYears(-1),
            UpdatedAt = DateTime.UtcNow.AddYears(-1),
            Email = "valid@gmail.com",
            MaintenanceHistories = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            Vehicles = [],
        };

        _mockUserRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync(returnedUser);
        _mockUserRepository.Setup(r => r.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(CreateSuccessResult());

        // When 
        var result = await _updateTechnicianCommandHandler.Handle(command, CancellationToken.None);

        // Then
        if (!command.ShouldUpdate)
        {
            _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
        }
        else
        {
            _mockUserRepository.Verify(r => r.UpdateAsync(It.Is<User>(u =>
                u.Id == command.Id &&
                (command.FirstName == null || u.FirstName == command.FirstName) &&
                (command.LastName == null || u.LastName == command.LastName) &&
                (command.HireDate == null || u.HireDate == command.HireDate) &&
                (command.IsActive == null || u.IsActive == command.IsActive)
            )), Times.Once);
        }

    }

    [Fact(Skip = "not implemented yet")]
    public async Task Handle_Should_Throw_NotFoundException_When_Technician_Not_Found()
    {
        // Given
        var command = CreateValidCommand(
            firstName: "John"
        );

        _mockUserRepository.Setup(r => r.GetByIdAsync(command.Id))
            .ReturnsAsync((User?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateTechnicianCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Once);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }

    [Fact(Skip = "not implemented yet")]
    public async Task Handle_Should_Throw_BadRequestException_On_Validation_Fail()
    {
        // Given
        var command = CreateValidCommand();

        SetupInvalidValidation(command, "id", "id is required");

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _updateTechnicianCommandHandler.Handle(command, CancellationToken.None)
        );

        Assert.Contains("id is required", exception.Message.ToLowerInvariant());

        // Verify only validation was called
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.GetByIdAsync(It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(r => r.UpdateAsync(It.IsAny<User>()), Times.Never);
    }
}