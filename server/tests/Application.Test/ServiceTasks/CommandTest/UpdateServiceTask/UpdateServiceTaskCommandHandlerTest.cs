using System;
using System.Collections.Generic;

using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.ServiceTasks.Command.UpdateServiceTask;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

using Xunit;

namespace Application.Test.ServiceTasks.CommandTest.UpdateServiceTask;

public class UpdateServiceTaskCommandHandlerTest
{
    private readonly UpdateServiceTaskCommandHandler _updateServiceTaskCommandHandler;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IValidator<UpdateServiceTaskCommand>> _mockValidator;
    private readonly Mock<IAppLogger<UpdateServiceTaskCommandHandler>> _mockLogger;

    public UpdateServiceTaskCommandHandlerTest()
    {
        _mockServiceTaskRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<ServiceTaskMappingProfile>());
        var mapper = config.CreateMapper();

        _updateServiceTaskCommandHandler = new(
            _mockServiceTaskRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            mapper);
    }

    // Create valid command with minimal data
    private static UpdateServiceTaskCommand CreateValidCommand(
        int serviceTaskID = 123,
        string name = "Updated Service Task",
        string? description = "Updated test description",
        double estimatedLabourHours = 2.5,
        decimal estimatedCost = 150.00m,
        ServiceTaskCategoryEnum category = ServiceTaskCategoryEnum.PREVENTIVE,
        bool isActive = true)
    {
        return new UpdateServiceTaskCommand(
            ServiceTaskID: serviceTaskID,
            Name: name,
            Description: description,
            EstimatedLabourHours: estimatedLabourHours,
            EstimatedCost: estimatedCost,
            Category: category,
            IsActive: isActive
        );
    }

    // Create existing service task
    private static ServiceTask CreateExistingServiceTask(
        int id = 123,
        string name = "Original Service Task",
        string? description = "Original description")
    {
        return new ServiceTask()
        {
            ID = id,
            CreatedAt = DateTime.UtcNow.AddDays(-30),
            UpdatedAt = DateTime.UtcNow.AddDays(-1),
            Name = name,
            Description = description,
            EstimatedLabourHours = 1.5,
            EstimatedCost = 100.00m,
            Category = ServiceTaskCategoryEnum.CORRECTIVE,
            IsActive = true,
            ServiceScheduleTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = []
        };
    }

    // Set up valid validation result
    private void SetupValidValidation(UpdateServiceTaskCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    // Set up validation failure
    private void SetupInvalidValidation(UpdateServiceTaskCommand command, string propertyName, string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ServiceTaskID_On_Successfully_Updating_ServiceTask()
    {
        // Given
        var command = CreateValidCommand();
        var existingServiceTask = CreateExistingServiceTask();

        // Setup validation to pass
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync(existingServiceTask);
        _mockServiceTaskRepository.Setup(r => r.DoesNameExistExcludingIdAsync(command.Name, command.ServiceTaskID)).ReturnsAsync(false);
        _mockServiceTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.ServiceTaskID, result);
        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.Update(It.IsAny<ServiceTask>()), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);

        // Verify that Update was called with a service task containing the updated values
        _mockServiceTaskRepository.Verify(
            r => r.Update(It.Is<ServiceTask>(st =>
                st.ID == command.ServiceTaskID &&
                st.Name == command.Name &&
                st.Description == command.Description &&
                st.EstimatedLabourHours == command.EstimatedLabourHours &&
                st.EstimatedCost == command.EstimatedCost &&
                st.Category == command.Category &&
                st.IsActive == command.IsActive)),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_Invalid_ServiceTaskID()
    {
        // Given
        var command = CreateValidCommand(serviceTaskID: 999);

        // Setup validation to pass
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync((ServiceTask?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.DoesNameExistExcludingIdAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.Update(It.IsAny<ServiceTask>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_DuplicateEntityException_On_Duplicate_Name_From_Different_ServiceTask()
    {
        // Given
        var command = CreateValidCommand(name: "Duplicate Service Task Name");
        var existingServiceTask = CreateExistingServiceTask(name: "Original Service Task Name"); // Different name

        // Setup validation to pass
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync(existingServiceTask);
        _mockServiceTaskRepository.Setup(r => r.DoesNameExistExcludingIdAsync(command.Name, command.ServiceTaskID)).ReturnsAsync(true);

        // When & Then
        await Assert.ThrowsAsync<DuplicateEntityException>(
            () => _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None)
        );

        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.DoesNameExistExcludingIdAsync(command.Name, command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.Update(It.IsAny<ServiceTask>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Allow_Update_With_Same_Name_As_Current_ServiceTask()
    {
        // Given - updating service task with the same name it already has (should be allowed)
        var sameName = "Current Service Task Name";
        var command = CreateValidCommand(name: sameName);
        var existingServiceTask = CreateExistingServiceTask(name: sameName); // Same name

        // Setup validation to pass
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync(existingServiceTask);
        _mockServiceTaskRepository.Setup(r => r.DoesNameExistExcludingIdAsync(command.Name, command.ServiceTaskID)).ReturnsAsync(false);
        _mockServiceTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.ServiceTaskID, result);
        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.DoesNameExistExcludingIdAsync(command.Name, command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.Update(It.IsAny<ServiceTask>()), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Check_ServiceTask_Existence_Before_Name_Duplication()
    {
        // Given - non-existent service task
        var command = CreateValidCommand();

        // Setup validation to pass
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync((ServiceTask?)null);

        // When & Then
        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None)
        );

        // Verify that name check was not attempted since service task doesn't exist
        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(command.ServiceTaskID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.DoesNameExistExcludingIdAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.Update(It.IsAny<ServiceTask>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        // Given
        var command = CreateValidCommand();

        // Setup validation to fail
        SetupInvalidValidation(command, "Name", "Service task name is required");

        // When & Then
        var exception = await Assert.ThrowsAsync<BadRequestException>(
            () => _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None)
        );

        Assert.Contains("Service task name is required", exception.Message);

        // Verify that no repository methods were called due to validation failure
        _mockServiceTaskRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.DoesNameExistExcludingIdAsync(It.IsAny<string>(), It.IsAny<int>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.Update(It.IsAny<ServiceTask>()), Times.Never);
        _mockServiceTaskRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Update_All_Properties_Correctly()
    {
        // Given - command with all different values from existing entity
        var command = CreateValidCommand(
            serviceTaskID: 123,
            name: "Completely New Name",
            description: "Completely new description",
            estimatedLabourHours: 5.5,
            estimatedCost: 350.75m,
            category: ServiceTaskCategoryEnum.EMERGENCY,
            isActive: false
        );

        var existingServiceTask = CreateExistingServiceTask(
            id: 123,
            name: "Old Name",
            description: "Old description"
        );

        // Setup validation to pass
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync(existingServiceTask);
        _mockServiceTaskRepository.Setup(r => r.DoesNameExistExcludingIdAsync(command.Name, command.ServiceTaskID)).ReturnsAsync(false);
        _mockServiceTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.ServiceTaskID, result);

        // Verify the entity was updated with all the new values
        _mockServiceTaskRepository.Verify(
            r => r.Update(It.Is<ServiceTask>(st =>
                st.ID == 123 &&
                st.Name == "Completely New Name" &&
                st.Description == "Completely new description" &&
                st.EstimatedLabourHours == 5.5 &&
                st.EstimatedCost == 350.75m &&
                st.Category == ServiceTaskCategoryEnum.EMERGENCY &&
                st.IsActive == false)),
            Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Handle_Null_Description_Correctly()
    {
        // Given
        var command = CreateValidCommand(description: null);
        var existingServiceTask = CreateExistingServiceTask();

        // Setup validation to pass
        SetupValidValidation(command);

        _mockServiceTaskRepository.Setup(r => r.GetByIdAsync(command.ServiceTaskID)).ReturnsAsync(existingServiceTask);
        _mockServiceTaskRepository.Setup(r => r.DoesNameExistExcludingIdAsync(command.Name, command.ServiceTaskID)).ReturnsAsync(false);
        _mockServiceTaskRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // When
        var result = await _updateServiceTaskCommandHandler.Handle(command, CancellationToken.None);

        // Then
        Assert.Equal(command.ServiceTaskID, result);

        // Verify that null description was handled correctly
        _mockServiceTaskRepository.Verify(
            r => r.Update(It.Is<ServiceTask>(st =>
                st.ID == command.ServiceTaskID &&
                st.Name == command.Name &&
                st.Description == null &&
                st.EstimatedLabourHours == command.EstimatedLabourHours &&
                st.EstimatedCost == command.EstimatedCost &&
                st.Category == command.Category &&
                st.IsActive == command.IsActive)),
            Times.Once);
    }
}