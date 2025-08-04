using System;
using System.Threading;
using System.Threading.Tasks;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Auth.Command.Register;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Microsoft.AspNetCore.Identity;

using Moq;

using Xunit;

namespace Application.Test.Auth.CommandTest.RegisterOperator;

public class RegisterOperatorCommandHandlerTest
{
    readonly Mock<IUserRepository> _mockUserRepository;
    readonly Mock<IAppLogger<RegisterOperatorCommandHandler>> _mockLogger;
    readonly Mock<IValidator<RegisterOperatorCommand>> _mockValidator;
    readonly RegisterOperatorCommandHandler _registerOperatorCommandHandler;

    public RegisterOperatorCommandHandlerTest()
    {
        _mockUserRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<UserMappingProfile>());
        var mapper = config.CreateMapper();

        _registerOperatorCommandHandler = new RegisterOperatorCommandHandler(
            _mockUserRepository.Object,
            mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(RegisterOperatorCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(RegisterOperatorCommand command, string propertyName = "Name", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(invalidResult);
    }

    // Helper method to create a valid command
    private RegisterOperatorCommand CreateValidCommand()
    {
        return new RegisterOperatorCommand(
            Email: "alice.smith@company.com",
            Password: "SecurePass123!",
            FirstName: "Alice",
            LastName: "Smith",
            HireDate: DateTime.UtcNow.AddDays(-1),
            IsActive: true
        );
    }

    // Helper to create successful IdentityResult
    private IdentityResult CreateSuccessResult()
    {
        return IdentityResult.Success;
    }

    [Fact]
    public async Task Handle_Should_Return_UserId_When_Command_Is_Valid()
    {
        // Given
        var command = CreateValidCommand();

        SetupValidValidation(command);

        _mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email))
            .ReturnsAsync(false);

        _mockUserRepository.Setup(r => r.AddAsyncWithRole(It.IsAny<User>(), command.Password, UserRole.FleetManager))
            .ReturnsAsync(CreateSuccessResult());

        // When
        var result = await _registerOperatorCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.NotEqual(Guid.Empty, result);

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsyncWithRole(It.Is<User>(u =>
            u.Email == command.Email &&
            u.FirstName == command.FirstName &&
            u.LastName == command.LastName &&
            u.HireDate == command.HireDate &&
            u.IsActive == command.IsActive
        ), command.Password, UserRole.FleetManager), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_DuplicateEntityException_On_Email_Already_Exist()
    {
        // Given
        var command = CreateValidCommand();

        SetupValidValidation(command);

        _mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email))
            .ReturnsAsync(true);

        // When & Then
        var exception = await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _registerOperatorCommandHandler.Handle(command, CancellationToken.None)
        );

        Assert.Contains("email", exception.Message.ToLowerInvariant());
        Assert.Contains(command.Email, exception.Message);

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsyncWithRole(It.IsAny<User>(), It.IsAny<string>(), UserRole.FleetManager), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_On_Validation_Fail()
    {
        // Given
        var command = CreateValidCommand();

        SetupInvalidValidation(command, "FirstName", "First name is required");

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _registerOperatorCommandHandler.Handle(command, CancellationToken.None)
        );

        Assert.Contains("first name is required", exception.Message.ToLowerInvariant());

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(r => r.AddAsyncWithRole(It.IsAny<User>(), It.IsAny<string>(), UserRole.FleetManager), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Repository_Fails()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email))
            .ReturnsAsync(false);

        _mockUserRepository.Setup(r => r.AddAsyncWithRole(It.IsAny<User>(), It.IsAny<string>(), UserRole.FleetManager))
            .ThrowsAsync(new Exception("Database connection failed"));

        // When & Then
        await Assert.ThrowsAsync<Exception>(
            () => _registerOperatorCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsyncWithRole(It.IsAny<User>(), It.IsAny<string>(), UserRole.FleetManager), Times.Once);
    }
}