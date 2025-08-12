using Application.Exceptions;
using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceReminders.Command.SyncServiceReminders;
using Application.Features.ServiceReminders.Query;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;
using Application.Features.ServiceSchedules.Command.UpdateServiceSchedule;
using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Features.Vehicles.Command.UpdateVehicle;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ServiceReminders;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceReminder))]
public class SyncServiceRemindersIntegrationTests : BaseIntegrationTest
{
    public SyncServiceRemindersIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_GenerateAndPersistReminders_For_TimeBasedSchedule()
    {
        // ===== Arrange =====
        var todayUtc = GetUtcToday();

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 12000.0);
        var serviceProgramId = await CreateServiceProgramAsync();
        var serviceTaskId = await CreateServiceTaskAsync();
        var serviceScheduleId = await CreateTimeBasedServiceScheduleAsync(
            serviceProgramId,
            [serviceTaskId],
            intervalValue: 90,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 7,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: todayUtc.AddDays(10)
        );
        await AddVehicleToServiceProgramAsync(serviceProgramId, vehicleId);

        // ===== Act =====
        var syncResult = await SyncServiceRemindersAsync();

        // ===== Assert =====
        syncResult.Should().NotBeNull();
        syncResult.Success.Should().BeTrue();

        // Act - Read reminders via query
        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, serviceScheduleId);

        // Expectations: future first service date → only one UPCOMING; no OVERDUE/DUE_SOON
        reminders.Should().HaveCount(1);
        // Invariants via helpers
        reminders.ShouldBeTimeBased();
        reminders.ShouldContainExactlyOneUpcoming();
        syncResult.GeneratedCount.Should().Be(reminders.Count);
        // PK/FK sanity: correct pair keys
        reminders.ShouldBelongToScheduleVehiclePair(serviceScheduleId, vehicleId);
        reminders.ShouldBePersisted();
    }

    [Fact]
    public async Task Should_Enforce_AtMostOneUpcoming_Per_Vehicle_And_ServiceSchedule()
    {
        // ===== Arrange =====
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 15000.0);
        var programId = await CreateServiceProgramAsync();
        var t1 = await CreateServiceTaskAsync();
        var t2 = await CreateServiceTaskAsync();
        var scheduleId = await CreateMileageBasedServiceScheduleAsync(
            programId,
            [t1, t2],
            mileageInterval: 10000,
            mileageBuffer: 1000,
            firstServiceMileage: 30000);
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act =====
        var syncResult = await SyncServiceRemindersAsync();
        syncResult.Success.Should().BeTrue();

        // ===== Assert =====
        var byPair = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);
        byPair.Should().NotBeEmpty();

        // Expectations: at most one UPCOMING for (vehicle, schedule); historical rows allowed
        byPair.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

        // Invariants
        byPair.ShouldBeMileageBased();

        // Should not contain DUE_SOON or OVERDUE
        byPair.Should().NotContain(r =>
            r.Status == ServiceReminderStatusEnum.DUE_SOON ||
            r.Status == ServiceReminderStatusEnum.OVERDUE);

        // Exact due meter for this setup: 30,000 and date must be null
        var upcoming = byPair.Single(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
        upcoming.DueMileage.Should().Be(30000);
        upcoming.DueDate.Should().BeNull();

        // Idempotency without churn: capture UPCOMING ID, re-sync, ensure the same UPCOMING ID remains
        var upcomingId = upcoming.ID;
        await SyncServiceRemindersAsync();
        var after = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);
        after.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING);
        after.Single(r => r.Status == ServiceReminderStatusEnum.UPCOMING).ID.Should().Be(upcomingId);
        // Shape still the same after re-sync
        after.Should().OnlyContain(r =>
            r.ScheduleType == ServiceScheduleTypeEnum.MILEAGE &&
            r.DueMileage != null &&
            r.DueDate == null);

        // PK/FK sanity
        after.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        after.ShouldBePersisted();
    }

    [Fact]
    public async Task Should_Generate_TimeBased_With_MixedStatuses_And_OneUpcoming()
    {
        // ===== Arrange =====
        var todayUtc = GetUtcToday();
        var firstServiceDate = todayUtc.AddDays(-200);

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 50000.0);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 180,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: firstServiceDate
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act =====
        var syncResult = await SyncServiceRemindersAsync();

        // ===== Assert =====
        syncResult.Success.Should().BeTrue();

        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);

        // Scenario math: first = -200, interval = 180, buffer = 30
        // Due dates: -200 (OVERDUE), -20 (OVERDUE), next +160 (UPCOMING), none DUE_SOON.
        reminders.Should().HaveCount(3);
        // Status counts via helper
        reminders.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 2);

        // Sort and verify increasing due dates
        reminders.OrderBy(r => r.DueDate).Should().BeInAscendingOrder(r => r.DueDate);
        reminders.Should().BeInAscendingOrder(r => r.DueDate);
        reminders.ShouldBeTimeBased();
        reminders.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders.ShouldBePersisted();
        syncResult.GeneratedCount.Should().Be(reminders.Count);
        var expected1 = firstServiceDate;
        var expected2 = expected1.AddDays(180);
        var expected3 = expected2.AddDays(180);

        // Assert exact due dates as a set (order already checked above)
        var dueDates = reminders.Select(r => r.DueDate!.Value.Date).ToArray();
        dueDates.Should().BeEquivalentTo([expected1, expected2, expected3]);

        reminders.Should().Contain(r => r.DueDate!.Value.Date == expected1 && r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders.Should().Contain(r => r.DueDate!.Value.Date == expected2 && r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders.Should().Contain(r => r.DueDate!.Value.Date == expected3 && r.Status == ServiceReminderStatusEnum.UPCOMING);
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
        var todayUtc = GetUtcToday();
        var firstServiceDate = todayUtc.AddDays(-170);
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
        var syncResult = await SyncServiceRemindersAsync();
        syncResult.Success.Should().BeTrue();

        // ===== Assert =====
        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);
        syncResult.GeneratedCount.Should().Be(reminders.Count);
        reminders.Should().HaveCount(3);

        // Pure time-based schedule invariants via helper
        reminders.ShouldBeTimeBased();

        // Sort by due date to reason about sequence
        var orderedReminders = reminders.OrderBy(r => r.DueDate).ToList();
        orderedReminders.Should().BeInAscendingOrder(r => r.DueDate);

        reminders.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 1, overdue: 1);

        // Due Dates & Status
        var due1 = firstServiceDate;                       // -170  => OVERDUE
        var due2 = firstServiceDate.AddDays(interval * 1); // +10   => DUE_SOON
        var due3 = firstServiceDate.AddDays(interval * 2); // +190  => UPCOMING

        reminders.Should().ContainSingle(r => r.DueDate!.Value.Date == due1 && r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders.Should().ContainSingle(r => r.DueDate!.Value.Date == due2 && r.Status == ServiceReminderStatusEnum.DUE_SOON);
        reminders.Should().ContainSingle(r => r.DueDate!.Value.Date == due3 && r.Status == ServiceReminderStatusEnum.UPCOMING);

        // PK & FK sanity check
        reminders.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders.ShouldBePersisted();
    }

    [Fact]
    public async Task Should_Generate_MileageBased_With_MixedStatuses_And_OneUpcoming()
    {
        // ===== Arrange =====
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

        // ===== Act =====
        var syncResult = await SyncServiceRemindersAsync();

        // ===== Assert =====
        syncResult.Success.Should().BeTrue();

        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);

        // Scenario math: current = 80k, first = 50k, interval = 30k, buffer = 5k
        // Due mileages: 50k (OVERDUE), 80k (OVERDUE due to equality at due point), 110k (UPCOMING)
        reminders.Should().HaveCount(3);
        reminders.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 2);
        reminders.ShouldBeMileageBased();
        reminders.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders.ShouldBePersisted();
        reminders.OrderBy(r => r.DueMileage).Should().BeInAscendingOrder(r => r.DueMileage);
        syncResult.GeneratedCount.Should().Be(reminders.Count);

        // Verify increasing due mileages without pre-sorting (non-tautological)
        reminders.Should().BeInAscendingOrder(r => r.DueMileage);

        // Assert exact due mileages and no duplicates
        var dueMiles = reminders.Select(r => r.DueMileage!.Value).ToArray();
        dueMiles.Should().BeEquivalentTo([50_000, 80_000, 110_000]);
        reminders.ShouldHaveUniqueDueKey();

        // Exact status per mileage
        reminders.Should().ContainSingle(r => r.DueMileage == 50_000 && r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders.Should().ContainSingle(r => r.DueMileage == 80_000 && r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders.Should().ContainSingle(r => r.DueMileage == 110_000 && r.Status == ServiceReminderStatusEnum.UPCOMING);
    }

    [Fact]
    public async Task Should_Generate_For_Multiple_Vehicles()
    {
        // ===== Arrange =====
        var todayUtc = GetUtcToday();
        var firstServiceDate = todayUtc.AddDays(180);

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId1 = await CreateVehicleAsync(vehicleGroupId);
        var vehicleId2 = await CreateVehicleAsync(vehicleGroupId);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 365,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: firstServiceDate
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId1);
        await AddVehicleToServiceProgramAsync(programId, vehicleId2);

        // ===== Act =====
        var syncResult = await SyncServiceRemindersAsync();

        // ===== Assert =====
        syncResult.Success.Should().BeTrue();

        var upcoming1 = (await GetRemindersForScheduleVehiclePairAsync(vehicleId1, scheduleId))
            .Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Subject;
        var upcoming2 = (await GetRemindersForScheduleVehiclePairAsync(vehicleId2, scheduleId))
            .Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Subject;

        upcoming1.DueDate!.Value.Date.Should().Be(firstServiceDate);
        upcoming2.DueDate!.Value.Date.Should().Be(firstServiceDate);
        upcoming1.ScheduleType.Should().Be(ServiceScheduleTypeEnum.TIME);
        upcoming2.ScheduleType.Should().Be(ServiceScheduleTypeEnum.TIME);
        upcoming1.DueMileage.Should().BeNull();
        upcoming2.DueMileage.Should().BeNull();

        // Assert absence of other statuses for each vehicle (far future due date)
        var all1 = await GetRemindersForScheduleVehiclePairAsync(vehicleId1, scheduleId);
        var all2 = await GetRemindersForScheduleVehiclePairAsync(vehicleId2, scheduleId);
        all1.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 0);
        all2.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 0);
    }

    [Fact]
    public async Task Should_Be_Idempotent_When_Reminders_Already_Exist()
    {
        // ===== Arrange =====
        var firstServiceDate = GetUtcToday().AddDays(-400);

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 365,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: firstServiceDate
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act & Assert =====
        var first = await SyncServiceRemindersAsync();
        first.Success.Should().BeTrue();
        first.GeneratedCount.Should().Be(3);

        // Second run is idempotent (no duplicates)
        var second = await SyncServiceRemindersAsync();
        second.Success.Should().BeTrue();
        second.GeneratedCount.Should().Be(0);

        // Verify persisted state
        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);
        reminders.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders.ShouldBePersisted();

        // Exactly 3 rows with unique due keys
        reminders.Should().HaveCount(3);
        reminders.ShouldHaveUniqueDueKey();

        // Status distribution for (-400, 365, buffer 30)
        reminders.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 2);

        // Invariants
        reminders.ShouldBeTimeBased();

        // Assert exact due dates
        var expected = new[] {
            firstServiceDate,
            firstServiceDate.AddDays(365),
            firstServiceDate.AddDays(730)
        }.Select(d => d.Date);
        reminders.Select(r => r.DueDate!.Value.Date)
                .Should().BeEquivalentTo(expected, opts => opts.WithoutStrictOrdering());

        // Exactly one UPCOMING per (Vehicle, Schedule)
        reminders.ShouldContainExactlyOneUpcoming();
    }

    [Fact]
    public async Task Should_Cancel_Reminders_On_Schedule_SoftDelete()
    {
        // ===== Arrange =====
        var firstServiceDate = GetUtcTodayPlusDays(90);

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 15000.0);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 365,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 60,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: firstServiceDate
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act =====
        // Generate initial
        await SyncServiceRemindersAsync();

        // Soft delete schedule
        await Sender.Send(new DeleteServiceScheduleCommand(scheduleId));

        // Trigger generation which should not generate new since deleted
        await SyncServiceRemindersAsync();

        // ===== Assert =====
        var reminders = await GetRemindersInDbByScheduleAsync(scheduleId);
        // Non-final reminders are deleted; only completed/cancelled or work-order linked may remain
        reminders.ShouldNotContainNonFinalStatuses();
        reminders.ShouldContainOnlyFinalStatusesOrLinked();
    }

    [Fact]
    public async Task Should_Respect_TimeBuffer_With_Weeks_As_DueSoon_Boundary()
    {
        // Verifies the "due soon" boundary logic for a time-based schedule that uses week units.
        // Setup: interval = 8 weeks (56 days), buffer = 2 weeks (14 days).
        // 1) First run with firstServiceDate = today + 10 days (inside the 14-day buffer):
        //   - Expect exactly two reminders:
        //     - DUE_SOON at firstServiceDate (today + 10d)
        //     - UPCOMING at firstServiceDate + 56d (today + 66d)
        //   - No OVERDUE reminders.
        // 2) Update the schedule: firstServiceDate = today + 7 days (still inside buffer) and regenerate:
        //   - Expect exactly two reminders:
        //     - DUE_SOON at today + 7d
        //     - UPCOMING at today + 7d + 56d (today + 63d)
        //   - No OVERDUE reminders.

        // ===== Arrange =====
        var todayUtc = GetUtcToday();
        var firstServiceDate = todayUtc.AddDays(10);

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 8,
            intervalUnit: TimeUnitEnum.Weeks,
            bufferValue: 2,
            bufferUnit: TimeUnitEnum.Weeks,
            firstServiceDate: firstServiceDate);

        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act =====
        var gen1 = await SyncServiceRemindersAsync();
        gen1.Success.Should().BeTrue();
        // Expect 2 reminders: DUE_SOON at +10d, UPCOMING at +66d
        var reminders1 = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);
        reminders1.Should().HaveCount(2);
        reminders1.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 1, overdue: 0);
        reminders1.ShouldBeTimeBased();
        reminders1.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders1.ShouldBePersisted();
        reminders1.Select(r => r.DueDate!.Value.Date)
                .Should().BeEquivalentTo(
                    [firstServiceDate.Date, firstServiceDate.AddDays(56).Date],
                    o => o.WithoutStrictOrdering());

        // Update schedule: move first service to +7d (still inside 14d buffer)
        var newFirstServiceDate = todayUtc.AddDays(7);
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
            FirstServiceDate: newFirstServiceDate,
            FirstServiceMileage: null,
            IsActive: true
        ));

        // Regenerate after update
        var gen2 = await SyncServiceRemindersAsync();
        gen2.Success.Should().BeTrue();

        // ===== Assert =====
        var reminders2 = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);

        // Assert exactly 2: DUE_SOON at +7d, UPCOMING at +63d
        reminders2.Should().HaveCount(2);
        reminders2.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 1, overdue: 0);
        reminders2.Select(r => r.DueDate!.Value.Date)
                .Should().BeEquivalentTo(
                    [newFirstServiceDate.Date, newFirstServiceDate.AddDays(56).Date],
                    o => o.WithoutStrictOrdering());

        // Invariants
        reminders2.ShouldBeTimeBased();
        reminders2.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders2.ShouldBePersisted();

        // No OVERDUE
        reminders1.Should().NotContain(r => r.Status == ServiceReminderStatusEnum.OVERDUE);
        reminders2.Should().NotContain(r => r.Status == ServiceReminderStatusEnum.OVERDUE);
    }

    [Fact]
    public async Task Should_Respect_MileageBuffer_Boundary_As_DueSoon()
    {
        // ===== Arrange =====
        var currentMileage = 40_000;
        var firstServiceMileage = 40_000;
        var interval = 10_000;
        var buffer = 2_000;

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: currentMileage);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateMileageBasedServiceScheduleAsync(
            programId,
            [taskId],
            mileageInterval: interval,
            mileageBuffer: buffer,
            firstServiceMileage: firstServiceMileage
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act =====
        (await SyncServiceRemindersAsync()).Success.Should().BeTrue();

        // Set odometer near due-soon boundary prior to re-generation to reflect status using the command
        // Choose 49_000 to be inside [48_000, 50_000) and below due, ensuring DUE_SOON.
        await Sender.Send(new UpdateVehicleCommand(
            VehicleID: vehicleId,
            Name: "Vehicle Name",
            Make: "TestMake",
            Model: "TestModel",
            Year: 2021,
            VIN: "JHMED9367KS013168",
            LicensePlate: "ABC-123",
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.BUS,
            VehicleGroupID: vehicleGroupId,
            Trim: "Base",
            Mileage: 49_000,
            EngineHours: 300.0,
            FuelCapacity: 50.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: 20000,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Test Yard",
            AssignedTechnicianID: null
        ));

        (await SyncServiceRemindersAsync()).Success.Should().BeTrue();

        // ===== Assert =====
        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);

        // Should contain OVERDUE at 40k, DUE_SOON at 50k, and UPCOMING at 60k
        reminders.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 1, overdue: 1);

        reminders.ShouldBeMileageBased();
        reminders.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders.ShouldBePersisted();
        reminders.ShouldHaveUniqueDueKey();
    }

    [Fact]
    public async Task Should_Use_CurrentMileage_When_FirstServiceMileage_NotProvided()
    {
        // ===== Arrange =====
        var currentMileage = 10_000;
        var interval = 5_000;
        var buffer = 500;
        var expectedFirstDueMileage = currentMileage + interval; // 15_000

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: currentMileage);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateMileageBasedServiceScheduleAsync(
            programId,
            [taskId],
            mileageInterval: interval,
            mileageBuffer: buffer,
            firstServiceMileage: null
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act =====
        var syncResult = await SyncServiceRemindersAsync();

        // ===== Assert =====
        syncResult.Success.Should().BeTrue();

        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);
        reminders.Should().HaveCount(1);

        // Use extensions for shape/ownership/persistence/counts
        reminders.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 0);
        reminders.ShouldBeMileageBased();
        reminders.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders.ShouldBePersisted();
        reminders.ShouldHaveUniqueDueKey();

        var upcomingReminder = reminders
            .Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING)
            .Subject;

        upcomingReminder.DueMileage.Should().BeApproximately(expectedFirstDueMileage, 0.001);
        upcomingReminder.DueDate.Should().BeNull();
        upcomingReminder.ScheduleType.Should().Be(ServiceScheduleTypeEnum.MILEAGE);
        upcomingReminder.VehicleID.Should().Be(vehicleId);
        upcomingReminder.ServiceScheduleID.Should().Be(scheduleId);
    }

    [Fact]
    public async Task Should_Regenerate_On_Schedule_Update()
    {
        // ===== Arrange - Before Update =====
        var todayUtc = GetUtcToday();

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var programId = await CreateServiceProgramAsync();
        var oldTaskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [oldTaskId],
            intervalValue: 180,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 30,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: todayUtc.AddDays(90)
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // ===== Act - Before Update =====
        await SyncServiceRemindersAsync();

        // ===== Assert - Before Update =====
        var before = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);

        // Exactly one active reminder and it's UPCOMING at +90d
        var upcomingBefore = before.Should()
            .ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING)
            .Subject;

        upcomingBefore.DueDate!.Value.Date.Should().Be(todayUtc.AddDays(90));
        upcomingBefore.ScheduleType.Should().Be(ServiceScheduleTypeEnum.TIME);
        upcomingBefore.DueMileage.Should().BeNull();

        // Snapshot reflects original schedule fields
        upcomingBefore.TimeIntervalValue.Should().Be(180);
        upcomingBefore.TimeIntervalUnit.Should().Be(TimeUnitEnum.Days);
        upcomingBefore.TimeBufferValue.Should().Be(30);
        upcomingBefore.TimeBufferUnit.Should().Be(TimeUnitEnum.Days);

        // Old task wired up
        upcomingBefore.ServiceTasks.Should().ContainSingle(t => t.ServiceTaskID == oldTaskId);

        // Invariants and counts via helpers
        before.ShouldBeTimeBased();
        before.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        before.ShouldBePersisted();
        before.ShouldHaveUniqueDueKey();
        before.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 0);

        // ===== Arrange - After Update =====
        var newTaskId = await CreateServiceTaskAsync();

        // ===== Act - After Update =====
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
            FirstServiceDate: todayUtc.AddDays(120),
            FirstServiceMileage: null,
            IsActive: true
        ));

        await SyncServiceRemindersAsync();

        // ===== Assert - After Update =====
        var reminders = await GetRemindersForScheduleVehiclePairAsync(vehicleId, scheduleId);

        // Exactly one reminder and it's UPCOMING
        var upcoming = reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Subject;

        // Old due date (+90) no longer present
        reminders.Should().NotContain(r => r.DueDate!.Value.Date == todayUtc.AddDays(90));

        // New due date is exactly +120 days, time-based, no mileage
        upcoming.DueDate!.Value.Date.Should().Be(todayUtc.AddDays(120));
        upcoming.ScheduleType.Should().Be(ServiceScheduleTypeEnum.TIME);
        upcoming.DueMileage.Should().BeNull();

        // Snapshot reflects updated schedule fields
        upcoming.TimeIntervalValue.Should().Be(365);
        upcoming.TimeIntervalUnit.Should().Be(TimeUnitEnum.Days);
        upcoming.TimeBufferValue.Should().Be(60);
        upcoming.TimeBufferUnit.Should().Be(TimeUnitEnum.Days);

        // Tasks replaced with the new one
        upcoming.ServiceTasks.Should().HaveCount(1);
        upcoming.ServiceTasks.Single().ServiceTaskID.Should().Be(newTaskId);
        reminders.SelectMany(r => r.ServiceTasks).Should().NotContain(t => t.ServiceTaskID == oldTaskId);

        // Invariants / integrity
        reminders.ShouldBelongToScheduleVehiclePair(scheduleId, vehicleId);
        reminders.ShouldBePersisted();
        reminders.ShouldHaveUniqueDueKey();
        reminders.ShouldBeTimeBased();
        reminders.ShouldHaveStatusCounts(upcoming: 1, dueSoon: 0, overdue: 0);
    }

    [Fact]
    public async Task Should_Ignore_ScheduleCreation_When_Program_Inactive()
    {
        // ===== Arrange =====
        var todayUtc = GetUtcToday();

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 15000.0);
        var programId = await CreateServiceProgramAsync(isActive: false); // Inactive Program
        var taskId = await CreateServiceTaskAsync();

        // ===== Act & Assert =====
        // Creating a schedule under an inactive program should fail upfront
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
            FirstServiceDate: todayUtc.AddDays(30),
            FirstServiceMileage: null,
            IsActive: true
        ))).Should().ThrowAsync<EntityNotFoundException>();
    }

    [Fact]
    public async Task Should_Preserve_Final_And_WorkOrderLinked_Reminders_When_Schedule_Deleted()
    {
        // ===== Arrange =====
        var todayUtc = GetUtcToday();

        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId, mileage: 20000.0);
        var programId = await CreateServiceProgramAsync();
        var taskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            programId,
            [taskId],
            intervalValue: 90,
            intervalUnit: TimeUnitEnum.Days,
            bufferValue: 7,
            bufferUnit: TimeUnitEnum.Days,
            firstServiceDate: todayUtc.AddDays(-100) // ensures some overdue
        );
        await AddVehicleToServiceProgramAsync(programId, vehicleId);

        // Generate initial reminders
        await SyncServiceRemindersAsync();

        // Pull reminders for this pair
        var initial = await GetRemindersInDbByScheduleAsync(scheduleId);
        initial.Should().NotBeEmpty();

        // Mark one as COMPLETED and one as CANCELLED, and link one to a WorkOrder
        var toComplete = initial.FirstOrDefault(r => r.Status != ServiceReminderStatusEnum.COMPLETED);
        var toCancel = initial.FirstOrDefault(r => r != toComplete && r.Status != ServiceReminderStatusEnum.CANCELLED);
        toComplete.Should().NotBeNull();
        toCancel.Should().NotBeNull();

        // Create a minimal work order row via direct update substitute: set a fake WorkOrderID
        // (Integration boundary: assume 9999 is a placeholder; only linkage matters for preservation)
        await DbContext.ServiceReminders.Where(r => r.ID == toComplete!.ID)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, ServiceReminderStatusEnum.COMPLETED));
        await DbContext.ServiceReminders.Where(r => r.ID == toCancel!.ID)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.Status, ServiceReminderStatusEnum.CANCELLED));

        // To link to a work order, create a technician user, then create a real work order and link
        var technicianId = await Sender.Send(new Application.Features.Users.Command.CreateTechnician.CreateTechnicianCommand(
            Email: $"tech_{Faker.Random.AlphaNumeric(6)}@example.com",
            Password: Faker.Internet.Password(length: 50, regexPattern: @"[a-zA-Z0-9!@#$%^&*()_+=\-]"),
            FirstName: Faker.Name.FirstName(),
            LastName: Faker.Name.LastName(),
            HireDate: DateTime.UtcNow.AddYears(-1),
            IsActive: true
        ));

        var woId = await Sender.Send(new Application.Features.WorkOrders.Command.CreateWorkOrder.CreateWorkOrderCommand(
            VehicleID: vehicleId,
            AssignedToUserID: technicianId.ToString(),
            Title: $"WO {Faker.Random.AlphaNumeric(5)}",
            Description: "",
            WorkOrderType: WorkTypeEnum.SCHEDULED,
            PriorityLevel: PriorityLevelEnum.MEDIUM,
            Status: WorkOrderStatusEnum.CREATED,
            // Validator uses DateTime.UtcNow, so base these on real clock, not FakeTime
            ScheduledStartDate: DateTime.UtcNow.Date.AddDays(1),
            ActualStartDate: null,
            ScheduledCompletionDate: DateTime.UtcNow.Date.AddDays(2),
            ActualCompletionDate: null,
            StartOdometer: 0,
            EndOdometer: null,
            IssueIdList: [],
            WorkOrderLineItems: []
        ));
        var toLink = initial.First(r => r.ID != toComplete!.ID && r.ID != toCancel!.ID);
        await DbContext.ServiceReminders.Where(r => r.ID == toLink.ID)
            .ExecuteUpdateAsync(s => s.SetProperty(p => p.WorkOrderID, woId));

        // ===== Act =====
        // Soft delete the schedule, then sync
        await Sender.Send(new DeleteServiceScheduleCommand(scheduleId));
        await SyncServiceRemindersAsync();

        // ===== Assert =====
        var after = await GetRemindersInDbByScheduleAsync(scheduleId);
        after.ShouldNotContainNonFinalStatuses();
        after.ShouldContainOnlyFinalStatusesOrLinked();
    }

    // ===================
    // ===== HELPERS =====
    // ===================

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

    private async Task<SyncServiceRemindersResponse> SyncServiceRemindersAsync()
    {
        return await Sender.Send(new SyncServiceRemindersCommand());
    }

    private async Task<List<ServiceReminderDTO>> GetRemindersForScheduleVehiclePairAsync(int vehicleId, int scheduleId, int pageSize = 100)
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

