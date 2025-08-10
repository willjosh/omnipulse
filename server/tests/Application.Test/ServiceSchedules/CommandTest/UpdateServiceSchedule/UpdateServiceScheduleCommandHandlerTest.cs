using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceReminders.Command.GenerateServiceReminders;
using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

using Moq;

namespace Application.Test.ServiceSchedules.CommandTest.UpdateServiceSchedule;

public class UpdateServiceScheduleCommandHandlerTest
{
    private readonly UpdateServiceScheduleCommandHandler _commandHandler;
    private readonly Mock<IServiceScheduleRepository> _mockScheduleRepository = new();
    private readonly Mock<IServiceProgramRepository> _mockProgramRepository = new();
    private readonly Mock<IXrefServiceScheduleServiceTaskRepository> _mockXrefRepository = new();
    private readonly Mock<IServiceReminderRepository> _mockReminderRepository = new();
    private readonly Mock<ISender> _mockSender = new();
    private readonly Mock<IValidator<UpdateServiceScheduleCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<UpdateServiceScheduleCommandHandler>> _mockLogger = new();

    public UpdateServiceScheduleCommandHandlerTest()
    {
        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceScheduleMappingProfile>());
        var mapper = config.CreateMapper();

        _commandHandler = new UpdateServiceScheduleCommandHandler(
            _mockScheduleRepository.Object,
            _mockProgramRepository.Object,
            _mockXrefRepository.Object,
            _mockReminderRepository.Object,
            _mockSender.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            mapper);
    }

    private static UpdateServiceScheduleCommand CreateValidCommand(
        int serviceScheduleID = 1,
        int serviceProgramId = 1,
        string name = "5000 km / 6-week service",
        List<int>? serviceTaskIDs = null,
        int? timeIntervalValue = 6,
        TimeUnitEnum? timeIntervalUnit = TimeUnitEnum.Weeks,
        int? timeBufferValue = 1,
        TimeUnitEnum? timeBufferUnit = TimeUnitEnum.Days,
        int? mileageInterval = 5000,
        int? mileageBuffer = 250,
        DateTime? firstServiceDate = null,
        int? firstServiceMileage = null,
        bool isActive = true) => new(
            ServiceScheduleID: serviceScheduleID,
            ServiceProgramID: serviceProgramId,
            Name: name,
            ServiceTaskIDs: serviceTaskIDs ?? [1, 2],
            TimeIntervalValue: timeIntervalValue,
            TimeIntervalUnit: timeIntervalUnit,
            TimeBufferValue: timeBufferValue,
            TimeBufferUnit: timeBufferUnit,
            MileageInterval: mileageInterval,
            MileageBuffer: mileageBuffer,
            FirstServiceDate: firstServiceDate,
            FirstServiceMileage: firstServiceMileage,
            IsActive: isActive);

    private void SetupValidValidation(UpdateServiceScheduleCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(UpdateServiceScheduleCommand command, string propertyName = "Name", string errorMessage = "Validation failed")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handle_ValidCommand_ReturnsServiceScheduleID()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockProgramRepository.Setup(r => r.ExistsAsync(command.ServiceProgramID)).ReturnsAsync(true);
        _mockScheduleRepository.Setup(r => r.GetByIdAsync(command.ServiceScheduleID)).ReturnsAsync(new ServiceSchedule
        {
            ID = command.ServiceScheduleID,
            ServiceProgramID = command.ServiceProgramID,
            Name = command.Name,
            TimeIntervalValue = command.TimeIntervalValue,
            TimeIntervalUnit = command.TimeIntervalUnit,
            MileageInterval = command.MileageInterval,
            TimeBufferValue = command.TimeBufferValue,
            TimeBufferUnit = command.TimeBufferUnit,
            MileageBuffer = command.MileageBuffer,
            FirstServiceDate = command.FirstServiceDate,
            FirstServiceMileage = command.FirstServiceMileage,
            IsActive = true,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            XrefServiceScheduleServiceTasks = [],
            ServiceProgram = new ServiceProgram
            {
                ID = command.ServiceProgramID,
                Name = "Prog",
                IsActive = true,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                ServiceSchedules = [],
                XrefServiceProgramVehicles = []
            }
        });

        _mockReminderRepository.Setup(r => r.DeleteNonFinalRemindersForScheduleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>())).ReturnsAsync(0);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.ServiceScheduleID, result);
        _mockReminderRepository.Verify(r => r.DeleteNonFinalRemindersForScheduleAsync(It.IsAny<int>(), It.IsAny<CancellationToken>()), Times.Once);
        _mockSender.Verify(s => s.Send(It.IsAny<GenerateServiceRemindersCommand>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task Handle_InvalidValidation_ThrowsBadRequestException()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupInvalidValidation(command);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ServiceScheduleNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockScheduleRepository.Setup(r => r.GetByIdAsync(command.ServiceScheduleID)).ReturnsAsync((ServiceSchedule?)null);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }

    [Fact]
    public async Task Handle_ServiceProgramNotFound_ThrowsEntityNotFoundException()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var existingSchedule = new ServiceSchedule
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceProgramID = 2,
            Name = "Old Name",
            TimeIntervalValue = 90,
            TimeIntervalUnit = TimeUnitEnum.Days,
            TimeBufferValue = 10,
            TimeBufferUnit = TimeUnitEnum.Days,
            MileageInterval = null,
            MileageBuffer = null,
            FirstServiceDate = null,
            FirstServiceMileage = null,
            IsActive = true,
            XrefServiceScheduleServiceTasks = [],
            ServiceProgram = new ServiceProgram
            {
                ID = 2,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Program 2",
                XrefServiceProgramVehicles = [],
                ServiceSchedules = [],
                IsActive = true
            }
        };
        _mockScheduleRepository.Setup(r => r.GetByIdAsync(command.ServiceScheduleID)).ReturnsAsync(existingSchedule);
        _mockProgramRepository.Setup(r => r.ExistsAsync(command.ServiceProgramID)).ReturnsAsync(false);

        // Act & Assert
        await Assert.ThrowsAsync<EntityNotFoundException>(() => _commandHandler.Handle(command, CancellationToken.None));
    }
}