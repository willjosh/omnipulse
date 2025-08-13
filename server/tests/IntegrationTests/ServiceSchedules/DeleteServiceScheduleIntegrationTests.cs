using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceSchedules.Command.DeleteServiceSchedule;
using Application.Features.ServiceTasks.Command.CreateServiceTask;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ServiceSchedules;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceSchedule))]
public class DeleteServiceScheduleIntegrationTests : BaseIntegrationTest
{
    public DeleteServiceScheduleIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task SoftDelete_Allows_Creating_Schedule_With_Same_Name()
    {
        // ===== Arrange =====
        var programId = await CreateServiceProgramAsync();

        var scheduleName = $"Service Schedule Name {Faker.Random.AlphaNumeric(6)}";

        var serviceTaskId = await CreateServiceTaskAsync();
        var scheduleId = await CreateTimeBasedServiceScheduleAsync(
            serviceProgramId: programId,
            serviceTaskIds: [serviceTaskId],
            intervalValue: 4,
            intervalUnit: TimeUnitEnum.Weeks,
            bufferValue: 1,
            bufferUnit: TimeUnitEnum.Weeks,
            firstServiceDate: null,
            name: scheduleName
        );

        // Soft delete
        var deleteScheduleCommand = new DeleteServiceScheduleCommand(scheduleId);
        var deletedId = await Sender.Send(deleteScheduleCommand);
        deletedId.Should().Be(scheduleId);

        // ===== Act =====
        var recreatedId = await CreateTimeBasedServiceScheduleAsync(
            serviceProgramId: programId,
            serviceTaskIds: [serviceTaskId],
            intervalValue: 8,
            intervalUnit: TimeUnitEnum.Weeks,
            bufferValue: 1,
            bufferUnit: TimeUnitEnum.Weeks,
            firstServiceDate: null,
            name: scheduleName
        );

        // ===== Assert =====
        recreatedId.Should().BeGreaterThan(0);
        recreatedId.Should().NotBe(scheduleId);

        // Verify DB state
        var firstSchedule = await DbContext.ServiceSchedules
            .IgnoreQueryFilters()
            .FirstOrDefaultAsync(s => s.ID == scheduleId);
        firstSchedule.Should().NotBeNull();
        firstSchedule!.IsSoftDeleted.Should().BeTrue();
        firstSchedule.Name.Should().Be(scheduleName);
        firstSchedule.ServiceProgramID.Should().Be(programId);

        var secondSchedule = await DbContext.ServiceSchedules.FindAsync(recreatedId);
        secondSchedule.Should().NotBeNull();
        secondSchedule!.IsSoftDeleted.Should().BeFalse();
        secondSchedule.Name.Should().Be(scheduleName);
        secondSchedule.ServiceProgramID.Should().Be(programId);

        // Only one active (non-deleted) schedule with this name under the program
        var activeCount = await DbContext.ServiceSchedules
            .CountAsync(s => s.ServiceProgramID == programId && s.Name == scheduleName);
        activeCount.Should().Be(1);

        // Total including soft-deleted should be 2
        var totalCount = await DbContext.ServiceSchedules
            .IgnoreQueryFilters()
            .CountAsync(s => s.ServiceProgramID == programId && s.Name == scheduleName);
        totalCount.Should().Be(2);
    }
}