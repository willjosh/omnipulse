using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceReminders.Command.AddServiceReminderToExistingWorkOrder;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.Users.Command.CreateTechnician;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Features.WorkOrders.Command.CreateWorkOrder;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ServiceReminders;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceReminder))]
public class AddServiceReminderToExistingWorkOrderIntegrationTests : BaseIntegrationTest
{
    public AddServiceReminderToExistingWorkOrderIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_LinkServiceReminderToWorkOrder_When_CommandIsValid()
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

        // Arrange - Create ServiceSchedule (time-based only - XOR constraint)
        var createServiceScheduleCommand = new CreateServiceScheduleCommand(
            ServiceProgramID: serviceProgramId,
            Name: $"Test ServiceSchedule {Faker.Random.AlphaNumeric(5)}",
            ServiceTaskIDs: [serviceTaskId],
            TimeIntervalValue: 30,
            TimeIntervalUnit: TimeUnitEnum.Days,
            TimeBufferValue: 5,
            TimeBufferUnit: TimeUnitEnum.Days,
            MileageInterval: null, // XOR: time-based only
            MileageBuffer: null,
            FirstServiceDate: DateTime.Today.AddDays(3),
            FirstServiceMileage: null // XOR: time-based only
        );
        int serviceScheduleId = await Sender.Send(createServiceScheduleCommand);

        // Arrange - Add Vehicle to ServiceProgram (this will create ServiceReminders)
        var addVehicleToServiceProgramCommand = new AddVehicleToServiceProgramCommand(
            ServiceProgramID: serviceProgramId,
            VehicleID: vehicleId
        );
        await Sender.Send(addVehicleToServiceProgramCommand);

        // Arrange - Trigger service reminder generation
        var getAllServiceRemindersQuery = new GetAllServiceRemindersQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });
        await Sender.Send(getAllServiceRemindersQuery);

        // Arrange - Create User (Technician)
        var createTechnicianCommand = new CreateTechnicianCommand(
            Email: $"technician_{Faker.Random.AlphaNumeric(5)}@omnipulse.com",
            Password: Faker.Internet.Password(length: 50, regexPattern: @"[a-zA-Z0-9!@#$%^&*()_+=\-]"),
            FirstName: Faker.Name.FirstName(),
            LastName: Faker.Name.LastName(),
            HireDate: DateTime.UtcNow.AddYears(-1),
            IsActive: true
        );
        Guid technicianId = await Sender.Send(createTechnicianCommand);

        // Arrange - Create WorkOrder
        var createWorkOrderCommand = new CreateWorkOrderCommand(
            VehicleID: vehicleId,
            AssignedToUserID: technicianId.ToString(),
            Title: "Test Work Order",
            Description: "Test work order description",
            WorkOrderType: WorkTypeEnum.SCHEDULED,
            PriorityLevel: PriorityLevelEnum.MEDIUM,
            Status: WorkOrderStatusEnum.CREATED,
            ScheduledStartDate: DateTime.Today.AddDays(1),
            ActualStartDate: null,
            ScheduledCompletionDate: DateTime.Today.AddDays(2),
            ActualCompletionDate: null,
            StartOdometer: 15000.0,
            EndOdometer: null,
            IssueIdList: [],
            WorkOrderLineItems: []
        );
        int workOrderId = await Sender.Send(createWorkOrderCommand);

        // Get the created ServiceReminder from the DB
        var serviceReminder = await DbContext.ServiceReminders
            .FirstOrDefaultAsync(sr => sr.VehicleID == vehicleId && sr.ServiceScheduleID == serviceScheduleId);

        serviceReminder.Should().NotBeNull();
        serviceReminder!.WorkOrderID.Should().BeNull(); // Should not be linked initially

        // Act
        var addServiceReminderToWorkOrderCommand = new AddServiceReminderToExistingWorkOrderCommand(
            ServiceReminderID: serviceReminder.ID,
            WorkOrderID: workOrderId
        );
        int returnedWorkOrderId = await Sender.Send(addServiceReminderToWorkOrderCommand);

        // Assert
        returnedWorkOrderId.Should().Be(workOrderId);

        // Verify the service reminder is now linked to the work order
        var updatedServiceReminder = await DbContext.ServiceReminders.FindAsync(serviceReminder.ID);
        updatedServiceReminder.Should().NotBeNull();
        updatedServiceReminder!.WorkOrderID.Should().Be(workOrderId);

        updatedServiceReminder.Status.Should().Be(ServiceReminderStatusEnum.UPCOMING);
    }
}