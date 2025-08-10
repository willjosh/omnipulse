using Application.Exceptions;
using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceReminders.Command.GenerateServiceReminders;
using Application.Features.ServiceReminders.Query;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;
using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;
using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ServiceReminders;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceReminder))]
public class GenerateServiceRemindersIntegrationTests : BaseIntegrationTest
{
    public GenerateServiceRemindersIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_GenerateAndPersistReminders_For_TimeBasedSchedule()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 12000.0, year: 2021);
        var serviceProgramId = await CreateServiceProgramAsync();
        var serviceTaskId = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 50.00m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var serviceScheduleId = await CreateTimeBasedServiceScheduleAsync(
            serviceProgramId,
            [serviceTaskId],
            intervalValue: 90,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 7,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(10)
        );
        await AddVehicleToServiceProgramAsync(serviceProgramId, vehicleId);

        // Act - Generate reminders
        var genResult = await GenerateServiceRemindersAsync();

        // Assert - Command success
        genResult.Should().NotBeNull();
        genResult.Success.Should().BeTrue();
        genResult.GeneratedCount.Should().BeGreaterThan(0);

        // Act - Read reminders via query
        var reminders = await GetServiceRemindersAsync(vehicleId, serviceScheduleId);
        reminders.Should().NotBeEmpty();
        reminders.Should().Contain(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

        // Business rule: XOR (time-based only)
        var upcoming = reminders.First(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
        upcoming.DueDate.Should().NotBeNull();
        upcoming.DueMileage.Should().BeNull();
        upcoming.ScheduleType.Should().Be(ServiceScheduleTypeEnum.TIME);
    }

    [Fact]
    public async Task Should_Enforce_OneUpcoming_Per_Vehicle_And_ServiceSchedule()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 15000.0, year: 2020);
        var programId = await CreateServiceProgramAsync();
        var t1 = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 10m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var t2 = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 10m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var scheduleId = await CreateMileageBasedServiceScheduleAsync(
            programId,
            [t1, t2],
            mileageInterval: 10000,
            mileageBuffer: 1000,
            firstServiceMileage: 30000);
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Act - Generate
        var gen = await GenerateServiceRemindersAsync();
        gen.Success.Should().BeTrue();

        // Act - Read reminders
        var byPair = await GetServiceRemindersAsync(vehicleId, scheduleId);

        // Assert - Exactly one UPCOMING for (vehicle, schedule)
        byPair.Count(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Should().Be(1);
    }

    [Fact]
    public async Task Should_Generate_TimeBased_With_MixedStatuses_And_OneUpcoming()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 50000.0, year: 2019);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 3.0, estimatedCost: 200m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 180,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(-200)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Act
        var gen = await GenerateServiceRemindersAsync();

        // Assert
        gen.Success.Should().BeTrue();

        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);

        // With first service 200 days ago, interval 180, buffer 30:
        // Due dates: -200 (OVERDUE), -20 (OVERDUE), next +160 (UPCOMING), none DUE_SOON.
        reminders.Should().HaveCount(3);
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.OVERDUE).Should().Be(2);
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.DUE_SOON).Should().Be(0);
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Should().Be(1);
    }

    [Fact]
    public async Task Should_Generate_Exactly_One_Overdue_One_DueSoon_One_Upcoming_TimeBased()
    {
        // ===== Arrange =====
        // Should generate 3 reminder occurences:
        // Today = Day 0
        // -170 -> OVERDUE
        // +10  -> DUE_SOON
        // +190 -> UPCOMING
        var now = DateTime.UtcNow.Date;
        var firstServiceDate = now.AddDays(-170);
        const int interval = 180;
        const int buffer = 30;

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: interval,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: buffer,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: firstServiceDate
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act =====
        var gen = await GenerateServiceRemindersAsync();
        gen.Success.Should().BeTrue();

        // ===== Assert =====
        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);
        gen.GeneratedCount.Should().Be(reminders.Count);
        reminders.Should().HaveCount(3);
        reminders.Select(r => r.OccurrenceNumber).Should().OnlyHaveUniqueItems();

        // Pure time-based schedule invariants
        reminders.Should().OnlyContain(r => r.ScheduleType == ServiceScheduleTypeEnum.TIME
                                        && r.DueMileage == null
                                        && r.DueDate != null);

        // Sort by due date to reason about sequence
        var orderedReminders = reminders.OrderBy(r => r.DueDate).ToList();
        orderedReminders.Should().BeInAscendingOrder(r => r.DueDate);

        // Occurrence numbers should be 1,2,3
        orderedReminders.Select(r => r.OccurrenceNumber).Should().Equal(1, 2, 3);

        // Status counts
        reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.DUE_SOON);
        reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

        // Due Dates & Status
        var due1 = firstServiceDate;                     // -170  => OVERDUE
        var due2 = firstServiceDate.AddDays(interval * 1); // +10   => DUE_SOON
        var due3 = firstServiceDate.AddDays(interval * 2); // +190  => UPCOMING

        reminders.Should().ContainSingle(r => r.DueDate!.Value.Date == due1 && r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders.Should().ContainSingle(r => r.DueDate!.Value.Date == due2 && r.Status == ServiceReminderStatusEnum.DUE_SOON);
        reminders.Should().ContainSingle(r => r.DueDate!.Value.Date == due3 && r.Status == ServiceReminderStatusEnum.UPCOMING);

        // PK & FK sanity check
        reminders.Should().OnlyContain(r => r.VehicleID == vehicleId && r.ServiceScheduleID == scheduleId && r.ID > 0);
    }

    [Fact]
    public async Task Should_Generate_MileageBased_With_MixedStatuses_And_OneUpcoming()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 80000.0, year: 2018);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 2.0, estimatedCost: 150m, category: ServiceTaskCategoryEnum.INSPECTION);
        var scheduleId = await CreateMileageBasedServiceScheduleAsync(
            programId,
            [taskId],
            mileageInterval: 30000,
            mileageBuffer: 5000,
            firstServiceMileage: 50000
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Act
        var gen = await GenerateServiceRemindersAsync();

        // Assert
        gen.Success.Should().BeTrue();

        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);
        reminders.Should().HaveCountGreaterThan(1);
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.OVERDUE).Should().BeGreaterThan(0);
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Should().Be(1);
    }

    [Fact]
    public async Task Should_Generate_For_Multiple_Vehicles()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId1 = await CreateVehicleAsync(vehicleGroupId, mileage: 10000.0, year: 2022);
        var vehicleId2 = await CreateVehicleAsync(vehicleGroupId, mileage: 5000.0, year: 2023);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 50m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 365,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(180)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId1);
        await AddVehicleToServiceProgramAsync(programId, vehicleId2);

        // Act
        var gen = await GenerateServiceRemindersAsync();

        // Assert
        gen.Success.Should().BeTrue();

        var reminders1 = (await GetServiceRemindersAsync(vehicleId1, scheduleId)).Where(r => r.Status == ServiceReminderStatusEnum.UPCOMING).ToList();
        var reminders2 = (await GetServiceRemindersAsync(vehicleId2, scheduleId)).Where(r => r.Status == ServiceReminderStatusEnum.UPCOMING).ToList();

        reminders1.Should().HaveCount(1);
        reminders2.Should().HaveCount(1);
        reminders1[0].Status.Should().Be(ServiceReminderStatusEnum.UPCOMING);
        reminders2[0].Status.Should().Be(ServiceReminderStatusEnum.UPCOMING);
    }

    [Fact]
    public async Task Should_Handle_Existing_Reminders_Update()
    {
        // Arrange - Setup with existing reminders
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 25000.0, year: 2021);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 1.5, estimatedCost: 100m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 365,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(-400)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Initial generation
        await GenerateServiceRemindersAsync();

        // Act - Regenerate
        var gen = await GenerateServiceRemindersAsync();

        // Assert - Should update existing, not duplicate
        gen.Success.Should().BeTrue();

        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Should().Be(1);
        reminders.Count.Should().BeGreaterThan(1); // Overdue + upcoming
    }

    [Fact]
    public async Task Should_Cancel_Reminders_On_Schedule_SoftDelete()
    {
        // Arrange - Setup active schedule with reminders using helpers
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 15000.0, year: 2022);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 100m, category: ServiceTaskCategoryEnum.INSPECTION);
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 365,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 60,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(90)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Generate initial
        await GenerateServiceRemindersAsync();

        // Act - Soft delete schedule
        await Sender.Send(new DeleteServiceScheduleCommand(scheduleId));

        // Trigger generation which should not generate new since deleted
        await GenerateServiceRemindersAsync();

        // Assert
        var reminders = await GetRemindersInDbByScheduleAsync(scheduleId);
        reminders.Should().NotBeEmpty();
        reminders.Should().AllSatisfy(r => r.Status.Should().Be(ServiceReminderStatusEnum.CANCELLED));
        reminders.Should().AllSatisfy(r => r.CancelReason.Should().NotBeNullOrWhiteSpace());
    }

    [Fact]
    public async Task Should_Ignore_Schedules_With_No_Tasks()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 10000.0, year: 2022);
        var programId = await CreateServiceProgramAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [],
            intervalValue: 6,
            intervalUnit: TimeUnitEnum.Weeks,
            bufferValue: 1,
            bufferUnit: TimeUnitEnum.Weeks,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(30)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Act
        var gen = await GenerateServiceRemindersAsync();

        // Assert
        gen.Success.Should().BeTrue();

        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);
        reminders.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_Respect_TimeBuffer_With_Weeks_As_DueSoon_Boundary()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 22000.0, year: 2021);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 0.5, estimatedCost: 25m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 8,
            intervalUnit: TimeUnitEnum.Weeks,
            bufferValue: 2,
            bufferUnit: TimeUnitEnum.Weeks,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(10)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        var gen = await GenerateServiceRemindersAsync();
        gen.Success.Should().BeTrue();

        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);
        reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

        // Update schedule first date to within 14 days buffer (e.g., 7 days)
        await Sender.Send(new UpdateServiceScheduleCommand(
            ServiceScheduleID: scheduleId,
            ServiceProgramID: programId,
            Name: "WeeksBuffer Schedule Updated",
            ServiceTaskIDs: [taskId],
            TimeIntervalValue: 8,
            TimeIntervalUnit: TimeUnitEnum.Weeks,
            TimeBufferValue: 2,
            TimeBufferUnit: TimeUnitEnum.Weeks,
            MileageInterval: null,
            MileageBuffer: null,
            FirstServiceDate: DateTime.UtcNow.Date.AddDays(7),
            FirstServiceMileage: null,
            IsActive: true
        ));

        // Make intent explicit: regenerate after update
        await GenerateServiceRemindersAsync();
        var reminders2 = await GetServiceRemindersAsync(vehicleId, scheduleId);
        reminders2.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
        reminders2.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.DUE_SOON);
    }

    [Fact]
    public async Task Should_Respect_MileageBuffer_Boundary_As_DueSoon()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var currentMileage = 40000.0;
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: currentMileage, year: 2020);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 80m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var interval = 10000;
        var buffer = 1500;
        var scheduleId = await CreateMileageBasedServiceScheduleAsync(
            programId,
            [taskId],
            mileageInterval: interval,
            mileageBuffer: buffer,
            firstServiceMileage: 40000
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);
        var gen = await GenerateServiceRemindersAsync();
        gen.Success.Should().BeTrue();

        // boundary check: set odometer near due-soon boundary prior to re-generation to reflect status
        await DbContext.Vehicles.Where(v => v.ID == vehicleId)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Mileage, 48500.0));

        // Re-run generation, then query
        await GenerateServiceRemindersAsync();
        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);
        reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.DUE_SOON);
        reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
    }

    [Fact]
    public async Task Should_Use_CurrentMileage_When_FirstServiceMileage_NotProvided()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var currentMileage = 10000.0;
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: currentMileage, year: 2022);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync(estimatedHours: 0.5, estimatedCost: 30m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var interval = 5000;
        var buffer = 500;
        var scheduleId = await CreateMileageBasedServiceScheduleAsync(
            programId,
            [taskId],
            mileageInterval: interval,
            mileageBuffer: buffer,
            firstServiceMileage: null
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Act
        var gen = await GenerateServiceRemindersAsync();

        // Assert
        gen.Success.Should().BeTrue();

        var reminder = (await GetServiceRemindersAsync(vehicleId, scheduleId))
            .FirstOrDefault(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

        reminder.Should().NotBeNull();
        reminder!.DueMileage.Should().Be(currentMileage + interval);
        reminder.ScheduleType.Should().Be(ServiceScheduleTypeEnum.MILEAGE);
    }

    [Fact]
    public async Task Should_Regenerate_On_Schedule_Update()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 25000.0, year: 2021);
        var programId = await CreateServiceProgramAsync();
        var oldTaskId = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 50m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [oldTaskId],
            intervalValue: 180,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: DateTime.UtcNow.Date.AddDays(90)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);
        await GenerateServiceRemindersAsync();

        var newTaskId = await CreateServiceTaskAsync(estimatedHours: 2.0, estimatedCost: 100m, category: ServiceTaskCategoryEnum.PREVENTIVE);
        await Sender.Send(new UpdateServiceScheduleCommand(
            ServiceScheduleID: scheduleId,
            ServiceProgramID: programId,
            Name: "Updated Schedule",
            ServiceTaskIDs: [newTaskId],
            TimeIntervalValue: 365,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 60,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: null,
            MileageBuffer: null,
            FirstServiceDate: DateTime.UtcNow.Date.AddDays(120),
            FirstServiceMileage: null,
            IsActive: true
        ));

        // Make intent explicit: regenerate after update
        await GenerateServiceRemindersAsync();
        var reminders = await GetServiceRemindersAsync(vehicleId, scheduleId);
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.CANCELLED).Should().BeGreaterThan(0);
        var upcoming = reminders.FirstOrDefault(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
        upcoming.Should().NotBeNull();
        upcoming!.ServiceTasks.Should().HaveCount(1);
        // We avoid brittle name assertions; ensure one task exists
        upcoming.TimeIntervalValue.Should().Be(365);
        upcoming.DueDate.Should().BeCloseTo(DateTime.UtcNow.Date.AddDays(120), TimeSpan.FromDays(1));
    }

    [Fact]
    public async Task Should_Ignore_ScheduleCreation_When_Program_Inactive()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 15000.0, year: 2021);
        var programId = await CreateServiceProgramAsync(isActive: false); // Inactive Program
        var taskId = await CreateServiceTaskAsync(estimatedHours: 1.0, estimatedCost: 50m, category: ServiceTaskCategoryEnum.PREVENTIVE);

        // Act + Assert: creating a schedule under an inactive program should fail upfront
        await FluentActions.Invoking(async () => await Sender.Send(new CreateServiceScheduleCommand(
            ServiceProgramID: programId,
            Name: $"ProgInactive Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [taskId],
            TimeIntervalValue: 90,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 7,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: null,
            MileageBuffer: null,
            FirstServiceDate: DateTime.UtcNow.Date.AddDays(30),
            FirstServiceMileage: null,
            IsActive: true
        ))).Should().ThrowAsync<EntityNotFoundException>();
    }

    private async Task<int> CreateVehicleGroupAsync()
    {
        return await Sender.Send(new CreateVehicleGroupCommand(
            Name: $"Vehicle Group Name {Faker.Random.AlphaNumeric(5)}",
            Description: $"Vehicle Group Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        ));
    }

    private async Task<int> CreateVehicleAsync(
        int vehicleGroupId,
        double mileage = 10000.0,
        int year = 2021)
    {
        return await Sender.Send(new CreateVehicleCommand(
            Name: $"Vehicle Name {Faker.Random.AlphaNumeric(5)}",
            Make: "TestMake",
            Model: "TestModel",
            Year: year,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.BUS,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "Base",
            Mileage: mileage,
            EngineHours: 300.0,
            FuelCapacity: 50.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: 20000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Test Yard"
        ));
    }

    private async Task<int> CreateServiceProgramAsync(bool isActive = true)
    {
        return await Sender.Send(new CreateServiceProgramCommand(
            Name: $"Service Program Name {Faker.Random.AlphaNumeric(5)}",
            Description: $"Service Program Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: isActive
        ));
    }

    private async Task<int> CreateServiceTaskAsync(
        double estimatedHours = 1.0,
        decimal estimatedCost = 10m,
        ServiceTaskCategoryEnum category = ServiceTaskCategoryEnum.PREVENTIVE,
        bool isActive = true)
    {
        return await Sender.Send(new CreateServiceTaskCommand(
            Name: $"Service Task Name {Faker.Random.AlphaNumeric(5)}",
            Description: $"Service Task Description {Faker.Random.AlphaNumeric(5)}",
            EstimatedLabourHours: estimatedHours,
            EstimatedCost: estimatedCost,
            Category: category,
            IsActive: isActive
        ));
    }

    private async Task AddVehicleToServiceProgramAsync(int serviceProgramId, int vehicleId)
    {
        await Sender.Send(new AddVehicleToServiceProgramCommand(ServiceProgramID: serviceProgramId, VehicleID: vehicleId));
    }

    private async Task<int> CreateTimeBasedServiceScheduleAsync(
        int serviceProgramId,
        List<int> serviceTaskIds,
        int intervalValue,
        TimeUnitEnum intervalUnit,
        int bufferValue,
        TimeUnitEnum bufferUnit,
        DateTime? firstServiceDate,
        bool isActive = true,
        string? name = null)
    {
        return await Sender.Send(new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: name ?? $"Time Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: serviceTaskIds,
            TimeIntervalValue: intervalValue,
            TimeIntervalUnit: intervalUnit,
            TimeBufferValue: bufferValue,
            TimeBufferUnit: bufferUnit,
            MileageInterval: null,
            MileageBuffer: null,
            FirstServiceDate: firstServiceDate,
            FirstServiceMileage: null,
            IsActive: isActive
        ));
    }

    private async Task<int> CreateMileageBasedServiceScheduleAsync(
        int serviceProgramId,
        List<int> serviceTaskIds,
        int mileageInterval,
        int mileageBuffer,
        int? firstServiceMileage,
        bool isActive = true,
        string? name = null)
    {
        return await Sender.Send(new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: name ?? $"Mileage Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: serviceTaskIds,
            TimeIntervalValue: null,
            TimeIntervalUnit: null,
            TimeBufferValue: null,
            TimeBufferUnit: null,
            MileageInterval: mileageInterval,
            MileageBuffer: mileageBuffer,
            FirstServiceDate: null,
            FirstServiceMileage: firstServiceMileage,
            IsActive: isActive
        ));
    }

    private async Task<GenerateServiceRemindersResponse> GenerateServiceRemindersAsync()
    {
        return await Sender.Send(new GenerateServiceRemindersCommand());
    }

    private async Task<List<ServiceReminderDTO>> GetServiceRemindersAsync(int vehicleId, int scheduleId, int pageSize = 100)
    {
        var page = await Sender.Send(new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = pageSize }));
        return [.. page.Items.Where(r => r.VehicleID == vehicleId && r.ServiceScheduleID == scheduleId)];
    }

    private async Task<List<ServiceReminder>> GetRemindersInDbByScheduleAsync(int scheduleId)
    {
        return await DbContext.ServiceReminders
            .Where(r => r.ServiceScheduleID == scheduleId)
            .ToListAsync();
    }
}