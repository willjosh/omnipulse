using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Exceptions;
using Application.Features.MaintenanceHistories.Command.CreateMaintenanceHistory;

using AutoMapper;

using Domain.Entities;

using FluentValidation;

using Moq;

namespace Application.Test.MaintenanceHistories.CommandTest.CreateMaintenanceHistory;

public class CreateMaintenanceHistoryCommandHandlerTest
{
    private readonly Mock<IMaintenanceHistoryRepository> _mockMaintenanceHistoryRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IWorkOrderRepository> _mockWorkOrderRepository;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IUserRepository> _mockUserRepository;
    private readonly Mock<IAppLogger<CreateMaintenanceHistoryCommandHandler>> _mockLogger;
    private readonly Mock<IValidator<CreateMaintenanceHistoryCommand>> _mockValidator;
    private readonly IMapper _mapper;
    private readonly CreateMaintenanceHistoryCommandHandler _handler;

    public CreateMaintenanceHistoryCommandHandlerTest()
    {
        _mockMaintenanceHistoryRepository = new();
        _mockVehicleRepository = new();
        _mockWorkOrderRepository = new();
        _mockServiceTaskRepository = new();
        _mockUserRepository = new();
        _mockLogger = new();
        _mockValidator = new();

        var config = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile<Application.MappingProfiles.MaintenanceHistoryMappingProfile>();
        });
        _mapper = config.CreateMapper();

        _handler = new CreateMaintenanceHistoryCommandHandler(
            _mockMaintenanceHistoryRepository.Object,
            _mockVehicleRepository.Object,
            _mockWorkOrderRepository.Object,
            _mockServiceTaskRepository.Object,
            _mockUserRepository.Object,
            _mapper,
            _mockLogger.Object,
            _mockValidator.Object
        );
    }

    private CreateMaintenanceHistoryCommand CreateValidCommand(
        int vehicleId = 1,
        int workOrderId = 1,
        int serviceTaskId = 1,
        string technicianId = "tech-123",
        DateTime? serviceDate = null,
        double mileageAtService = 10000,
        string? description = "Routine maintenance",
        decimal cost = 250.50m,
        double labourHours = 2.5,
        string? notes = "Changed oil and filter"
    )
    {
        return new CreateMaintenanceHistoryCommand(
            VehicleID: vehicleId,
            WorkOrderID: workOrderId,
            ServiceTaskID: serviceTaskId,
            TechnicianID: technicianId,
            ServiceDate: serviceDate ?? DateTime.UtcNow.AddDays(-1),
            MileageAtService: mileageAtService,
            Description: description,
            Cost: cost,
            LabourHours: labourHours,
            Notes: notes
        );
    }

    private void SetupValidValidation(CreateMaintenanceHistoryCommand command)
    {
        var validResult = new FluentValidation.Results.ValidationResult();
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(validResult);
    }

    private void SetupInvalidValidation(CreateMaintenanceHistoryCommand command, string propertyName = "ServiceDate", string errorMessage = "Validation failed")
    {
        var invalidResult = new FluentValidation.Results.ValidationResult(
            [new FluentValidation.Results.ValidationFailure(propertyName, errorMessage)]
        );
        _mockValidator.Setup(v => v.ValidateAsync(command, CancellationToken.None))
            .ReturnsAsync(invalidResult);
    }

    [Fact]
    public async Task Handler_Should_Return_MaintenanceHistoryID_On_Success()
    {
        var command = CreateValidCommand();
        SetupValidValidation(command);

        var expectedEntity = new MaintenanceHistory
        {
            ID = 42,
            VehicleID = command.VehicleID,
            WorkOrderID = command.WorkOrderID,
            ServiceTaskID = command.ServiceTaskID,
            TechnicianID = command.TechnicianID,
            ServiceDate = command.ServiceDate,
            MileageAtService = command.MileageAtService,
            Description = command.Description,
            Cost = command.Cost,
            LabourHours = command.LabourHours,
            Notes = command.Notes,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Vehicle = null!,
            WorkOrder = null!,
            ServiceTask = null!,
            User = null!,
            InventoryTransactions = []
        };

        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockWorkOrderRepository.Setup(r => r.ExistsAsync(command.WorkOrderID)).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(r => r.ExistsAsync(command.ServiceTaskID)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.TechnicianID)).ReturnsAsync(true);
        _mockMaintenanceHistoryRepository.Setup(r => r.AddAsync(It.IsAny<MaintenanceHistory>())).ReturnsAsync(expectedEntity);
        _mockMaintenanceHistoryRepository.Setup(r => r.SaveChangesAsync()).ReturnsAsync(1);

        var result = await _handler.Handle(command, CancellationToken.None);

        Assert.Equal(expectedEntity.ID, result);
        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Handler_Should_Throw_BadRequestException_On_Validation_Failure()
    {
        var command = CreateValidCommand();
        SetupInvalidValidation(command, "ServiceDate", "Service date is invalid");

        await Assert.ThrowsAsync<BadRequestException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
        _mockMaintenanceHistoryRepository.Verify(r => r.SaveChangesAsync(), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_Nonexistent_VehicleID()
    {
        var command = CreateValidCommand(vehicleId: 999);
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.ExistsAsync(command.VehicleID), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_Nonexistent_WorkOrderID()
    {
        var command = CreateValidCommand(workOrderId: 888);
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockWorkOrderRepository.Setup(r => r.ExistsAsync(command.WorkOrderID)).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.ExistsAsync(command.VehicleID), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.ExistsAsync(command.WorkOrderID), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_Nonexistent_ServiceTaskID()
    {
        var command = CreateValidCommand(serviceTaskId: 777);
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockWorkOrderRepository.Setup(r => r.ExistsAsync(command.WorkOrderID)).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(r => r.ExistsAsync(command.ServiceTaskID)).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.ExistsAsync(command.VehicleID), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.ExistsAsync(command.WorkOrderID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.ExistsAsync(command.ServiceTaskID), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
    }

    [Fact]
    public async Task Handler_Should_Throw_EntityNotFoundException_On_Nonexistent_TechnicianID()
    {
        var command = CreateValidCommand(technicianId: "nonexistent-tech");
        SetupValidValidation(command);

        _mockVehicleRepository.Setup(r => r.ExistsAsync(command.VehicleID)).ReturnsAsync(true);
        _mockWorkOrderRepository.Setup(r => r.ExistsAsync(command.WorkOrderID)).ReturnsAsync(true);
        _mockServiceTaskRepository.Setup(r => r.ExistsAsync(command.ServiceTaskID)).ReturnsAsync(true);
        _mockUserRepository.Setup(r => r.ExistsAsync(command.TechnicianID)).ReturnsAsync(false);

        await Assert.ThrowsAsync<EntityNotFoundException>(
            () => _handler.Handle(command, CancellationToken.None)
        );

        _mockValidator.Verify(v => v.ValidateAsync(command, CancellationToken.None), Times.Once);
        _mockVehicleRepository.Verify(r => r.ExistsAsync(command.VehicleID), Times.Once);
        _mockWorkOrderRepository.Verify(r => r.ExistsAsync(command.WorkOrderID), Times.Once);
        _mockServiceTaskRepository.Verify(r => r.ExistsAsync(command.ServiceTaskID), Times.Once);
        _mockUserRepository.Verify(r => r.ExistsAsync(command.TechnicianID), Times.Once);
        _mockMaintenanceHistoryRepository.Verify(r => r.AddAsync(It.IsAny<MaintenanceHistory>()), Times.Never);
    }
}