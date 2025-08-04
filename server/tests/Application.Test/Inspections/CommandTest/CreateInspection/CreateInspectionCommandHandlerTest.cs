using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.Inspections.Command.CreateInspection;
using Application.MappingProfiles;

using AutoMapper;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;
using FluentValidation.Results;

using Moq;

namespace Application.Test.Inspections.CommandTest.CreateInspection;

public class CreateInspectionCommandHandlerTest
{
    private readonly CreateInspectionCommandHandler _commandHandler;
    private readonly Mock<IInspectionRepository> _mockInspectionRepository;
    private readonly Mock<IInspectionFormRepository> _mockInspectionFormRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IIssueRepository> _mockIssueRepository;
    private readonly Mock<IValidator<CreateInspectionCommand>> _mockValidator;
    private readonly Mock<IAppLogger<CreateInspectionCommandHandler>> _mockLogger;
    private readonly IMapper _mapper;

    // Constants
    private static readonly DateTime FixedStartTime = new(2025, 6, 2, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime FixedEndTime = new(2025, 6, 2, 9, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime FixedCreatedTime = new(2025, 6, 2, 9, 30, 0, DateTimeKind.Utc);

    public CreateInspectionCommandHandlerTest()
    {
        _mockInspectionRepository = new();
        _mockInspectionFormRepository = new();
        _mockVehicleRepository = new();
        _mockUserRepository = new();
        _mockIssueRepository = new();
        _mockValidator = new();
        _mockLogger = new();

        var config = new MapperConfiguration(cfg => cfg.AddProfile<InspectionMappingProfile>());
        _mapper = config.CreateMapper();

        _commandHandler = new CreateInspectionCommandHandler(
            _mockInspectionRepository.Object,
            _mockInspectionFormRepository.Object,
            _mockVehicleRepository.Object,
            _mockUserRepository.Object,
            _mockIssueRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object,
            _mapper
        );
    }

    private static CreateInspectionCommand CreateValidCommand(
        int inspectionFormId = 1,
        int vehicleId = 1,
        string technicianId = "1b6ed760-b26c-4800-8e86-c888d002b9c1",
        DateTime? inspectionStartTime = null,
        DateTime? inspectionEndTime = null,
        double? odometerReading = 50000.0,
        VehicleConditionEnum vehicleCondition = VehicleConditionEnum.Excellent,
        string? notes = "Test inspection notes",
        ICollection<CreateInspectionPassFailItemCommand>? inspectionItems = null)
    {
        var items = inspectionItems ??
        [
            new(1, true, "Test comment for item 1"),
            new(2, true, "Test comment for item 2"),
            new(3, true, "Test comment for item 3")
        ];

        return new CreateInspectionCommand(
            inspectionFormId,
            vehicleId,
            technicianId,
            inspectionStartTime ?? FixedStartTime,
            inspectionEndTime ?? FixedEndTime,
            odometerReading,
            vehicleCondition,
            notes,
            items
        );
    }

    private static CreateInspectionCommand CreateValidCommandWithFailedItems(
        int inspectionFormId = 1,
        int vehicleId = 1,
        string technicianId = "1b6ed760-b26c-4800-8e86-c888d002b9c1",
        DateTime? inspectionStartTime = null,
        DateTime? inspectionEndTime = null,
        double? odometerReading = 50000.0,
        VehicleConditionEnum vehicleCondition = VehicleConditionEnum.Excellent,
        string? notes = "Test inspection notes")
    {
        var items = new List<CreateInspectionPassFailItemCommand>
        {
            new(1, true, "Test comment for passed item 1"),
            new(2, false, "Test comment for failed item 1"), // Failed item
            new(3, true, "Test comment for passed item 2"),
            new(4, false, "Test comment for failed item 2")  // Failed item
        };

        return new CreateInspectionCommand(
            inspectionFormId,
            vehicleId,
            technicianId,
            inspectionStartTime ?? FixedStartTime,
            inspectionEndTime ?? FixedEndTime,
            odometerReading,
            vehicleCondition,
            notes,
            items
        );
    }

    private void SetupValidValidation(CreateInspectionCommand command)
    {
        var validResult = new ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateInspectionCommand command, string propertyName = nameof(CreateInspectionCommand.InspectionFormID), string errorMessage = "Invalid Validation")
    {
        var invalidResult = new ValidationResult([new ValidationFailure(propertyName, errorMessage)]);
        _mockValidator.Setup(v => v.ValidateAsync(command, It.IsAny<CancellationToken>()))
            .ReturnsAsync(invalidResult);
    }

    private void SetupSuccessfulEntityRetrieval(CreateInspectionCommand command)
    {
        var inspectionForm = CreateMockInspectionForm();
        var vehicle = CreateMockVehicle();
        var technician = CreateMockUser();

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID))
            .ReturnsAsync(vehicle);
        _mockUserRepository.Setup(r => r.GetTechnicianByIdAsync(command.TechnicianID))
            .ReturnsAsync(technician);

        // Setup issue repository mocks
        _mockIssueRepository.Setup(r => r.AddAsync(It.IsAny<Issue>()))
            .ReturnsAsync((Issue issue) => issue);
        _mockIssueRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);
    }

    private static InspectionForm CreateMockInspectionForm(bool isActive = true)
    {
        var inspectionForm = new InspectionForm
        {
            ID = 1,
            Title = "Test Form",
            Description = "Test Description",
            IsActive = isActive,
            CreatedAt = FixedCreatedTime,
            UpdatedAt = FixedCreatedTime,
            InspectionFormItems = [],
            Inspections = []
        };

        inspectionForm.InspectionFormItems =
        [
            new()
            {
                ID = 1,
                InspectionFormID = 1,
                ItemLabel = "Test Item 1",
                ItemDescription = "Test Description 1",
                ItemInstructions = "Test Instructions 1",
                IsRequired = true,
                IsActive = true,
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionForm = inspectionForm
            },
            new()
            {
                ID = 2,
                InspectionFormID = 1,
                ItemLabel = "Test Item 2",
                ItemDescription = "Test Description 2",
                ItemInstructions = "Test Instructions 2",
                IsRequired = true,
                IsActive = true,
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionForm = inspectionForm
            },
            new()
            {
                ID = 3,
                InspectionFormID = 1,
                ItemLabel = "Test Item 3",
                ItemDescription = "Test Description 3",
                ItemInstructions = "Test Instructions 3",
                IsRequired = true,
                IsActive = true,
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionForm = inspectionForm
            },
            new()
            {
                ID = 4,
                InspectionFormID = 1,
                ItemLabel = "Test Item 4",
                ItemDescription = "Test Description 4",
                ItemInstructions = "Test Instructions 4",
                IsRequired = false,
                IsActive = true,
                InspectionFormItemTypeEnum = InspectionFormItemTypeEnum.PassFail,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionForm = inspectionForm
            }
        ];

        return inspectionForm;
    }

    private static Vehicle CreateMockVehicle()
    {
        return new Vehicle
        {
            ID = 1,
            Name = "Test Vehicle",
            Make = "Test Make",
            Model = "Test Model",
            Year = 2023,
            VIN = "TESTVIN123456789",
            LicensePlate = "TEST123",
            LicensePlateExpirationDate = FixedCreatedTime.AddYears(1),
            VehicleType = VehicleTypeEnum.CAR,
            VehicleGroupID = 1,
            Trim = "Standard",
            Mileage = 50000,
            EngineHours = 1000,
            FuelCapacity = 50,
            FuelType = FuelTypeEnum.PETROL,
            PurchaseDate = FixedCreatedTime.AddYears(-1),
            PurchasePrice = 25000,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Garage A",
            CreatedAt = FixedCreatedTime,
            UpdatedAt = FixedCreatedTime,
            VehicleGroup = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = []
        };
    }

    private static User CreateMockUser()
    {
        return new User
        {
            Id = "1b6ed760-b26c-4800-8e86-c888d002b9c1",
            FirstName = "John",
            LastName = "Doe",
            Email = "john.doe@example.com",
            HireDate = FixedCreatedTime.AddYears(-2),
            IsActive = true,
            CreatedAt = FixedCreatedTime,
            UpdatedAt = FixedCreatedTime,
            MaintenanceHistories = [],
            IssueAttachments = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            Inspections = [],
            Vehicles = [],
            InventoryTransactions = []
        };
    }

    [Fact]
    public async Task Handle_Should_Return_InspectionID_On_Success()
    {
        // Arrange
        var command = CreateValidCommandWithFailedItems();
        SetupValidValidation(command);
        SetupSuccessfulEntityRetrieval(command);

        var expectedInspection = new Inspection
        {
            ID = 12,
            InspectionFormID = command.InspectionFormID,
            VehicleID = command.VehicleID,
            TechnicianID = command.TechnicianID,
            InspectionStartTime = command.InspectionStartTime,
            InspectionEndTime = command.InspectionEndTime,
            OdometerReading = command.OdometerReading,
            VehicleCondition = command.VehicleCondition,
            Notes = command.Notes,
            SnapshotFormTitle = "Test Form",
            SnapshotFormDescription = "Test Description",
            CreatedAt = FixedCreatedTime,
            UpdatedAt = FixedCreatedTime,
            InspectionForm = null!,
            Vehicle = null!,
            User = null!,
            InspectionPassFailItems = []
        };

        _mockInspectionRepository.Setup(r => r.AddAsync(It.IsAny<Inspection>()))
            .ReturnsAsync(expectedInspection);
        _mockInspectionRepository.Setup(r => r.SaveChangesAsync())
            .ReturnsAsync(1);

        // Act
        var result = await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.Equal(expectedInspection.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetInspectionFormWithItemsAsync(command.InspectionFormID), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(r => r.GetTechnicianByIdAsync(command.TechnicianID), Times.Once);
        _mockInspectionRepository.Verify(r => r.AddAsync(It.IsAny<Inspection>()), Times.Once);
        _mockInspectionRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_Validation_Fails()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupInvalidValidation(command, nameof(CreateInspectionCommand.InspectionFormID), "Inspection form is required");

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Equal("Inspection form is required", exception.Message);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        _mockInspectionRepository.Verify(r => r.AddAsync(It.IsAny<Inspection>()), Times.Never);
        _mockInspectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_InspectionForm_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync((InspectionForm?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Contains("InspectionForm", exception.Message);
        Assert.Contains(command.InspectionFormID.ToString(), exception.Message);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockInspectionRepository.Verify(r => r.AddAsync(It.IsAny<Inspection>()), Times.Never);
        _mockInspectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Vehicle_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var inspectionForm = CreateMockInspectionForm();
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID))
            .ReturnsAsync((Vehicle?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Contains("Vehicle", exception.Message);
        Assert.Contains(command.VehicleID.ToString(), exception.Message);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockInspectionRepository.Verify(r => r.AddAsync(It.IsAny<Inspection>()), Times.Never);
        _mockInspectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_EntityNotFoundException_When_Technician_Not_Found()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var inspectionForm = CreateMockInspectionForm();
        var vehicle = CreateMockVehicle();
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockInspectionFormRepository.Setup(r => r.GetInspectionFormWithItemsAsync(command.InspectionFormID))
            .ReturnsAsync(inspectionForm);
        _mockVehicleRepository.Setup(r => r.GetByIdAsync(command.VehicleID))
            .ReturnsAsync(vehicle);
        _mockUserRepository.Setup(r => r.GetTechnicianByIdAsync(command.TechnicianID))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<EntityNotFoundException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Contains("User", exception.Message);
        Assert.Contains(command.TechnicianID, exception.Message);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockVehicleRepository.Verify(r => r.GetByIdAsync(command.VehicleID), Times.Once);
        _mockUserRepository.Verify(r => r.GetTechnicianByIdAsync(command.TechnicianID), Times.Once);
        _mockInspectionRepository.Verify(r => r.AddAsync(It.IsAny<Inspection>()), Times.Never);
        _mockInspectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Throw_BadRequestException_When_InspectionForm_Is_Inactive()
    {
        // Arrange
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var inactiveInspectionForm = CreateMockInspectionForm(isActive: false);
        _mockInspectionFormRepository.Setup(r => r.GetByIdAsync(command.InspectionFormID))
            .ReturnsAsync(inactiveInspectionForm);

        // Act & Assert
        var exception = await Assert.ThrowsAsync<BadRequestException>(() =>
            _commandHandler.Handle(command, CancellationToken.None));

        Assert.Contains("inactive", exception.Message.ToLower());
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockInspectionFormRepository.Verify(r => r.GetByIdAsync(command.InspectionFormID), Times.Once);
        _mockInspectionRepository.Verify(r => r.AddAsync(It.IsAny<Inspection>()), Times.Never);
        _mockInspectionRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handle_Should_Map_Command_Properties_Correctly()
    {
        // Arrange
        var command = CreateValidCommand(
            inspectionFormId: 5,
            vehicleId: 10,
            technicianId: "1b6ed760-b26c-4800-8e86-c888d002b9c1",
            inspectionStartTime: FixedStartTime,
            inspectionEndTime: FixedEndTime,
            odometerReading: 75000.5,
            vehicleCondition: VehicleConditionEnum.HasIssuesButSafeToOperate,
            notes: "Custom inspection notes for mapping test"
        );

        SetupValidValidation(command);
        SetupSuccessfulEntityRetrieval(command);

        Inspection? capturedInspection = null;
        _mockInspectionRepository.Setup(r => r.AddAsync(It.IsAny<Inspection>()))
            .Callback<Inspection>(inspection => capturedInspection = inspection)
            .ReturnsAsync(new Inspection
            {
                ID = 1,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionFormID = command.InspectionFormID,
                VehicleID = command.VehicleID,
                TechnicianID = command.TechnicianID,
                InspectionStartTime = command.InspectionStartTime,
                InspectionEndTime = command.InspectionEndTime,
                OdometerReading = command.OdometerReading,
                VehicleCondition = command.VehicleCondition,
                Notes = command.Notes,
                SnapshotFormTitle = "Test Form",
                SnapshotFormDescription = "Test Description",
                InspectionForm = null!,
                Vehicle = null!,
                User = null!,
                InspectionPassFailItems = []
            });
        _mockInspectionRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedInspection);
        Assert.Equal(command.InspectionFormID, capturedInspection.InspectionFormID);
        Assert.Equal(command.VehicleID, capturedInspection.VehicleID);
        Assert.Equal(command.TechnicianID, capturedInspection.TechnicianID);
        Assert.Equal(command.InspectionStartTime, capturedInspection.InspectionStartTime);
        Assert.Equal(command.InspectionEndTime, capturedInspection.InspectionEndTime);
        Assert.Equal(command.OdometerReading, capturedInspection.OdometerReading);
        Assert.Equal(command.VehicleCondition, capturedInspection.VehicleCondition);
        Assert.Equal(command.Notes, capturedInspection.Notes);
        Assert.Equal("Test Form", capturedInspection.SnapshotFormTitle);
        Assert.Equal("Test Description", capturedInspection.SnapshotFormDescription);
        Assert.Equal(0, capturedInspection.ID); // Should be 0 before persistence
    }

    [Fact]
    public async Task Handle_Should_Handle_Null_Notes_Correctly()
    {
        // Arrange
        var command = CreateValidCommand(notes: null);
        SetupValidValidation(command);
        SetupSuccessfulEntityRetrieval(command);

        Inspection? capturedInspection = null;
        _mockInspectionRepository.Setup(r => r.AddAsync(It.IsAny<Inspection>()))
            .Callback<Inspection>(inspection => capturedInspection = inspection)
            .ReturnsAsync(new Inspection
            {
                ID = 1,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionFormID = command.InspectionFormID,
                VehicleID = command.VehicleID,
                TechnicianID = command.TechnicianID,
                InspectionStartTime = command.InspectionStartTime,
                InspectionEndTime = command.InspectionEndTime,
                OdometerReading = command.OdometerReading,
                VehicleCondition = command.VehicleCondition,
                Notes = null,
                SnapshotFormTitle = "Test Form",
                SnapshotFormDescription = "Test Description",
                InspectionForm = null!,
                Vehicle = null!,
                User = null!,
                InspectionPassFailItems = []
            });
        _mockInspectionRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedInspection);
        Assert.Null(capturedInspection.Notes);
    }

    [Fact]
    public async Task Handle_Should_Handle_Null_OdometerReading_Correctly()
    {
        // Arrange
        var command = CreateValidCommand(odometerReading: null);
        SetupValidValidation(command);
        SetupSuccessfulEntityRetrieval(command);

        Inspection? capturedInspection = null;
        _mockInspectionRepository.Setup(r => r.AddAsync(It.IsAny<Inspection>()))
            .Callback<Inspection>(inspection => capturedInspection = inspection)
            .ReturnsAsync(new Inspection
            {
                ID = 1,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionFormID = command.InspectionFormID,
                VehicleID = command.VehicleID,
                TechnicianID = command.TechnicianID,
                InspectionStartTime = command.InspectionStartTime,
                InspectionEndTime = command.InspectionEndTime,
                OdometerReading = null,
                VehicleCondition = command.VehicleCondition,
                Notes = command.Notes,
                SnapshotFormTitle = "Test Form",
                SnapshotFormDescription = "Test Description",
                InspectionForm = null!,
                Vehicle = null!,
                User = null!,
                InspectionPassFailItems = []
            });
        _mockInspectionRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedInspection);
        Assert.Null(capturedInspection.OdometerReading);
    }

    [Theory]
    [InlineData(VehicleConditionEnum.Excellent)]
    [InlineData(VehicleConditionEnum.HasIssuesButSafeToOperate)]
    [InlineData(VehicleConditionEnum.NotSafeToOperate)]
    public async Task Handle_Should_Handle_Different_VehicleConditions_Correctly(VehicleConditionEnum condition)
    {
        // Arrange
        var command = CreateValidCommand(vehicleCondition: condition);
        SetupValidValidation(command);
        SetupSuccessfulEntityRetrieval(command);

        Inspection? capturedInspection = null;
        _mockInspectionRepository.Setup(r => r.AddAsync(It.IsAny<Inspection>()))
            .Callback<Inspection>(inspection => capturedInspection = inspection)
            .ReturnsAsync(new Inspection
            {
                ID = 1,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionFormID = command.InspectionFormID,
                VehicleID = command.VehicleID,
                TechnicianID = command.TechnicianID,
                InspectionStartTime = command.InspectionStartTime,
                InspectionEndTime = command.InspectionEndTime,
                OdometerReading = command.OdometerReading,
                VehicleCondition = condition,
                Notes = command.Notes,
                SnapshotFormTitle = "Test Form",
                SnapshotFormDescription = "Test Description",
                InspectionForm = null!,
                Vehicle = null!,
                User = null!,
                InspectionPassFailItems = []
            });
        _mockInspectionRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedInspection);
        Assert.Equal(condition, capturedInspection.VehicleCondition);
    }

    [Fact]
    public async Task Handle_Should_Create_InspectionPassFailItems_From_Command()
    {
        // Arrange
        var inspectionItems = new List<CreateInspectionPassFailItemCommand>
        {
            new(1, true, "Item 1 passed"),
            new(2, false, "Item 2 failed"),
            new(3, true, null) // Test null comment
        };
        var command = CreateValidCommand(inspectionItems: inspectionItems);

        SetupValidValidation(command);
        SetupSuccessfulEntityRetrieval(command);

        Inspection? capturedInspection = null;
        _mockInspectionRepository.Setup(r => r.AddAsync(It.IsAny<Inspection>()))
            .Callback<Inspection>(inspection => capturedInspection = inspection)
            .ReturnsAsync(new Inspection
            {
                ID = 1,
                CreatedAt = FixedCreatedTime,
                UpdatedAt = FixedCreatedTime,
                InspectionFormID = command.InspectionFormID,
                VehicleID = command.VehicleID,
                TechnicianID = command.TechnicianID,
                InspectionStartTime = command.InspectionStartTime,
                InspectionEndTime = command.InspectionEndTime,
                OdometerReading = command.OdometerReading,
                VehicleCondition = command.VehicleCondition,
                Notes = command.Notes,
                SnapshotFormTitle = "Test Form",
                SnapshotFormDescription = "Test Description",
                InspectionForm = null!,
                Vehicle = null!,
                User = null!,
                InspectionPassFailItems = []
            });
        _mockInspectionRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        // Act
        await _commandHandler.Handle(command, CancellationToken.None);

        // Assert
        Assert.NotNull(capturedInspection);
        Assert.Equal(3, capturedInspection.InspectionPassFailItems.Count);

        var item1 = capturedInspection.InspectionPassFailItems.First(i => i.InspectionFormItemID == 1);
        Assert.True(item1.Passed);
        Assert.Equal("Item 1 passed", item1.Comment);

        var item2 = capturedInspection.InspectionPassFailItems.First(i => i.InspectionFormItemID == 2);
        Assert.False(item2.Passed);
        Assert.Equal("Item 2 failed", item2.Comment);

        var item3 = capturedInspection.InspectionPassFailItems.First(i => i.InspectionFormItemID == 3);
        Assert.True(item3.Passed);
        Assert.Null(item3.Comment);
    }
}