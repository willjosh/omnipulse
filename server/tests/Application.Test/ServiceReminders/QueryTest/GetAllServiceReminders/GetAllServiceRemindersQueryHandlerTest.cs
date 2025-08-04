using Application.Contracts.Logger;
using Application.Contracts.Persistence;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentValidation;

using Moq;

namespace Application.Test.ServiceReminders.QueryTest.GetAllServiceReminders;

public class GetAllServiceRemindersQueryHandlerTest
{
    private readonly Mock<IServiceScheduleRepository> _mockServiceScheduleRepository;
    private readonly Mock<IVehicleRepository> _mockVehicleRepository;
    private readonly Mock<IXrefServiceProgramVehicleRepository> _mockXrefServiceProgramVehicleRepository;
    private readonly Mock<IXrefServiceScheduleServiceTaskRepository> _mockXrefServiceScheduleServiceTaskRepository;
    private readonly Mock<IServiceTaskRepository> _mockServiceTaskRepository;
    private readonly Mock<IValidator<GetAllServiceRemindersQuery>> _mockValidator;
    private readonly Mock<IAppLogger<GetAllServiceRemindersQueryHandler>> _mockLogger;
    private readonly GetAllServiceRemindersQueryHandler _handler;

    public GetAllServiceRemindersQueryHandlerTest()
    {
        _mockServiceScheduleRepository = new Mock<IServiceScheduleRepository>();
        _mockVehicleRepository = new Mock<IVehicleRepository>();
        _mockXrefServiceProgramVehicleRepository = new Mock<IXrefServiceProgramVehicleRepository>();
        _mockXrefServiceScheduleServiceTaskRepository = new Mock<IXrefServiceScheduleServiceTaskRepository>();
        _mockServiceTaskRepository = new Mock<IServiceTaskRepository>();
        _mockValidator = new Mock<IValidator<GetAllServiceRemindersQuery>>();
        _mockLogger = new Mock<IAppLogger<GetAllServiceRemindersQueryHandler>>();

        _handler = new GetAllServiceRemindersQueryHandler(
            _mockServiceScheduleRepository.Object,
            _mockVehicleRepository.Object,
            _mockXrefServiceProgramVehicleRepository.Object,
            _mockXrefServiceScheduleServiceTaskRepository.Object,
            _mockServiceTaskRepository.Object,
            _mockValidator.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task Handle_Should_Return_Multiple_Reminders_For_Overdue_Schedule()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });

        var serviceProgram = CreateServiceProgram();
        var serviceSchedule = CreateTimeBasedServiceSchedule(
            serviceProgram.ID,
            DateTime.UtcNow.AddDays(-10), // First service was 10 days ago
            TimeUnitEnum.Days,
            3); // Every 3 days

