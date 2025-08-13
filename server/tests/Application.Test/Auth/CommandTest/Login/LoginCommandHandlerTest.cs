using Application.Contracts.JwtService;
using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Auth.Command;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.Auth.CommandTest.Login;

public class LoginCommandHandlerTest
{
    private readonly Mock<IUserRepository> _mockUserRepository = new();
    private readonly Mock<IJwtService> _mockJwtService = new();
    private readonly Mock<IAppLogger<LoginCommandHandler>> _mockLogger = new();
    private readonly Mock<IValidator<LoginCommand>> _mockValidator = new();
    private readonly IMapper _mapper;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg =>
            cfg.AddProfile<UserMappingProfile>()
        );

        _mapper = config.CreateMapper();

        _handler = new LoginCommandHandler(
            _mockJwtService.Object,
            _mockLogger.Object,
            _mapper,
            _mockValidator.Object,
            _mockUserRepository.Object
        );
    }

    private LoginCommand CreateValidCommand(string email = "test@example.com", string password = "Valid1@Password")
        => new LoginCommand(email, password);

    private User CreateTestUser(string id = "user-1", string email = "test@example.com", bool isActive = true)
        => new User
        {
            Id = id,
            Email = email,
            IsActive = isActive,
            FirstName = "Test",
            LastName = "User",
            HireDate = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            MaintenanceHistories = new List<MaintenanceHistory>(),
            IssueAttachments = new List<IssueAttachment>(),
            VehicleAssignments = new List<VehicleAssignment>(),
            VehicleDocuments = new List<VehicleDocument>(),
            Inspections = new List<Inspection>(),
            Vehicles = new List<Vehicle>(),
            InventoryTransactions = new List<InventoryTransaction>()
        };

    private void SetupValidValidation(LoginCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    [Fact]
    public async Task Handler_Should_Return_AuthUserDTO_On_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var user = CreateTestUser();
        var roles = new List<string> { "User", "Admin" };
        var token = "jwt-token";
        var refreshToken = "refresh-token";

        _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.CheckPasswordAsync(user, command.Password))
            .ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.GetRolesAsync(user))
            .ReturnsAsync(roles);
        _mockJwtService.Setup(j => j.GenerateToken(user.Id, user.Email!, roles))
            .Returns(token);
        _mockJwtService.Setup(j => j.GenerateRefreshToken())
            .Returns(refreshToken);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(user.Id, result.Id);
        Assert.Equal(user.Email, result.Email);
        Assert.Equal(token, result.Token);
        Assert.Equal(refreshToken, result.RefreshToken);
        Assert.Equal(roles, result.Roles);
        Assert.True(result.Expires > DateTime.UtcNow);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Arrange
        var command = CreateValidCommand();
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure("Email", "Email is required")]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _handler.Handle(command, CancellationToken.None));
        _mockUserRepository.Verify(r => r.GetByEmailAsync(It.IsAny<string>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_UnauthorizedAccessException_When_User_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_Should_Throw_UnauthorizedAccessException_When_User_Is_Inactive()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var user = CreateTestUser(isActive: false);

        _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_Should_Throw_UnauthorizedAccessException_When_Password_Is_Invalid()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var user = CreateTestUser();

        _mockUserRepository.Setup(r => r.GetByEmailAsync(command.Email))
            .ReturnsAsync(user);
        _mockUserRepository.Setup(r => r.CheckPasswordAsync(user, command.Password))
            .ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedAccessException>(() => _handler.Handle(command, CancellationToken.None));
    }
}