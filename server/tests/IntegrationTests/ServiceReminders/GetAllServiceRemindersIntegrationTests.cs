using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.ServiceReminders;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceReminder))]
public class GetAllServiceRemindersIntegrationTests : BaseIntegrationTest
{
    public GetAllServiceRemindersIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnAllServiceReminders_When_DataExists()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Test VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Toyota",
            Model: "Camry",
            Year: 2020,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "LE",
            Mileage: 15000.0,
            EngineHours: 500.0,
            FuelCapacity: 60.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-2),
            PurchasePrice: 25000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Main Garage"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Test ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test ServiceProgram Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create ServiceTasks
        var createServiceTask1Command = new CreateServiceTaskCommand(
            Name: "Oil Change",
            Description: "Change engine oil and filter",
            EstimatedLabourHours: 1.0,
            EstimatedCost: 50.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTask1Id = await Sender.Send(createServiceTask1Command);

        var createServiceTask2Command = new CreateServiceTaskCommand(
            Name: "Tire Rotation",
            Description: "Rotate tires for even wear",
            EstimatedLabourHours: 0.5,
            EstimatedCost: 25.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTask2Id = await Sender.Send(createServiceTask2Command);

        // Arrange - Create ServiceSchedule (due soon)
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Test ServiceSchedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTask1Id, serviceTask2Id],
            TimeIntervalValue: 30,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 5,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 5000,
            MileageBuffer: 500,
            FirstServiceDate: DateTime.Today.AddDays(3), // Due soon (within buffer)
            FirstServiceMileage: 20000, // Vehicle at 15k, due at 20k
            IsActive: true
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Add Vehicle to ServiceProgram
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();

        var reminder = result.Items.First();
        reminder.Should().NotBeNull();

        // Find our specific reminder by filtering for our created vehicle
        var specificReminder = result.Items.FirstOrDefault(r => r.VehicleID == vehicleId);
        specificReminder.Should().NotBeNull();

        // Verify basic properties
        specificReminder!.VehicleID.Should().Be(vehicleId);
        specificReminder.ServiceProgramID.Should().Be(serviceProgramId);
        specificReminder.ServiceScheduleID.Should().Be(serviceScheduleId);
        specificReminder.ServiceScheduleName.Should().Contain("Test ServiceSchedule");

        // Verify calculated properties
        specificReminder.Status.Should().Be(ServiceReminderStatusEnum.DUE_SOON); // Within buffer threshold
        specificReminder.ServiceTasks.Should().HaveCount(2);
        specificReminder.TaskCount.Should().Be(2);
        specificReminder.TotalEstimatedCost.Should().Be(75.00m); // 50 + 25
        specificReminder.TotalEstimatedLabourHours.Should().Be(1.5); // 1.0 + 0.5

        // Verify tasks are populated correctly
        var oilChangeTask = specificReminder.ServiceTasks.FirstOrDefault(t => t.ServiceTaskName.Contains("Oil Change"));
        oilChangeTask.Should().NotBeNull();
        oilChangeTask!.EstimatedCost.Should().Be(50.00m);
        oilChangeTask.EstimatedLabourHours.Should().Be(1.0);
        oilChangeTask.ServiceTaskCategory.Should().Be(ServiceTaskCategoryEnum.PREVENTIVE);

        var tireRotationTask = specificReminder.ServiceTasks.FirstOrDefault(t => t.ServiceTaskName.Contains("Tire Rotation"));
        tireRotationTask.Should().NotBeNull();
        tireRotationTask!.EstimatedCost.Should().Be(25.00m);
        tireRotationTask.EstimatedLabourHours.Should().Be(0.5);

        // Verify due dates are calculated
        specificReminder.DueDate.Should().NotBeNull();
        specificReminder.DueDate.Should().BeCloseTo(DateTime.Today.AddDays(3), TimeSpan.FromHours(1));
        // This is time-based only since vehicle mileage (15k) is below first service mileage (20k)
        specificReminder.DueMileage.Should().BeNull();
        specificReminder.IsTimeBasedReminder.Should().BeTrue();
        specificReminder.IsMileageBasedReminder.Should().BeFalse();

        // Verify pagination
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalCount.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_ReturnOnlyRelevantReminders_When_VehicleNotAssignedToThisServiceProgram()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Isolated VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Isolated VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle (but don't assign to any service program)
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Isolated Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Ford",
            Model: "Focus",
            Year: 2021,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "SE",
            Mileage: 10000.0,
            EngineHours: 300.0,
            FuelCapacity: 55.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: 20000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Parking Lot"
        );
        int isolatedVehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram (but don't assign the vehicle to it)
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Isolated ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"Isolated ServiceProgram Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        // Should not contain reminders for the isolated vehicle since it's not assigned to any service program
        result.Items.Should().NotContain(r => r.VehicleID == isolatedVehicleId);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
    }

    [Fact]
    public async Task Should_ReturnOverdueReminder_When_ServiceIsOverdue()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Test VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle with high mileage
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Honda",
            Model: "Civic",
            Year: 2019,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "EX",
            Mileage: 25000.0, // High mileage to trigger overdue
            EngineHours: 800.0,
            FuelCapacity: 50.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-3),
            PurchasePrice: 22000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Service Bay 2"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Test ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test ServiceProgram Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create ServiceTask
        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: "Brake Inspection",
            Description: "Inspect brake pads and rotors",
            EstimatedLabourHours: 2.0,
            EstimatedCost: 100.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Create ServiceSchedule that's overdue
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Overdue ServiceSchedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 90,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 7,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 10000,
            MileageBuffer: 1000,
            FirstServiceDate: DateTime.Today.AddDays(-10), // Overdue by 10 days
            FirstServiceMileage: 20000, // Vehicle at 25k, was due at 20k
            IsActive: true
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Add Vehicle to ServiceProgram
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();

        // Find our specific overdue reminder
        var overdueReminder = result.Items.FirstOrDefault(r => r.VehicleID == vehicleId);
        overdueReminder.Should().NotBeNull();

        // Verify overdue status
        overdueReminder!.Status.Should().Be(ServiceReminderStatusEnum.OVERDUE);
        overdueReminder.PriorityLevel.Should().Be(PriorityLevelEnum.HIGH);

        // Verify basic properties
        overdueReminder.VehicleID.Should().Be(vehicleId);
        overdueReminder.ServiceScheduleName.Should().Contain("Overdue ServiceSchedule");
        overdueReminder.ServiceTasks.Should().HaveCount(1);
        overdueReminder.ServiceTasks.First().ServiceTaskName.Should().Contain("Brake Inspection");
    }

    [Fact]
    public async Task Should_ReturnUpcomingReminder_When_ServiceIsNotYetDueSoon()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Test VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Nissan",
            Model: "Altima",
            Year: 2022,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "SV",
            Mileage: 5000.0, // Low mileage
            EngineHours: 200.0,
            FuelCapacity: 65.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddMonths(-6),
            PurchasePrice: 28000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Fleet Parking"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Test ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test ServiceProgram Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create ServiceTask
        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"Air Filter Replacement {Faker.Random.AlphaNumeric(5)}",
            Description: "Replace engine air filter",
            EstimatedLabourHours: 0.5,
            EstimatedCost: 30.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Create ServiceSchedule that's upcoming (not due soon yet)
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Upcoming ServiceSchedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 180,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 14,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 15000,
            MileageBuffer: 1500,
            FirstServiceDate: DateTime.Today.AddDays(30), // Due in 30 days (outside buffer)
            FirstServiceMileage: 20000, // Vehicle at 5k, due at 20k
            IsActive: true
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Add Vehicle to ServiceProgram
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();

        // Find our specific upcoming reminder
        var upcomingReminder = result.Items.FirstOrDefault(r => r.VehicleID == vehicleId);
        upcomingReminder.Should().NotBeNull();

        // Verify upcoming status
        upcomingReminder!.Status.Should().Be(ServiceReminderStatusEnum.UPCOMING);
        upcomingReminder.PriorityLevel.Should().Be(PriorityLevelEnum.LOW);

        // Verify properties
        upcomingReminder.VehicleID.Should().Be(vehicleId);
        upcomingReminder.ServiceScheduleName.Should().Contain("Upcoming ServiceSchedule");
        upcomingReminder.ServiceTasks.Should().HaveCount(1);
        upcomingReminder.ServiceTasks.First().ServiceTaskName.Should().Contain("Air Filter Replacement");

        // Verify due date is in the future but not within buffer
        upcomingReminder.DueDate.Should().BeCloseTo(DateTime.Today.AddDays(30), TimeSpan.FromHours(1));
        upcomingReminder.DaysUntilDue.Should().BeGreaterThan(14); // Outside buffer

        // Verify it's not mileage-based yet (vehicle at 5k, due at 20k)
        upcomingReminder.DueMileage.Should().BeNull();
        upcomingReminder.IsTimeBasedReminder.Should().BeTrue();
        upcomingReminder.IsMileageBasedReminder.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnDueSoonReminder_When_ServiceIsWithinBuffer()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Test VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Mazda",
            Model: "CX-5",
            Year: 2021,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.VAN,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "Touring",
            Mileage: 18500.0, // Close to due mileage
            EngineHours: 600.0,
            FuelCapacity: 58.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: 32000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "North Depot"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Test ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test ServiceProgram Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create ServiceTask
        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"Transmission Service {Faker.Random.AlphaNumeric(5)}",
            Description: "Change transmission fluid",
            EstimatedLabourHours: 1.5,
            EstimatedCost: 150.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Create ServiceSchedule that's due soon
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Due Soon ServiceSchedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 365,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 7,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 30000,
            MileageBuffer: 2000,
            FirstServiceDate: DateTime.Today.AddDays(5), // Due in 5 days (within 7-day buffer)
            FirstServiceMileage: 20000, // Vehicle at 18.5k, due at 20k (within 2k buffer)
            IsActive: true
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Add Vehicle to ServiceProgram
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();

        // Find our specific due soon reminder
        var dueSoonReminder = result.Items.FirstOrDefault(r => r.VehicleID == vehicleId);
        dueSoonReminder.Should().NotBeNull();

        // Verify due soon status
        dueSoonReminder!.Status.Should().Be(ServiceReminderStatusEnum.DUE_SOON);
        dueSoonReminder.PriorityLevel.Should().Be(PriorityLevelEnum.MEDIUM);

        // Verify properties
        dueSoonReminder.VehicleID.Should().Be(vehicleId);
        dueSoonReminder.ServiceScheduleName.Should().Contain("Due Soon ServiceSchedule");
        dueSoonReminder.ServiceTasks.Should().HaveCount(1);
        dueSoonReminder.ServiceTasks.First().ServiceTaskName.Should().Contain("Transmission Service");

        // Verify due date is within buffer
        dueSoonReminder.DueDate.Should().BeCloseTo(DateTime.Today.AddDays(5), TimeSpan.FromHours(1));
        dueSoonReminder.DaysUntilDue.Should().BeInRange(0, 7); // Within buffer

        // Verify mileage is also within buffer
        dueSoonReminder.DueMileage.Should().Be(20000);
        dueSoonReminder.MileageVariance.Should().Be(-1500); // 18.5k - 20k = -1.5k (within 2k buffer)
    }

    [Fact]
    public async Task Should_ReturnOverdueReminder_When_BothTimeAndMileageAreOverdue()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Test VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle with high mileage
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Chevrolet",
            Model: "Silverado",
            Year: 2018,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.TRUCK,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "LT",
            Mileage: 85000.0, // Very high mileage
            EngineHours: 2500.0,
            FuelCapacity: 100.0,
            FuelType: FuelTypeEnum.DIESEL,
            PurchaseDate: DateTime.Today.AddYears(-5),
            PurchasePrice: 45000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Heavy Equipment Yard"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Test ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test ServiceProgram Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create Multiple ServiceTasks
        var createServiceTask1Command = new CreateServiceTaskCommand(
            Name: $"Engine Overhaul {Faker.Random.AlphaNumeric(5)}",
            Description: "Major engine service",
            EstimatedLabourHours: 8.0,
            EstimatedCost: 2000.00m,
            Category: ServiceTaskCategoryEnum.CORRECTIVE,
            IsActive: true
        );
        int serviceTask1Id = await Sender.Send(createServiceTask1Command);

        var createServiceTask2Command = new CreateServiceTaskCommand(
            Name: $"Transmission Rebuild {Faker.Random.AlphaNumeric(5)}",
            Description: "Rebuild transmission",
            EstimatedLabourHours: 6.0,
            EstimatedCost: 1500.00m,
            Category: ServiceTaskCategoryEnum.CORRECTIVE,
            IsActive: true
        );
        int serviceTask2Id = await Sender.Send(createServiceTask2Command);

        // Arrange - Create ServiceSchedule that's severely overdue
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Severely Overdue ServiceSchedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTask1Id, serviceTask2Id],
            TimeIntervalValue: 365,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 30,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 50000,
            MileageBuffer: 5000,
            FirstServiceDate: DateTime.Today.AddDays(-60), // Overdue by 60 days
            FirstServiceMileage: 60000, // Vehicle at 85k, was due at 60k
            IsActive: true
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Add Vehicle to ServiceProgram
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();

        // Find our specific overdue reminder
        var overdueReminder = result.Items.FirstOrDefault(r => r.VehicleID == vehicleId);
        overdueReminder.Should().NotBeNull();

        // Verify overdue status
        overdueReminder!.Status.Should().Be(ServiceReminderStatusEnum.OVERDUE);
        overdueReminder.PriorityLevel.Should().Be(PriorityLevelEnum.HIGH);

        // Verify properties
        overdueReminder.VehicleID.Should().Be(vehicleId);
        overdueReminder.ServiceScheduleName.Should().Contain("Severely Overdue ServiceSchedule");

        // Verify multiple tasks
        overdueReminder.ServiceTasks.Should().HaveCount(2);
        overdueReminder.TotalEstimatedCost.Should().Be(3500.00m); // 2000 + 1500
        overdueReminder.TotalEstimatedLabourHours.Should().Be(14.0); // 8.0 + 6.0

        // Verify time is overdue
        overdueReminder.DueDate.Should().BeCloseTo(DateTime.Today.AddDays(-60), TimeSpan.FromHours(1));
        overdueReminder.DaysUntilDue.Should().BeLessThan(-30); // Well past buffer

        // Verify if mileage reminder is generated (vehicle at 85k, was due at 60k)
        if (overdueReminder.DueMileage.HasValue)
        {
            overdueReminder.DueMileage.Should().Be(60000);
            overdueReminder.MileageVariance.Should().Be(25000); // 85k - 60k = 25k overdue
            overdueReminder.IsMileageBasedReminder.Should().BeTrue();
        }

        // At least time based
        overdueReminder.IsTimeBasedReminder.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnMultipleReminders_When_VehicleHasMultipleSchedules()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Test VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Multi-Schedule Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Ford",
            Model: "F-150",
            Year: 2020,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.TRUCK,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "XLT",
            Mileage: 45000.0,
            EngineHours: 1200.0,
            FuelCapacity: 90.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-2),
            PurchasePrice: 40000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Main Fleet"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Comprehensive ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"ServiceProgram with multiple schedules",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create Multiple ServiceTasks
        var oilChangeTask = new CreateServiceTaskCommand(
            Name: $"Oil Change {Faker.Random.AlphaNumeric(5)}",
            Description: "Change engine oil",
            EstimatedLabourHours: 0.5,
            EstimatedCost: 50.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int oilChangeTaskId = await Sender.Send(oilChangeTask);

        var coolantFlushTask = new CreateServiceTaskCommand(
            Name: $"Coolant Flush {Faker.Random.AlphaNumeric(5)}",
            Description: "Flush cooling system",
            EstimatedLabourHours: 1.0,
            EstimatedCost: 100.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int coolantFlushTaskId = await Sender.Send(coolantFlushTask);

        var brakeServiceTask = new CreateServiceTaskCommand(
            Name: $"Brake Service {Faker.Random.AlphaNumeric(5)}",
            Description: "Service brake system",
            EstimatedLabourHours: 2.0,
            EstimatedCost: 300.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int brakeServiceTaskId = await Sender.Send(brakeServiceTask);

        // Arrange - Create Multiple ServiceSchedules with different statuses
        // Schedule 1: Overdue
        var overdueSchedule = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Oil Change Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [oilChangeTaskId],
            TimeIntervalValue: 90,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 7,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 5000,
            MileageBuffer: 500,
            FirstServiceDate: DateTime.Today.AddDays(-15), // Overdue
            FirstServiceMileage: 40000, // Vehicle at 45k, was due at 40k
            IsActive: true
        );
        int overdueScheduleId = await Sender.Send(overdueSchedule);

        // Schedule 2: Due Soon
        var dueSoonSchedule = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Coolant Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [coolantFlushTaskId],
            TimeIntervalValue: 365,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 14,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 30000,
            MileageBuffer: 3000,
            FirstServiceDate: DateTime.Today.AddDays(10), // Due soon
            FirstServiceMileage: 48000, // Vehicle at 45k, due at 48k (within buffer)
            IsActive: true
        );
        int dueSoonScheduleId = await Sender.Send(dueSoonSchedule);

        // Schedule 3: Upcoming
        var upcomingSchedule = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Brake Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [brakeServiceTaskId],
            TimeIntervalValue: 730,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 30,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 60000,
            MileageBuffer: 5000,
            FirstServiceDate: DateTime.Today.AddDays(90), // Far future
            FirstServiceMileage: 80000, // Vehicle at 45k, due at 80k
            IsActive: true
        );
        int upcomingScheduleId = await Sender.Send(upcomingSchedule);

        // Arrange - Add Vehicle to ServiceProgram
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().NotBeEmpty();

        // Find all reminders for our vehicle
        var vehicleReminders = result.Items.Where(r => r.VehicleID == vehicleId).ToList();
        vehicleReminders.Should().HaveCount(3); // One for each schedule

        // Verify we have one of each status
        var overdueReminder = vehicleReminders.FirstOrDefault(r => r.Status == ServiceReminderStatusEnum.OVERDUE);
        var dueSoonReminder = vehicleReminders.FirstOrDefault(r => r.Status == ServiceReminderStatusEnum.DUE_SOON);
        var upcomingReminder = vehicleReminders.FirstOrDefault(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

        overdueReminder.Should().NotBeNull();
        dueSoonReminder.Should().NotBeNull();
        upcomingReminder.Should().NotBeNull();

        // Verify each reminder has correct task type
        overdueReminder!.ServiceTasks.First().ServiceTaskName.Should().Contain("Oil Change");
        dueSoonReminder!.ServiceTasks.First().ServiceTaskName.Should().Contain("Coolant Flush");
        upcomingReminder!.ServiceTasks.First().ServiceTaskName.Should().Contain("Brake Service");

        // Verify priority levels
        overdueReminder.PriorityLevel.Should().Be(PriorityLevelEnum.HIGH);
        dueSoonReminder.PriorityLevel.Should().Be(PriorityLevelEnum.MEDIUM);
        upcomingReminder.PriorityLevel.Should().Be(PriorityLevelEnum.LOW);
    }

    [Fact]
    public async Task Should_HandlePagination_When_ManyRemindersExist()
    {
        // This test creates multiple vehicles and schedules to test pagination

        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Fleet VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Large fleet for pagination test",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Fleet ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"ServiceProgram for fleet",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create ServiceTask
        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"Fleet Maintenance {Faker.Random.AlphaNumeric(5)}",
            Description: "Standard fleet maintenance",
            EstimatedLabourHours: 2.0,
            EstimatedCost: 200.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Create multiple vehicles and schedules
        var vehicleIds = new List<int>();
        for (int i = 0; i < 15; i++) // Create 15 vehicles
        {
            var createVehicleCommand = new CreateVehicleCommand(
                Name: $"Fleet Vehicle {i + 1}",
                Make: "Toyota",
                Model: "Corolla",
                Year: 2020 + (i % 3),
                VIN: Faker.Vehicle.Vin(),
                LicensePlate: Faker.Random.Replace("???-###"),
                LicensePlateExpirationDate: DateTime.Today.AddYears(1),
                VehicleType: VehicleTypeEnum.CAR,
                VehicleGroupID: vehicleGroupId,
                AssignedTechnicianID: null,
                Trim: "Base",
                Mileage: 10000.0 + (i * 1000),
                EngineHours: 300.0 + (i * 50),
                FuelCapacity: 50.0,
                FuelType: FuelTypeEnum.PETROL,
                PurchaseDate: DateTime.Today.AddMonths(-12 - i),
                PurchasePrice: 20000.00m,
                Status: VehicleStatusEnum.ACTIVE,
                Location: $"Bay {i + 1}"
            );
            int vehicleId = await Sender.Send(createVehicleCommand);
            vehicleIds.Add(vehicleId);

            // Add to service program
            var addVehicleCommand = new AddVehicleToServiceProgramCommand(
                ServiceProgramID: serviceProgramId,
                VehicleID: vehicleId
            );
            await Sender.Send(addVehicleCommand);
        }

        // Create a schedule that applies to all vehicles
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Fleet Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 90,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 7,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 10000,
            MileageBuffer: 1000,
            FirstServiceDate: DateTime.Today.AddDays(5), // Due soon for all
            FirstServiceMileage: 15000,
            IsActive: true
        );
        await Sender.Send(createServiceScheduleCommand);

        // Test Page 1
        var page1Query = new GetAllServiceRemindersQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5
        });
        var page1Result = await Sender.Send(page1Query);

        page1Result.Should().NotBeNull();
        page1Result.Items.Should().HaveCount(5);
        page1Result.PageNumber.Should().Be(1);
        page1Result.PageSize.Should().Be(5);
        page1Result.TotalCount.Should().BeGreaterThanOrEqualTo(15);
        page1Result.TotalPages.Should().BeGreaterThanOrEqualTo(3);
        page1Result.HasPreviousPage.Should().BeFalse();
        page1Result.HasNextPage.Should().BeTrue();

        // Test Page 2
        var page2Query = new GetAllServiceRemindersQuery(new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 5
        });
        var page2Result = await Sender.Send(page2Query);

        page2Result.Should().NotBeNull();
        page2Result.Items.Should().HaveCount(5);
        page2Result.PageNumber.Should().Be(2);
        page2Result.HasPreviousPage.Should().BeTrue();
        page2Result.HasNextPage.Should().BeTrue();

        // Ensure different items on different pages
        var page1Ids = page1Result.Items.Select(r => r.VehicleID).ToList();
        var page2Ids = page2Result.Items.Select(r => r.VehicleID).ToList();
        page1Ids.Should().NotIntersectWith(page2Ids);
    }

    [Fact]
    public async Task Should_FilterInactiveSchedules_When_ScheduleIsInactive()
    {
        // Arrange - Create VehicleGroup
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Test VehicleGroup {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test VehicleGroup Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        // Arrange - Create Vehicle
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Subaru",
            Model: "Outback",
            Year: 2022,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.VAN,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "Premium",
            Mileage: 8000.0,
            EngineHours: 250.0,
            FuelCapacity: 63.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddMonths(-8),
            PurchasePrice: 35000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "East Location"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Test ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test ServiceProgram Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create ServiceTask
        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"Inactive Task {Faker.Random.AlphaNumeric(5)}",
            Description: "This schedule should not generate reminders",
            EstimatedLabourHours: 1.0,
            EstimatedCost: 100.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Create INACTIVE ServiceSchedule
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Inactive ServiceSchedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 30,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 5,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 5000,
            MileageBuffer: 500,
            FirstServiceDate: DateTime.Today.AddDays(-5), // Would be overdue
            FirstServiceMileage: 5000, // Would be overdue
            IsActive: false // INACTIVE
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Add Vehicle to ServiceProgram
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Create query
        var paginationParameters = new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        };
        var query = new GetAllServiceRemindersQuery(paginationParameters);

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        // Should not contain reminders for the inactive schedule
        result.Items.Should().NotContain(r => r.VehicleID == vehicleId && r.ServiceScheduleID == serviceScheduleId);
    }
}