using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.ServiceSchedules.CommandTest.UpdateServiceSchedule;

public class UpdateServiceScheduleCommandHandlerTest
{
    private readonly UpdateServiceScheduleCommandHandler _commandHandler;
    private readonly Mock<IServiceScheduleRepository> _mockScheduleRepository = new();
    private readonly Mock<IServiceProgramRepository> _mockProgramRepository = new();
    private readonly Mock<IXrefServiceScheduleServiceTaskRepository> _mockXrefRepository = new();
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
        int? firstServiceTimeValue = null,
        TimeUnitEnum? firstServiceTimeUnit = null,
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
            FirstServiceTimeValue: firstServiceTimeValue,
            FirstServiceTimeUnit: firstServiceTimeUnit,
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
        var serviceTaskIDs = new List<int> { 1, 2 };
        var command = CreateValidCommand(serviceTaskIDs: serviceTaskIDs);
        SetupValidValidation(command);

        var existingSchedule = new ServiceSchedule
        {
            ID = 1,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            ServiceProgramID = 1,
            Name = "Old Name",
            TimeIntervalValue = 90,
            TimeIntervalUnit = TimeUnitEnum.Days,
            TimeBufferValue = 10,
            TimeBufferUnit = TimeUnitEnum.Days,
            MileageInterval = null,
            MileageBuffer = null,
            FirstServiceTimeValue = null,
            FirstServiceTimeUnit = null,
            FirstServiceMileage = null,
            IsActive = true,
            XrefServiceScheduleServiceTasks = [],
            ServiceProgram = new ServiceProgram
            {
                ID = 1,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,
                Name = "Program",
                XrefServiceProgramVehicles = [],
                ServiceSchedules = [],
                IsActive = true
            }
        };
        _mockScheduleRepository.Setup(r => r.GetByIdAsync(command.ServiceScheduleID)).ReturnsAsync(existingSchedule);
        _mockProgramRepository.Setup(r => r.ExistsAsync(command.ServiceProgramID)).ReturnsAsync(true);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(existingSchedule.ID, result);
        _mockXrefRepository.Verify(r => r.AddRangeAsync(
            It.Is<List<XrefServiceScheduleServiceTask>>(xrefs =>
                xrefs.Count == serviceTaskIDs.Count &&
                serviceTaskIDs.All(id => xrefs.Any(x => x.ServiceTaskID == id && x.ServiceScheduleID == existingSchedule.ID))
            )), Times.Once);
        _mockScheduleRepository.Verify(r => r.Update(existingSchedule), Times.Once);
        _mockScheduleRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
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
            FirstServiceTimeValue = null,
            FirstServiceTimeUnit = null,
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