public static class SyncServiceRemindersIntegrationTestsExtensions
{
    public static void ShouldHaveDueDateXorDueMileage(this IEnumerable<ServiceReminderDTO> reminders)
        => reminders.Should().OnlyContain(r => r.DueDate.HasValue ^ r.DueMileage.HasValue);

    public static void ShouldBeTimeBased(this IEnumerable<ServiceReminderDTO> reminders)
        => reminders.Should().OnlyContain(r =>
            r.ScheduleType == ServiceScheduleTypeEnum.TIME &&
            r.DueDate != null &&
            r.DueMileage == null);

    public static void ShouldBeMileageBased(this IEnumerable<ServiceReminderDTO> reminders)
        => reminders.Should().OnlyContain(r =>
            r.ScheduleType == ServiceScheduleTypeEnum.MILEAGE &&
            r.DueMileage != null &&
            r.DueDate == null);

    public static void ShouldHaveUniqueDueKey(this IEnumerable<ServiceReminderDTO> reminders)
        => reminders.Select(r => (r.DueDate, r.DueMileage)).Should().OnlyHaveUniqueItems();

    // UPCOMING RULE
    public static void ShouldContainExactlyOneUpcoming(this IEnumerable<ServiceReminderDTO> reminders)
        => reminders.Should().ContainSingle(r => r.Status == ServiceReminderStatusEnum.UPCOMING);

