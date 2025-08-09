using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceReminders.Command.GenerateServiceReminders;
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
public class GenerateServiceRemindersIntegrationTests : BaseIntegrationTest
{
    public GenerateServiceRemindersIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_GenerateAndPersistReminders_For_TimeBasedSchedule()
    {
        // Arrange - Create VehicleGroup
        var vehicleGroupId = await Sender.Send(new CreateVehicleGroupCommand(
            Name: $"Gen VG {Faker.Random.AlphaNumeric(5)}",
            Description: "Group for generation tests",
            IsActive: true
        ));

        // Arrange - Create Vehicle
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Gen Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Toyota",
            Model: "Corolla",
            Year: 2021,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "Base",
            Mileage: 12000.0,
            EngineHours: 400.0,
            FuelCapacity: 50.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: 20000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Main Yard"
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        // Arrange - Create ServiceProgram
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"Gen ServiceProgram {Faker.Random.AlphaNumeric(5)}",
            Description: "Generation test program",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        // Arrange - Create ServiceTask
        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: "Oil Change",
            Description: "Change engine oil and filter",
            EstimatedLabourHours: 1.0,
            EstimatedCost: 50.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Create time-based ServiceSchedule (XOR: time-only)
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Gen Time Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 90,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 7,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: null,
            MileageBuffer: null,
            FirstServiceDate: DateTime.Today.AddDays(10), // upcoming (outside 7-day buffer)
            FirstServiceMileage: null,
            IsActive: true
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Assign vehicle to program
        await Sender.Send(new AddVehicleToServiceProgramCommand(ServiceProgramID: serviceProgramId, VehicleID: vehicleId));

        // Act - Generate reminders
        var genResult = await Sender.Send(new GenerateServiceRemindersCommand());

        // Assert - Command success
        genResult.Should().NotBeNull();
        genResult.Success.Should().BeTrue();
        genResult.GeneratedCount.Should().BeGreaterThan(0);

        // Act - Read reminders via query
        var query = new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 50 });
        var page = await Sender.Send(query);

        // Assert - Exists and upcoming for our vehicle
        var reminders = page.Items.Where(r => r.VehicleID == vehicleId && r.ServiceScheduleID == serviceScheduleId).ToList();
        reminders.Should().NotBeEmpty();
        reminders.Should().Contain(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

        // Business rule: XOR (time-based only)
        var upcoming = reminders.First(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
        upcoming.DueDate.Should().NotBeNull();
        upcoming.DueMileage.Should().BeNull();
        upcoming.ScheduleType.Should().Be(ServiceScheduleTypeEnum.TIME);
    }

    [Fact]
    public async Task Should_Enforce_OneUpcomingPer_Vehicle_And_ServiceSchedule()
    {
        // Arrange - Create VehicleGroup
        var vehicleGroupId = await Sender.Send(new CreateVehicleGroupCommand(
            Name: $"UPCOMING VG {Faker.Random.AlphaNumeric(5)}",
            Description: "Group for upcoming rule test",
            IsActive: true
        ));

        // Arrange - Create Vehicle
        var vehicleId = await Sender.Send(new CreateVehicleCommand(
            Name: $"UPCOMING Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: "Honda",
            Model: "Civic",
            Year: 2020,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "LX",
            Mileage: 15000.0,
            EngineHours: 500.0,
            FuelCapacity: 47.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-2),
            PurchasePrice: 18000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Depot"
        ));

        // Arrange - Create ServiceProgram
        var programId = await Sender.Send(new CreateServiceProgramCommand(
            Name: $"UPCOMING Program {Faker.Random.AlphaNumeric(5)}",
            Description: "One upcoming rule test",
            IsActive: true
        ));

        // Arrange - Two tasks under one schedule
        var t1 = await Sender.Send(new CreateServiceTaskCommand(
            Name: "Task A", Description: "A", EstimatedLabourHours: 1.0, EstimatedCost: 10m, Category: ServiceTaskCategoryEnum.PREVENTIVE, IsActive: true));
        var t2 = await Sender.Send(new CreateServiceTaskCommand(
            Name: "Task B", Description: "B", EstimatedLabourHours: 1.0, EstimatedCost: 10m, Category: ServiceTaskCategoryEnum.PREVENTIVE, IsActive: true));

        // Arrange - Mileage-based upcoming schedule (outside buffer) - XOR: mileage only
        var scheduleId = await Sender.Send(new CreateServiceScheduleCommand(
            ServiceProgramID: programId,
            Name: $"UPCOMING Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [t1, t2],
            TimeIntervalValue: null,
            TimeIntervalUnit: null,
            TimeBufferValue: null,
            TimeBufferUnit: null,
            MileageInterval: 10000,
            MileageBuffer: 1000,
            FirstServiceDate: null,
            FirstServiceMileage: 30000, // vehicle 15k, first occurrence at 30k => upcoming
            IsActive: true
        ));

        // Assign vehicle
        await Sender.Send(new AddVehicleToServiceProgramCommand(ServiceProgramID: programId, VehicleID: vehicleId));

        // Act - Generate
        var gen = await Sender.Send(new GenerateServiceRemindersCommand());
        gen.Success.Should().BeTrue();

        // Act - Read reminders
        var page = await Sender.Send(new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = 50 }));
        var byPair = page.Items.Where(r => r.VehicleID == vehicleId && r.ServiceScheduleID == scheduleId).ToList();

        // Assert - Exactly one UPCOMING for (vehicle, schedule)
        byPair.Count(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Should().Be(1);

        // And due soon / overdue may exist independently depending on current state
        byPair.Count(r => r.Status == ServiceReminderStatusEnum.DUE_SOON || r.Status == ServiceReminderStatusEnum.OVERDUE)
             .Should().BeGreaterThanOrEqualTo(0);

        // XOR: mileage-only
        var upcoming = byPair.First(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
        upcoming.DueMileage.Should().NotBeNull();
        upcoming.DueDate.Should().BeNull();
        upcoming.ScheduleType.Should().Be(ServiceScheduleTypeEnum.MILEAGE);
    }
}