        var vehicle = CreateVehicle();
        var serviceTask = CreateServiceTask();
        var vehicleAssignment = new XrefServiceProgramVehicle
        {
            ServiceProgramID = serviceProgram.ID,
            VehicleID = vehicle.ID,
            AddedAt = DateTime.UtcNow.AddDays(-15), // Assigned 15 days ago
            ServiceProgram = serviceProgram,
            Vehicle = vehicle
        };

        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockServiceScheduleRepository.Setup(r => r.GetAllActiveWithNavigationAsync())
            .ReturnsAsync([serviceSchedule]);

        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetByServiceProgramIDAsync(serviceProgram.ID))
            .ReturnsAsync([vehicleAssignment]);

        _mockXrefServiceScheduleServiceTaskRepository.Setup(r => r.GetByServiceScheduleIdAsync(serviceSchedule.ID))
            .ReturnsAsync(
            [
                new() { ServiceScheduleID = serviceSchedule.ID, ServiceTaskID = serviceTask.ID, ServiceSchedule = serviceSchedule, ServiceTask = serviceTask }
            ]);

        _mockServiceTaskRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([serviceTask]);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicle.ID))
            .ReturnsAsync(vehicle);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items.Count > 1, "Should generate multiple reminder occurrences");

        // Should have at least one overdue reminder
        Assert.Contains(result.Items, r => r.Status == ServiceReminderStatusEnum.OVERDUE);

        // Verify different occurrence numbers
        var occurrenceNumbers = result.Items.Select(r => r.OccurrenceNumber).Distinct().ToList();
        Assert.True(occurrenceNumbers.Count > 1, "Should have different occurrence numbers");
    }

    [Fact]
    public async Task Handle_Should_Return_Mileage_Based_Reminders()
    {
        // Arrange
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 10 });

        var serviceProgram = CreateServiceProgram();
        var serviceSchedule = CreateMileageBasedServiceSchedule(
            serviceProgram.ID,
            15000, // First service at 15,000 km
            5000); // Every 5,000 km

        var vehicle = CreateVehicle(mileage: 22000); // Current mileage 22,000 km
        var serviceTask = CreateServiceTask();
        var vehicleAssignment = new XrefServiceProgramVehicle
        {
            ServiceProgramID = serviceProgram.ID,
            VehicleID = vehicle.ID,
            AddedAt = DateTime.UtcNow.AddDays(-30),
            ServiceProgram = serviceProgram,
            Vehicle = vehicle
        };

        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockServiceScheduleRepository.Setup(r => r.GetAllActiveWithNavigationAsync())
            .ReturnsAsync([serviceSchedule]);

        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetByServiceProgramIDAsync(serviceProgram.ID))
            .ReturnsAsync([vehicleAssignment]);

        _mockXrefServiceScheduleServiceTaskRepository.Setup(r => r.GetByServiceScheduleIdAsync(serviceSchedule.ID))
            .ReturnsAsync(
            [
                new() { ServiceScheduleID = serviceSchedule.ID, ServiceTaskID = serviceTask.ID, ServiceSchedule = serviceSchedule, ServiceTask = serviceTask }
            ]);

        _mockServiceTaskRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([serviceTask]);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicle.ID))
            .ReturnsAsync(vehicle);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.True(result.Items.Count > 0, "Should generate mileage-based reminders");

        // Should have at least one overdue mileage reminder (20,000 km service is overdue)
        Assert.Contains(result.Items, r => r.Status == ServiceReminderStatusEnum.OVERDUE && r.IsMileageBasedReminder);

        // Verify mileage calculation
        var overdueReminder = result.Items.First(r => r.Status == ServiceReminderStatusEnum.OVERDUE && r.IsMileageBasedReminder);
        Assert.True(overdueReminder.DueMileage < vehicle.Mileage, "Overdue reminder should have due mileage less than current mileage");
    }

    [Fact]
    public async Task Handle_Should_Apply_Search_Filter()
    {
        // Arrange
        var searchTerm = "TestVehicle";
        var query = new GetAllServiceRemindersQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = searchTerm
        });

        var serviceProgram = CreateServiceProgram();
        var serviceSchedule = CreateTimeBasedServiceSchedule(serviceProgram.ID, DateTime.UtcNow, TimeUnitEnum.Days, 7);
        var vehicle = CreateVehicle(name: searchTerm);
        var serviceTask = CreateServiceTask();
        var vehicleAssignment = new XrefServiceProgramVehicle
        {
            ServiceProgramID = serviceProgram.ID,
            VehicleID = vehicle.ID,
            AddedAt = DateTime.UtcNow.AddDays(-1),
            ServiceProgram = serviceProgram,
            Vehicle = vehicle
        };

        _mockValidator.Setup(v => v.ValidateAsync(query, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FluentValidation.Results.ValidationResult());

        _mockServiceScheduleRepository.Setup(r => r.GetAllActiveWithNavigationAsync())
            .ReturnsAsync([serviceSchedule]);

        _mockXrefServiceProgramVehicleRepository.Setup(r => r.GetByServiceProgramIDAsync(serviceProgram.ID))
            .ReturnsAsync([vehicleAssignment]);

        _mockXrefServiceScheduleServiceTaskRepository.Setup(r => r.GetByServiceScheduleIdAsync(serviceSchedule.ID))
            .ReturnsAsync(
            [
                new() { ServiceScheduleID = serviceSchedule.ID, ServiceTaskID = serviceTask.ID, ServiceSchedule = serviceSchedule, ServiceTask = serviceTask }
            ]);

        _mockServiceTaskRepository.Setup(r => r.GetByIdsAsync(It.IsAny<IEnumerable<int>>()))
            .ReturnsAsync([serviceTask]);

        _mockVehicleRepository.Setup(r => r.GetByIdAsync(vehicle.ID))
            .ReturnsAsync(vehicle);

        // Act
        var result = await _handler.Handle(query, CancellationToken.None);

        // Assert
        Assert.NotNull(result);
        Assert.All(result.Items, reminder =>
            Assert.Contains(searchTerm, reminder.VehicleName, StringComparison.OrdinalIgnoreCase));
    }

    private static ServiceProgram CreateServiceProgram()
    {
        return new ServiceProgram
        {
            ID = 1,
            Name = "Test Service Program",
            Description = "Test program",
            IsActive = true,
            ServiceSchedules = [],
            XrefServiceProgramVehicles = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static ServiceSchedule CreateTimeBasedServiceSchedule(int serviceProgramId, DateTime firstServiceDate, TimeUnitEnum intervalUnit, int intervalValue)
    {
        return new ServiceSchedule
        {
            ID = 1,
            ServiceProgramID = serviceProgramId,
            Name = "Test Schedule",
            TimeIntervalValue = intervalValue,
            TimeIntervalUnit = intervalUnit,
            TimeBufferValue = 1,
            TimeBufferUnit = TimeUnitEnum.Days,
            MileageInterval = null,
            MileageBuffer = null,
            FirstServiceDate = firstServiceDate,
            FirstServiceMileage = null,
            IsActive = true,
            XrefServiceScheduleServiceTasks = [],
            ServiceProgram = null!,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static ServiceSchedule CreateMileageBasedServiceSchedule(int serviceProgramId, double firstServiceMileage, int mileageInterval)
    {
        return new ServiceSchedule
        {
            ID = 1,
            ServiceProgramID = serviceProgramId,
            Name = "Test Mileage Schedule",
            TimeIntervalValue = null,
            TimeIntervalUnit = null,
            TimeBufferValue = null,
            TimeBufferUnit = null,
            MileageInterval = mileageInterval,
            MileageBuffer = 500,
            FirstServiceDate = null,
            FirstServiceMileage = (int)firstServiceMileage,
            IsActive = true,
            XrefServiceScheduleServiceTasks = [],
            ServiceProgram = null!,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static Vehicle CreateVehicle(string name = "Test Vehicle", double mileage = 15000)
    {
        return new Vehicle
        {
            ID = 1,
            Name = name,
            Make = "Test Make",
            Model = "Test Model",
            Year = 2020,
            VIN = "TEST123456789",
            LicensePlate = "TEST001",
            LicensePlateExpirationDate = DateTime.UtcNow.AddYears(1),
            VehicleType = VehicleTypeEnum.BUS,
            VehicleGroupID = 1,
            AssignedTechnicianID = null,
            Trim = "Standard",
            Mileage = mileage,
            EngineHours = 500,
            FuelCapacity = 100,
            FuelType = FuelTypeEnum.ELECTRIC,
            PurchaseDate = DateTime.UtcNow.AddYears(-1),
            PurchasePrice = 100000,
            Status = VehicleStatusEnum.ACTIVE,
            Location = "Test Location",
            User = null,
            VehicleGroup = null!,
            VehicleImages = [],
            VehicleAssignments = [],
            VehicleDocuments = [],
            XrefServiceProgramVehicles = [],
            ServiceReminders = [],
            Issues = [],
            Inspections = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static ServiceTask CreateServiceTask()
    {
        return new ServiceTask
        {
            ID = 1,
            Name = "Test Service Task",
            Description = "Test task description",
            EstimatedLabourHours = 2.0,
            EstimatedCost = 150.00m,
            Category = ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive = true,
            XrefServiceScheduleServiceTasks = [],
            MaintenanceHistories = [],
            WorkOrderLineItems = [],
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }
}