    // STATUS COUNTS
    public static void ShouldHaveStatusCounts(
        this IEnumerable<ServiceReminderDTO> reminders,
        int upcoming, int dueSoon, int overdue)
    {
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.UPCOMING).Should().Be(upcoming, $"{nameof(ServiceReminderStatusEnum.UPCOMING)} count mismatch");
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.DUE_SOON).Should().Be(dueSoon, $"{nameof(ServiceReminderStatusEnum.DUE_SOON)} count mismatch");
        reminders.Count(r => r.Status == ServiceReminderStatusEnum.OVERDUE).Should().Be(overdue, $"{nameof(ServiceReminderStatusEnum.OVERDUE)} count mismatch");
    }

    // OWNERSHIP & PERSISTENCE
    public static void ShouldBelongToScheduleVehiclePair(this IEnumerable<ServiceReminderDTO> reminders, int scheduleId, int vehicleId)
        => reminders.Should().OnlyContain(r => r.ServiceScheduleID == scheduleId && r.VehicleID == vehicleId);

    public static void ShouldBePersisted(this IEnumerable<ServiceReminderDTO> reminders)
        => reminders.Should().OnlyContain(r => r.ID > 0);

    // NON-FINAL/FINAL
    public static void ShouldNotContainNonFinalStatuses(this IEnumerable<ServiceReminder> reminders)
        => reminders.Should().NotContain(r =>
            r.Status == ServiceReminderStatusEnum.UPCOMING ||
            r.Status == ServiceReminderStatusEnum.DUE_SOON ||
            r.Status == ServiceReminderStatusEnum.OVERDUE);

    public static void ShouldContainOnlyFinalStatusesOrLinked(this IEnumerable<ServiceReminder> reminders)
        => reminders.Should().OnlyContain(r =>
            r.Status == ServiceReminderStatusEnum.COMPLETED ||
            r.Status == ServiceReminderStatusEnum.CANCELLED ||
            r.WorkOrderID != null);
}