using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

using Xunit;

namespace Application.Test.ServiceSchedules.CommandTest.CreateServiceSchedule;

public class CreateServiceScheduleCommandHandlerTest
{
    private readonly CreateServiceScheduleCommandHandler _commandHandler;
    private readonly Mock<IServiceScheduleRepository> _mockScheduleRepository = new();
    private readonly Mock<IServiceProgramRepository> _mockProgramRepository = new();
    private readonly Mock<IValidator<CreateServiceScheduleCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<CreateServiceScheduleCommandHandler>> _mockLogger = new();

    public CreateServiceScheduleCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceScheduleMappingProfile>());
        var mapper = config.CreateMapper();

        _commandHandler = new CreateServiceScheduleCommandHandler(
            _mockScheduleRepository.Object,
            _mockProgramRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            mapper);
    }

    // Valid command factory
    private static CreateServiceScheduleCommand CreateValidCommand(
        int serviceProgramId = 1,
        string name = "5000 km / 6 month service",
        int? timeIntervalValue = 6,
        TimeUnitEnum? timeIntervalUnit = TimeUnitEnum.Days,
        int? timeBufferValue = 1,
        TimeUnitEnum? timeBufferUnit = TimeUnitEnum.Weeks,
        int? mileageInterval = 5000,
        int? mileageBuffer = 250,
        int? firstServiceTimeValue = null,
        TimeUnitEnum? firstServiceTimeUnit = null,
        int? firstServiceMileage = null,
        bool isActive = true) => new(
            ServiceProgramID: serviceProgramId,
            Name: name,
            TimeIntervalValue: timeIntervalValue,
            TimeIntervalUnit: timeIntervalUnit,
            TimeBufferValue: timeBufferValue,
            TimeBufferUnit: timeBufferUnit,
            MileageInterval: mileageInterval,
            MileageBuffer: mileageBuffer,
            FirstServiceTimeValue: firstServiceTimeValue,
            FirstServiceTimeUnit: firstServiceTimeUnit,
            FirstServiceMileage: firstServiceMileage,
            IsActive: isActive);

    private void SetupValidValidation(CreateServiceScheduleCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateServiceScheduleCommand command, string propertyName = "Name", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ID_On_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockProgramRepository.Setup(r => r.ExistsAsync(command.ServiceProgramID)).ReturnsAsync(true);
        var expectedEntity = new ServiceSchedule
        {
            ID = 42,
            ServiceProgramID = command.ServiceProgramID,
            Name = command.Name,
            TimeIntervalValue = command.TimeIntervalValue,
            TimeIntervalUnit = command.TimeIntervalUnit,
            TimeBufferValue = command.TimeBufferValue,
            TimeBufferUnit = command.TimeBufferUnit,
            MileageInterval = command.MileageInterval,
            MileageBuffer = command.MileageBuffer,
            FirstServiceTimeValue = command.FirstServiceTimeValue,
            FirstServiceTimeUnit = command.FirstServiceTimeUnit,
            FirstServiceMileage = command.FirstServiceMileage,
            IsActive = command.IsActive,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceScheduleTasks = [],
            ServiceProgram = null!
        };
        _mockScheduleRepository.Setup(r => r.AddAsync(It.IsAny<ServiceSchedule>())).ReturnsAsync(expectedEntity);
        _mockScheduleRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockProgramRepository.Verify(r => r.ExistsAsync(command.ServiceProgramID), Times.Once);
        _mockScheduleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_ServiceProgram_Not_Found()
    {
        var command = CreateValidCommand(serviceProgramId: 99);
        SetupValidValidation(command);
        _mockProgramRepository.Setup(r => r.ExistsAsync(command.ServiceProgramID)).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockProgramRepository.Verify(r => r.ExistsAsync(command.ServiceProgramID), Times.Once);
        _mockScheduleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        var command = CreateValidCommand();
        SetupInvalidValidation(command);

        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockProgramRepository.Verify(r => r.ExistsAsync(It.IsAny<int>()), Times.Never);
        _mockScheduleRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}