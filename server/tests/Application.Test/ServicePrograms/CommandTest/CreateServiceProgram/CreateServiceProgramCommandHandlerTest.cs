using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.ServicePrograms.CommandTest.CreateServiceProgram;

public class CreateServiceProgramCommandHandlerTest
{
    private readonly CreateServiceProgramCommandHandler _commandHandler;
    private readonly Mock<IServiceProgramRepository> _mockProgramRepository = new();
    private readonly Mock<IValidator<CreateServiceProgramCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<CreateServiceProgramCommandHandler>> _mockLogger = new();

    // Constants
    private static readonly DateTime TestDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public CreateServiceProgramCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceProgramMappingProfile>());
        var mapper = config.CreateMapper();
        _commandHandler = new CreateServiceProgramCommandHandler(
            _mockProgramRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            mapper);
    }

    // Valid command factory
    private static CreateServiceProgramCommand CreateValidCommand(
        string name = "Fleet Maintenance",
        string? description = "Description",
        bool isActive = true) => new(name, description, isActive);

    private void SetupValidValidation(CreateServiceProgramCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateServiceProgramCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateServiceProgramCommand command, string propertyName = nameof(CreateServiceProgramCommand.Name), string errorMessage = "Validation failed")
    {
        var invalidResult = new ValidationResult(
            [new ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(It.IsAny<CreateServiceProgramCommand>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ID_On_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockProgramRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(true);
        var expectedEntity = new ServiceProgram
        {
            ID = 42,
            Name = command.Name,
            Description = command.Description,
            IsActive = command.IsActive,
            CreatedAt = TestDate,
            UpdatedAt = TestDate,
            // Navigation Properties - Nullify
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        _mockProgramRepository.Setup(r => r.AddAsync(It.IsAny<ServiceProgram>())).ReturnsAsync(expectedEntity);
        _mockProgramRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockProgramRepository.Verify(r => r.IsNameUniqueAsync(command.Name), Times.Once);
        _mockProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockProgramRepository.Verify(r => r.AddAsync(It.IsAny<ServiceProgram>()), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_When_Name_Exists()
    {
        // Arrange
        var command = CreateValidCommand(name: "DuplicateName");
        SetupValidValidation(command);
        _mockProgramRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand(name: "");
        SetupInvalidValidation(command);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockProgramRepository.Verify(r => r.IsNameUniqueAsync(command.Name), Times.Never);
        _mockProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Allow_Optional_Description()
    {
        // Arrange
        var command = CreateValidCommand(description: null);
        SetupValidValidation(command);
        _mockProgramRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(true);
        var expectedEntity = new ServiceProgram
        {
            ID = 99,
            Name = command.Name,
            Description = null,
            IsActive = command.IsActive,
            CreatedAt = TestDate,
            UpdatedAt = TestDate,
            // Navigation Properties - Nullify
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        _mockProgramRepository.Setup(r => r.AddAsync(It.IsAny<ServiceProgram>())).ReturnsAsync(expectedEntity);
        _mockProgramRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockProgramRepository.Verify(r => r.IsNameUniqueAsync(command.Name), Times.Once);
        _mockProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}