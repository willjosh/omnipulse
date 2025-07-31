using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionForms.Command.DeactivateInspectionForm;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionForms.CommandTest.DeactivateInspectionForm;

public class DeactivateInspectionFormCommandHandlerTest
{
    private readonly DeactivateInspectionFormCommandHandler _commandHandler;
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository = new();
    private readonly Mock<IValidator<DeactivateInspectionFormCommand>> _mockValidator = new();
    private readonly Mock<IAppLogger<DeactivateInspectionFormCommandHandler>> _mockLogger = new();

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public DeactivateInspectionFormCommandHandlerTest()
    {
        _commandHandler = new DeactivateInspectionFormCommandHandler(
            _mockInspectionFormRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    private static DeactivateInspectionFormCommand CreateValidCommand(int inspectionFormID = 1) => new(inspectionFormID);

    private static InspectionForm CreateInspectionFormEntity(
        int id = 1,
        string title = $"Test {nameof(InspectionForm)} Title",
        string? description = $"Test {nameof(InspectionForm)} Description",
        bool isActive = true,
        bool hasInspections = false)
    {
        var inspections = hasInspections ? [null!] : new List<Inspection>(); // Just for count

        return new InspectionForm
        {
            ID = id,
            Title = title,
            Description = description,
            IsActive = isActive,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = inspections,
            InspectionFormItems = []
        };
    }

    private void SetupValidValidation(DeactivateInspectionFormCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(DeactivateInspectionFormCommand command, string propertyName = nameof(DeactivateInspectionFormCommand.InspectionFormID), string errorMessage = "Invalid ID")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_ID_When_Deletion_Successful()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity();
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.InspectionFormID, result);
        Assert.False(inspectionForm.IsActive); // Verify soft delete
        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.Update(inspectionForm), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_When_InspectionForm_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync((InspectionForm?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Equal(nameof(InspectionForm), exception.EntityName);
        Assert.Equal(nameof(InspectionForm.ID), exception.PropertyName);
        Assert.Equal(command.InspectionFormID.ToString(), exception.PropertyValue);

        _mockInspectionFormRepository.Verify(r => r.Update(It.IsAny<InspectionForm>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand(inspectionFormID: -1); // Invalid ID
        SetupInvalidValidation(command);

        // Act & Assert
        await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        _mockValidator.Verify(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.Update(It.IsAny<InspectionForm>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_When_InspectionForm_Is_Already_Inactive()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity(isActive: false); // Already inactive (soft deleted)
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Contains("already inactive", exception.Message);
        Assert.Contains("already deactivated", exception.Message);

        _mockInspectionFormRepository.Verify(r => r.Update(It.IsAny<InspectionForm>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Allow_Soft_Deletion_When_InspectionForm_Has_Inspections()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity(hasInspections: true); // Has associated inspections - still allowed for soft delete
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.InspectionFormID, result);
        Assert.False(inspectionForm.IsActive); // Verify soft delete
        _mockInspectionFormRepository.Verify(r => r.Update(inspectionForm), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Allow_Soft_Deletion_When_Inspections_Collection_Is_Null()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionForm = new InspectionForm
        {
            ID = 1,
            Title = "Test Form",
            Description = "Test Description",
            IsActive = true,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Inspections = null!, // Null collection
            InspectionFormItems = []
        };
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.InspectionFormID, result);
        Assert.False(inspectionForm.IsActive); // Verify soft delete
        _mockInspectionFormRepository.Verify(r => r.Update(inspectionForm), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Theory]
    [InlineData(1)]
    [InlineData(100)]
    [InlineData(999)]
    public async Task Handler_Should_Work_With_Different_IDs(int inspectionFormID)
    {
        // Arrange
        var command = CreateValidCommand(inspectionFormID);
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity(id: inspectionFormID);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(inspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(inspectionFormID, result);
    }

    [Fact]
    public async Task Handler_Should_Allow_Soft_Deletion_When_InspectionForm_Has_No_Inspections()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity(hasInspections: false); // No associated inspections
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(command.InspectionFormID, result);
        Assert.False(inspectionForm.IsActive); // Verify soft delete
        _mockInspectionFormRepository.Verify(r => r.Update(inspectionForm), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Log_Information_Messages()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);
        var inspectionForm = CreateInspectionFormEntity();
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Handling DeactivateInspectionFormCommand"))),
            Times.Once);
        _mockLogger.Verify(
            x => x.LogInformation(It.Is<string>(s => s.Contains("Successfully soft deleted (deactivated)"))),
            Times.Once);
    }
}