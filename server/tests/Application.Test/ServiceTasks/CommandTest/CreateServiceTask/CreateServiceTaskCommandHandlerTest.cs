using System;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

using Xunit;

namespace Application.Test.ServiceTasks.CommandTest.CreateServiceTask;

public class CreateServiceTaskCommandHandlerTest
{
    private readonly CreateServiceTaskCommandHandler _commandHandler;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IValidator<CreateServiceTaskCommand>> _mockValidator;
    private readonly Mock<IAppLogger<CreateServiceTaskCommandHandler>> _mockLogger;
    private readonly IMapper _mapper;

    public CreateServiceTaskCommandHandlerTest()
    {
        _mockServiceTaskRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceTaskMappingProfile>());
        _mapper = config.CreateMapper();

        _commandHandler = new CreateServiceTaskCommandHandler(
            _mockServiceTaskRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    private CreateServiceTaskCommand CreateValidCommand(
        string name = "Engine Oil Change",
        string? description = "Replace engine oil and filter",
        double estimatedLabourHours = 1.5,
        decimal estimatedCost = 85.50m,
        ServiceTaskCategoryEnum category = ServiceTaskCategoryEnum.PREVENTIVE,
        bool isActive = true)
    {
        return new CreateServiceTaskCommand(
            Name: name,
            Description: description,
            EstimatedLabourHours: estimatedLabourHours,
            EstimatedCost: estimatedCost,
            Category: category,
            IsActive: isActive
        );
    }

    private void SetupValidValidation(CreateServiceTaskCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateServiceTaskCommand command, string propertyName, string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult([
            new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)
        ]);
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ServiceTaskID_On_Success()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var expectedServiceTask = new ServiceTask
        {
            ID = 100,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Name = command.Name,
            Description = command.Description,
            EstimatedLabourHours = command.EstimatedLabourHours,
            EstimatedCost = command.EstimatedCost,
            Category = command.Category,
            IsActive = command.IsActive,
            ServiceScheduleTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = []
        };

        _mockServiceTaskRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(false);
        _mockServiceTaskRepository.Setup(r => r.AddAsync(It.IsAny<ServiceTask>())).ReturnsAsync(expectedServiceTask);
        _mockServiceTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(expectedServiceTask.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.IsNameUniqueAsync(command.Name), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_When_Name_Exists()
    {
        // Given
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(() => _commandHandler.Handle(command, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.IsNameUniqueAsync(command.Name), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "Name", "Name is required");

        _mockServiceTaskRepository.Setup(r => r.IsNameUniqueAsync(command.Name)).ReturnsAsync(false);

        // When & Then
        await Assert.ThrowsAsync<BadRequestException>(() => _commandHandler.Handle(command, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.IsNameUniqueAsync(It.IsAny<string>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }
}