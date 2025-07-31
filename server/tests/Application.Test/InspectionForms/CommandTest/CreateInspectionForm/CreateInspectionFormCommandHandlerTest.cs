using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.InspectionForms.Command.CreateInspectionForm;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.InspectionForms.CommandTest.CreateInspectionForm;

public class CreateInspectionFormCommandHandlerTest
{
    private readonly CreateInspectionFormCommandHandler _commandHandler;
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository;
    private readonly Mock<IValidator<CreateInspectionFormCommand>> _mockValidator;
    private readonly Mock<IAppLogger<CreateInspectionFormCommandHandler>> _mockLogger;
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedDate = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);

    public CreateInspectionFormCommandHandlerTest()
    {
        _mockInspectionFormRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionFormMappingProfile>());
        _mapper = config.CreateMapper();

        _commandHandler = new CreateInspectionFormCommandHandler(
            _mockInspectionFormRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    // Valid command factory
    private static CreateInspectionFormCommand CreateValidCommand(
        string title = "Test Inspection Title",
        string? description = "Test Inspection Description",
        bool isActive = true) => new(title, description, isActive);

    private void SetupValidValidation(CreateInspectionFormCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateInspectionFormCommand command, string propertyName = nameof(CreateInspectionFormCommand.Title), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handle_Should_Return_InspectionFormID_On_Success()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var expectedInspectionForm = new InspectionForm
        {
            ID = 12,
            CreatedAt = FixedDate,
            UpdatedAt = FixedDate,
            Title = command.Title,
            Description = command.Description,
            IsActive = command.IsActive,
            Inspections = [],
            InspectionFormItems = []
        };

        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title)).ReturnsAsync(true);
        _mockInspectionFormRepository.Setup(r => r.AddAsync(It.IsAny<InspectionForm>())).ReturnsAsync(expectedInspectionForm);
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedInspectionForm.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.IsTitleUniqueAsync(command.Title), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.AddAsync(It.IsAny<InspectionForm>()), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupInvalidValidation(command, nameof(CreateInspectionFormCommand.Title), "Title is required");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Equal("Title is required", exception.Message);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.IsTitleUniqueAsync(It.IsAny<string>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.AddAsync(It.IsAny<InspectionForm>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_DuplicateEntityException_When_Title_Already_Exists()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title)).ReturnsAsync(false);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<DuplicateEntityException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Contains("InspectionForm", exception.Message);
        Assert.Contains("Title", exception.Message);
        Assert.Contains(command.Title, exception.Message);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.IsTitleUniqueAsync(command.Title), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.AddAsync(It.IsAny<InspectionForm>()), Times.Never);
        _mockInspectionFormRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Map_Command_Properties_Correctly()
    {
        // Arrange
        var command = CreateValidCommand(
            title: "Custom Inspection Form",
            description: "Custom description for testing mapping",
            isActive: false
        );
        SetupValidValidation(command);

        InspectionForm? capturedInspectionForm = null;
        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title)).ReturnsAsync(true);
        _mockInspectionFormRepository.Setup(r => r.AddAsync(It.IsAny<InspectionForm>()))
            .Callback<InspectionForm>(form => capturedInspectionForm = form)
            .ReturnsAsync(new InspectionForm
            {
                ID = 1,
                CreatedAt = FixedDate,
                UpdatedAt = FixedDate,
                Title = command.Title,
                Description = command.Description,
                IsActive = command.IsActive,
                Inspections = [],
                InspectionFormItems = []
            });
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedInspectionForm);
        Assert.Equal(command.Title, capturedInspectionForm.Title);
        Assert.Equal(command.Description, capturedInspectionForm.Description);
        Assert.Equal(command.IsActive, capturedInspectionForm.IsActive);
        Assert.Equal(0, capturedInspectionForm.ID); // Should be 0 before persistence
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public async Task Handle_Should_Throw_BadRequestException_For_Invalid_Title(string? invalidTitle)
    {
        // Arrange
        var command = CreateValidCommand(title: invalidTitle!);
        SetupInvalidValidation(command, nameof(CreateInspectionFormCommand.Title), "Title is required");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Equal("Title is required", exception.Message);
    }

    [Fact]
    public async Task Handle_Should_Handle_Null_Description_Correctly()
    {
        // Arrange
        var command = CreateValidCommand(description: null);
        SetupValidValidation(command);

        InspectionForm? capturedInspectionForm = null;
        _mockInspectionFormRepository.Setup(r => r.IsTitleUniqueAsync(command.Title)).ReturnsAsync(true);
        _mockInspectionFormRepository.Setup(r => r.AddAsync(It.IsAny<InspectionForm>()))
            .Callback<InspectionForm>(form => capturedInspectionForm = form)
            .ReturnsAsync(new InspectionForm
            {
                ID = 1,
                CreatedAt = FixedDate,
                UpdatedAt = FixedDate,
                Title = command.Title,
                Description = null,
                IsActive = command.IsActive,
                Inspections = [],
                InspectionFormItems = []
            });
        _mockInspectionFormRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedInspectionForm);
        Assert.Null(capturedInspectionForm.Description);
    }
}