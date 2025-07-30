using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Command.UpdateServiceProgram;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.ServicePrograms.CommandTest.UpdateServiceProgram;

public class UpdateServiceProgramCommandHandlerTest
{
    private readonly UpdateServiceProgramCommandHandler _commandHandler;
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<IValidator<UpdateServiceProgramCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<UpdateServiceProgramCommandHandler>> _mockLogger = new();
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc);

    public UpdateServiceProgramCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceProgramMappingProfile>());
        _mapper = config.CreateMapper();
        _commandHandler = new UpdateServiceProgramCommandHandler(
            _mockServiceProgramRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper);
    }

    private static UpdateServiceProgramCommand CreateValidCommand(
        int serviceProgramID = 1,
        string name = "Service Program Name",
        string? description = "Service Program Description",
        bool isActive = true) => new(serviceProgramID, name, description, isActive);

    private void SetupValidValidation(UpdateServiceProgramCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateServiceProgramCommand command, string propertyName = nameof(UpdateServiceProgramCommand.Name), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ID_On_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var existingEntity = new ServiceProgram
        {
            ID = command.ServiceProgramID,
            Name = "Old Name",
            Description = "Old Description",
            IsActive = false,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(command.ServiceProgramID)).ReturnsAsync(existingEntity);
        _mockServiceProgramRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(true);
        _mockServiceProgramRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(command.ServiceProgramID), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.Update(existingEntity), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(command.ServiceProgramID)).ReturnsAsync((ServiceProgram?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_When_Name_Exists()
    {
        // Arrange
        var command = CreateValidCommand(name: "DuplicateName");
        SetupValidValidation(command);
        var existingEntity = new ServiceProgram
        {
            ID = command.ServiceProgramID,
            Name = "Old Name",
            Description = "Old Description",
            IsActive = false,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(command.ServiceProgramID)).ReturnsAsync(existingEntity);
        _mockServiceProgramRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(false);

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
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(command.ServiceProgramID), Times.Never);
        _mockServiceProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Allow_Optional_Description()
    {
        // Arrange
        var command = CreateValidCommand(description: null);
        SetupValidValidation(command);
        var existingEntity = new ServiceProgram
        {
            ID = command.ServiceProgramID,
            Name = "Old Name",
            Description = "Old Description",
            IsActive = false,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(command.ServiceProgramID)).ReturnsAsync(existingEntity);
        _mockServiceProgramRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(true);
        _mockServiceProgramRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(command.ServiceProgramID), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.Update(existingEntity), Times.Once);
    }
}