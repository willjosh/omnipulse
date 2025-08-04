using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceTasks.Command.CreateServiceTask;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.ServiceSchedules;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceSchedule))]
public class CreateServiceScheduleIntegrationTests : BaseIntegrationTest
{
    public CreateServiceScheduleIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddServiceScheduleToDatabase_When_CommandIsValid()
    {
        // Arrange - Dependencies
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_AddServiceScheduleToDatabase_When_CommandIsValid)}",
            Description: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_AddServiceScheduleToDatabase_When_CommandIsValid)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_AddServiceScheduleToDatabase_When_CommandIsValid)}",
            Description: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_AddServiceScheduleToDatabase_When_CommandIsValid)} {Faker.Random.AlphaNumeric(5)}",
            EstimatedLabourHours: 2.5,
            EstimatedCost: 150.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange
        var createCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Test {nameof(ServiceSchedule)} {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 30,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 5,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: 5000,
            MileageBuffer: 500,
            FirstServiceDate: DateTime.Today.AddDays(7),
            FirstServiceMileage: 1000,
            IsActive: true
        );

        // Act
        int createdServiceScheduleId = await Sender.Send(createCommand);

        // Assert
        ServiceSchedule? createdServiceScheduleEntity = await DbContext.ServiceSchedules.FindAsync(createdServiceScheduleId);
        createdServiceScheduleEntity.Should().NotBeNull();

        createdServiceScheduleEntity.ServiceProgramID.Should().Be(createCommand.ServiceProgramID);
        createdServiceScheduleEntity.Name.Should().Be(createCommand.Name);
        createdServiceScheduleEntity.TimeIntervalValue.Should().Be(createCommand.TimeIntervalValue);
        createdServiceScheduleEntity.TimeIntervalUnit.Should().Be(createCommand.TimeIntervalUnit);
        createdServiceScheduleEntity.TimeBufferValue.Should().Be(createCommand.TimeBufferValue);
        createdServiceScheduleEntity.TimeBufferUnit.Should().Be(createCommand.TimeBufferUnit);
        createdServiceScheduleEntity.MileageInterval.Should().Be(createCommand.MileageInterval);
        createdServiceScheduleEntity.MileageBuffer.Should().Be(createCommand.MileageBuffer);
        createdServiceScheduleEntity.FirstServiceMileage.Should().Be(createCommand.FirstServiceMileage);
        createdServiceScheduleEntity.IsActive.Should().Be(createCommand.IsActive);
    }
}