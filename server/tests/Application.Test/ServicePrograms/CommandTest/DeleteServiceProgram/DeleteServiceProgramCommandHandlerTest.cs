using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServicePrograms.Command.DeleteServiceProgram;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Moq;

namespace Application.Test.ServicePrograms.CommandTest.DeleteServiceProgram;

public class DeleteServiceProgramCommandHandlerTest
{
    private readonly DeleteServiceProgramCommandHandler _commandHandler;
    private readonly Mock<IServiceProgramRepository> _mockServiceProgramRepository = new();
    private readonly Mock<ISender> _mockSender = new();
    private readonly Mock<IValidator<DeleteServiceProgramCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<DeleteServiceProgramCommandHandler>> _mockLogger = new();

    public DeleteServiceProgramCommandHandlerTest()
    {
        _commandHandler = new(
            _mockServiceProgramRepository.Object,
            _mockSender.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    private static DeleteServiceProgramCommand CreateCommand(int serviceProgramID = 1) => new(serviceProgramID);

    private void SetupValidValidation(DeleteServiceProgramCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(DeleteServiceProgramCommand command, string propertyName = nameof(DeleteServiceProgramCommand.ServiceProgramID), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ServiceProgramID_On_Success()
    {
        // Arrange
        var command = CreateCommand(7);
        SetupValidValidation(command);
        var existingServiceProgram = new ServiceProgram
        {
            ID = 7,
            Name = "Test Name",
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = []
        };
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(command.ServiceProgramID)).ReturnsAsync(existingServiceProgram);
        _mockServiceProgramRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, default);

        // Assert
        Assert.Equal(existingServiceProgram.ID, result);
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(existingServiceProgram.ID), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.Delete(existingServiceProgram), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_InvalidServiceProgramID()
    {
        // Arrange
        var command = CreateCommand(999);
        SetupValidValidation(command);
        _mockServiceProgramRepository.Setup(r => r.GetByIdAsync(command.ServiceProgramID)).ReturnsAsync((ServiceProgram?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, default));
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(999), Times.Once);
        _mockServiceProgramRepository.Verify(r => r.Delete(It.IsAny<ServiceProgram>()), Times.Never);
        _mockServiceProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_InvalidRequest()
    {
        // Arrange
        var command = CreateCommand(0); // Invalid ID
        SetupInvalidValidation(command);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, default));
        _mockServiceProgramRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockServiceProgramRepository.Verify(r => r.Delete(It.IsAny<ServiceProgram>()), Times.Never);
        _mockServiceProgramRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}