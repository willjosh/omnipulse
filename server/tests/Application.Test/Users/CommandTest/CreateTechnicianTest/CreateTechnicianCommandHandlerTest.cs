using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.Users.Command.CreateTechnician;
using Application.MappingProfiles;
using AutoMapper;
using FluentValidation;
using Moq;
using Microsoft.AspNetCore.Identity;
using Application.Exceptions;

namespace Application.Test.Users.CommandTest.CreateTechnicianTest;

public class CreateTechnicianCommandHandlerTest
{
    Mock<IUserRepository> _mockUserRepository;
    Mock<IAppLogger<CreateTechnicianCommandHandler>> _mockLogger;
    Mock<IValidator<CreateTechnicianCommand>> _mockValidator;
    CreateTechnicianCommandHandler _CreateTechnicianCommandHandler;

    public CreateTechnicianCommandHandlerTest()
    {
        _mockUserRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<UserMappingProfile>();
        });
        var mapper = config.CreateMapper();

        _CreateTechnicianCommandHandler = new CreateTechnicianCommandHandler(
            _mockUserRepository.Object,
            mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    // Helper method to set up valid validation result
    private void SetupValidValidation(CreateTechnicianCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(validResult);
    }

    // Helper method to set up validation failure
    private void SetupInvalidValidation(CreateTechnicianCommand command, string propertyName = "Name", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
                     .ReturnsAsync(invalidResult);
    }

    // Helper method to create a valid command
    private CreateTechnicianCommand CreateValidCommand()
    {
        return new CreateTechnicianCommand(
            Email: "john.doe@company.com",
            Password: "SecurePass123!",
            FirstName: "John",
            LastName: "Doe",
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

        // Mock AddAsync to succeed AND simulate UserManager populating the ID
        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<IdentityUser>(), command.Password))
            .Callback<IdentityUser, string>((user, _) =>
            {
                user.Id = Guid.NewGuid().ToString();
            })
            .ReturnsAsync(CreateSuccessResult());

        // When
        var result = await _CreateTechnicianCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.NotEqual(Guid.Empty, result);

        // Verify all dependencies were called correctly
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.Is<IdentityUser>(u =>
            u.Email == command.Email &&
            ((Domain.Entities.User)u).FirstName == command.FirstName &&
            ((Domain.Entities.User)u).LastName == command.LastName &&
            ((Domain.Entities.User)u).HireDate == command.HireDate &&
            ((Domain.Entities.User)u).IsActive == command.IsActive
        ), command.Password), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_DuplicateEntityException_On_Email_Already_Exist()
    {
        // Given
        var command = CreateValidCommand();

        SetupValidValidation(command);

        _mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email))
            .ReturnsAsync(true); // Email already exists

        // When & Then
        var exception = await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _CreateTechnicianCommandHandler.Handle(command, CancellationToken.None)
        );

        Assert.Contains("email", exception.Message.ToLowerInvariant());
        Assert.Contains(command.Email, exception.Message);

        // Verify validation and email check were called but AddAsync was not
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_On_Validation_Fail()
    {
        // Given
        var command = CreateValidCommand();

        SetupInvalidValidation(command, "FirstName", "First name is required");

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _CreateTechnicianCommandHandler.Handle(command, CancellationToken.None)
        );

        Assert.Contains("first name is required", exception.Message.ToLowerInvariant());

        // Verify only validation was called
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(It.IsAny<string>()), Times.Never);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_Exception_When_Repository_Fails()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockUserRepository.Setup(r => r.EmailExistsAsync(command.Email))
            .ReturnsAsync(false);

        _mockUserRepository.Setup(r => r.AddAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database connection failed"));

        // When & Then
        await Assert.ThrowsAsync<Exception>(
            () => _CreateTechnicianCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify the expected calls were made
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockUserRepository.Verify(r => r.EmailExistsAsync(command.Email), Times.Once);
        _mockUserRepository.Verify(r => r.AddAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()), Times.Once);
    }

}