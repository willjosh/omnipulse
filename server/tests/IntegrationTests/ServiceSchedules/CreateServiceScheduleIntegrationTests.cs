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
            MileageInterval: null, // XOR: time-based only
            MileageBuffer: null,
            FirstServiceDate: DateTime.Today.AddDays(7),
            FirstServiceMileage: null, // XOR: time-based only
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
        createdServiceScheduleEntity.MileageInterval.Should().BeNull();
        createdServiceScheduleEntity.MileageBuffer.Should().BeNull();
        createdServiceScheduleEntity.FirstServiceMileage.Should().BeNull();
        createdServiceScheduleEntity.IsActive.Should().Be(createCommand.IsActive);
    }

    [Fact]
    public async Task Should_CreateTimeOnlySchedule_When_OnlyTimeParametersProvided()
    {
        // Arrange - Dependencies
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateTimeOnlySchedule_When_OnlyTimeParametersProvided)}",
            Description: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateTimeOnlySchedule_When_OnlyTimeParametersProvided)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateTimeOnlySchedule_When_OnlyTimeParametersProvided)}",
            Description: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateTimeOnlySchedule_When_OnlyTimeParametersProvided)} {Faker.Random.AlphaNumeric(5)}",
            EstimatedLabourHours: 1.5,
            EstimatedCost: 75.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Time-only schedule with first service date
        var createCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Time-Only Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 14,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 3,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: null, // No mileage scheduling
            MileageBuffer: null,
            FirstServiceDate: DateTime.Today.AddDays(14),
            FirstServiceMileage: null, // No mileage scheduling
            IsActive: true
        );

        // Act
        int createdServiceScheduleId = await Sender.Send(createCommand);

        // Assert
        ServiceSchedule? createdServiceScheduleEntity = await DbContext.ServiceSchedules.FindAsync(createdServiceScheduleId);
        createdServiceScheduleEntity.Should().NotBeNull();

        // Verify time-based properties are set
        createdServiceScheduleEntity!.TimeIntervalValue.Should().Be(14);
        createdServiceScheduleEntity.TimeIntervalUnit.Should().Be(TimeUnitEnum.Days);
        createdServiceScheduleEntity.TimeBufferValue.Should().Be(3);
        createdServiceScheduleEntity.TimeBufferUnit.Should().Be(TimeUnitEnum.Days);
        createdServiceScheduleEntity.FirstServiceDate.Should().Be(DateTime.Today.AddDays(14));

        // Verify mileage-based properties are null
        createdServiceScheduleEntity.MileageInterval.Should().BeNull();
        createdServiceScheduleEntity.MileageBuffer.Should().BeNull();
        createdServiceScheduleEntity.FirstServiceMileage.Should().BeNull();
    }

    [Fact]
    public async Task Should_CreateMileageOnlySchedule_When_OnlyMileageParametersProvided()
    {
        // Arrange - Dependencies
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateMileageOnlySchedule_When_OnlyMileageParametersProvided)}",
            Description: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateMileageOnlySchedule_When_OnlyMileageParametersProvided)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateMileageOnlySchedule_When_OnlyMileageParametersProvided)}",
            Description: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateMileageOnlySchedule_When_OnlyMileageParametersProvided)} {Faker.Random.AlphaNumeric(5)}",
            EstimatedLabourHours: 3.0,
            EstimatedCost: 200.00m,
            Category: ServiceTaskCategoryEnum.CORRECTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Mileage-only schedule with first service mileage
        var createCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Mileage-Only Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: null, // No time scheduling
            TimeIntervalUnit: null,
            TimeBufferValue: null,
            TimeBufferUnit: null,
            MileageInterval: 10000,
            MileageBuffer: 1000,
            FirstServiceDate: null, // No time scheduling
            FirstServiceMileage: 5000,
            IsActive: true
        );

        // Act
        int createdServiceScheduleId = await Sender.Send(createCommand);

        // Assert
        ServiceSchedule? createdServiceScheduleEntity = await DbContext.ServiceSchedules.FindAsync(createdServiceScheduleId);
        createdServiceScheduleEntity.Should().NotBeNull();

        // Verify mileage-based properties are set
        createdServiceScheduleEntity!.MileageInterval.Should().Be(10000);
        createdServiceScheduleEntity.MileageBuffer.Should().Be(1000);
        createdServiceScheduleEntity.FirstServiceMileage.Should().Be(5000);

        // Verify time-based properties are null
        createdServiceScheduleEntity.TimeIntervalValue.Should().BeNull();
        createdServiceScheduleEntity.TimeIntervalUnit.Should().BeNull();
        createdServiceScheduleEntity.TimeBufferValue.Should().BeNull();
        createdServiceScheduleEntity.TimeBufferUnit.Should().BeNull();
        createdServiceScheduleEntity.FirstServiceDate.Should().BeNull();
    }

    [Fact]
    public async Task Should_CreateTimeOnlySchedule_When_NoFirstServiceDateProvided()
    {
        // Arrange - Dependencies
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateTimeOnlySchedule_When_NoFirstServiceDateProvided)}",
            Description: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateTimeOnlySchedule_When_NoFirstServiceDateProvided)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateTimeOnlySchedule_When_NoFirstServiceDateProvided)}",
            Description: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateTimeOnlySchedule_When_NoFirstServiceDateProvided)} {Faker.Random.AlphaNumeric(5)}",
            EstimatedLabourHours: 0.5,
            EstimatedCost: 25.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Time-only schedule without first service date
        var createCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Time-Only No-First-Date Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 7,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 1,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: null, // No mileage scheduling
            MileageBuffer: null,
            FirstServiceDate: null, // No first service date
            FirstServiceMileage: null, // No mileage scheduling
            IsActive: true
        );

        // Act
        int createdServiceScheduleId = await Sender.Send(createCommand);

        // Assert
        ServiceSchedule? createdServiceScheduleEntity = await DbContext.ServiceSchedules.FindAsync(createdServiceScheduleId);
        createdServiceScheduleEntity.Should().NotBeNull();

        // Verify time-based properties are set (except first service date)
        createdServiceScheduleEntity!.TimeIntervalValue.Should().Be(7);
        createdServiceScheduleEntity.TimeIntervalUnit.Should().Be(TimeUnitEnum.Days);
        createdServiceScheduleEntity.TimeBufferValue.Should().Be(1);
        createdServiceScheduleEntity.TimeBufferUnit.Should().Be(TimeUnitEnum.Days);
        createdServiceScheduleEntity.FirstServiceDate.Should().BeNull(); // Should be null

        // Verify mileage-based properties are null
        createdServiceScheduleEntity.MileageInterval.Should().BeNull();
        createdServiceScheduleEntity.MileageBuffer.Should().BeNull();
        createdServiceScheduleEntity.FirstServiceMileage.Should().BeNull();
    }

    [Fact]
    public async Task Should_CreateMileageOnlySchedule_When_NoFirstServiceMileageProvided()
    {
        // Arrange - Dependencies
        var createServiceProgramCommand = new CreateServiceProgramCommand(
            Name: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateMileageOnlySchedule_When_NoFirstServiceMileageProvided)}",
            Description: $"[Dependency] {nameof(ServiceProgram)} - {nameof(Should_CreateMileageOnlySchedule_When_NoFirstServiceMileageProvided)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int serviceProgramId = await Sender.Send(createServiceProgramCommand);

        var createServiceTaskCommand = new CreateServiceTaskCommand(
            Name: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateMileageOnlySchedule_When_NoFirstServiceMileageProvided)}",
            Description: $"[Dependency] {nameof(ServiceTask)} - {nameof(Should_CreateMileageOnlySchedule_When_NoFirstServiceMileageProvided)} {Faker.Random.AlphaNumeric(5)}",
            EstimatedLabourHours: 4.0,
            EstimatedCost: 300.00m,
            Category: ServiceTaskCategoryEnum.EMERGENCY,
            IsActive: true
        );
        int serviceTaskId = await Sender.Send(createServiceTaskCommand);

        // Arrange - Mileage-only schedule without first service mileage
        var createCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Mileage-Only No-First-Mileage Schedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: null, // No time scheduling
            TimeIntervalUnit: null,
            TimeBufferValue: null,
            TimeBufferUnit: null,
            MileageInterval: 15000,
            MileageBuffer: 2000,
            FirstServiceDate: null, // No time scheduling
            FirstServiceMileage: null, // No first service mileage
            IsActive: true
        );

        // Act
        int createdServiceScheduleId = await Sender.Send(createCommand);

        // Assert
        ServiceSchedule? createdServiceScheduleEntity = await DbContext.ServiceSchedules.FindAsync(createdServiceScheduleId);
        createdServiceScheduleEntity.Should().NotBeNull();

        // Verify mileage-based properties are set (except first service mileage)
        createdServiceScheduleEntity!.MileageInterval.Should().Be(15000);
        createdServiceScheduleEntity.MileageBuffer.Should().Be(2000);
        createdServiceScheduleEntity.FirstServiceMileage.Should().BeNull(); // Should be null

        // Verify time-based properties are null
        createdServiceScheduleEntity.TimeIntervalValue.Should().BeNull();
        createdServiceScheduleEntity.TimeIntervalUnit.Should().BeNull();
        createdServiceScheduleEntity.TimeBufferValue.Should().BeNull();
        createdServiceScheduleEntity.TimeBufferUnit.Should().BeNull();
        createdServiceScheduleEntity.FirstServiceDate.Should().BeNull();
    }
}