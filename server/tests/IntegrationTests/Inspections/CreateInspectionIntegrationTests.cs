using Application.Features.InspectionFormItems.Command.CreateInspectionFormItem;
using Application.Features.InspectionForms.Command.CreateInspectionForm;
using Application.Features.Inspections.Command.CreateInspection;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;
using IntegrationTests.Users;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Inspections;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(Inspection))]
public sealed class CreateInspectionIntegrationTests : BaseIntegrationTest
{
    public CreateInspectionIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_CreateInspectionWithPassFailItems_When_CommandIsValid()
    {
        // ===== Arrange - Dependencies =====

        // Create Technician
        var technician = await CreateUserIntegrationTests.CreateTechnicianAsync(Sender, DbContext, Faker);

        // Create Vehicle
        var vehicle = await CreateVehicleAsync();

        // Create InspectionForm and its Items
        var form = await AddTestInspectionFormAsync();
        var formItems = await AddTestInspectionFormItemsAsync(form.ID, count: 2);

        // Create Command with Pass/Fail Items
        var inspectionItems = formItems.Select(item =>
            new CreateInspectionPassFailItemCommand(
                InspectionFormItemID: item.ID,
                Passed: true,
                Comment: $"{nameof(InspectionPassFailItem.Comment)} - {Faker.Lorem.Sentence()}"
            )).ToList();

        var command = new CreateInspectionCommand(
            InspectionFormID: form.ID,
            VehicleID: vehicle.ID,
            TechnicianID: technician.Id,
            InspectionStartTime: DateTime.UtcNow.AddMinutes(-10),
            InspectionEndTime: DateTime.UtcNow,
            OdometerReading: 99999,
            VehicleCondition: VehicleConditionEnum.Excellent,
            Notes: "Routine safety check passed.",
            InspectionItems: inspectionItems
        );

        // ===== Act =====
        int inspectionId = await Sender.Send(command);

        // ===== Assert =====
        var inspection = await DbContext.Inspections
            .Include(i => i.InspectionPassFailItems)
            .FirstOrDefaultAsync(i => i.ID == inspectionId);

        inspection.Should().NotBeNull();
        inspection!.TechnicianID.Should().Be(command.TechnicianID);
        inspection.VehicleID.Should().Be(command.VehicleID);
        inspection.Notes.Should().Be(command.Notes);
        inspection.VehicleCondition.Should().Be(command.VehicleCondition);
        inspection.OdometerReading.Should().Be(command.OdometerReading);

        inspection.InspectionPassFailItems.Should().HaveCount(command.InspectionItems.Count);
        foreach (var submittedItem in command.InspectionItems)
        {
            var item = inspection.InspectionPassFailItems.First(i => i.InspectionFormItemID == submittedItem.InspectionFormItemID);
            item.Passed.Should().Be(submittedItem.Passed);
            item.Comment.Should().Be(submittedItem.Comment);
        }
    }

    private async Task<InspectionForm> AddTestInspectionFormAsync()
    {
        var command = new CreateInspectionFormCommand(
            Title: $"Form {Faker.Random.AlphaNumeric(6)}",
            Description: $"Description {Faker.Random.AlphaNumeric(10)}",
            IsActive: true
        );

        var formId = await Sender.Send(command);
        var form = await DbContext.InspectionForms.FindAsync(formId);
        form.Should().NotBeNull();

        return form!;
    }

    private async Task<List<InspectionFormItem>> AddTestInspectionFormItemsAsync(int inspectionFormId, int count = 1)
    {
        var items = new List<InspectionFormItem>();

        for (int i = 0; i < count; i++)
        {
            var createItemCommand = new CreateInspectionFormItemCommand(
                InspectionFormID: inspectionFormId,
                ItemLabel: $"Checklist Item {Faker.Random.Word()} {i + 1}",
                ItemDescription: Faker.Lorem.Sentence(),
                ItemInstructions: Faker.Lorem.Sentence(),
                IsRequired: true,
                InspectionFormItemTypeEnum: InspectionFormItemTypeEnum.PassFail
            );

            var itemId = await Sender.Send(createItemCommand);
            var item = await DbContext.InspectionFormItems.FindAsync(itemId);
            item.Should().NotBeNull();

            items.Add(item!);
        }

        return items;
    }

    private async Task<Vehicle> CreateVehicleAsync()
    {
        // Create Vehicle Group dependency
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"Group_{Faker.Random.AlphaNumeric(6)}",
            Description: $"Test group for vehicle creation",
            IsActive: true
        );
        var groupId = await Sender.Send(createVehicleGroupCommand);

        // Create Vehicle
        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.Int(1000, 9999)}",
            Make: Faker.Vehicle.Manufacturer(),
            Model: Faker.Vehicle.Model(),
            Year: 2024,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: $"TEST{Faker.Random.AlphaNumeric(3)}",
            LicensePlateExpirationDate: DateTime.UtcNow.AddYears(1),
            VehicleType: VehicleTypeEnum.VAN,
            VehicleGroupID: groupId,
            Trim: "Standard",
            Mileage: 10,
            EngineHours: 0,
            FuelCapacity: 70,
            FuelType: FuelTypeEnum.DIESEL,
            PurchaseDate: DateTime.UtcNow.AddDays(-5),
            PurchasePrice: 50000,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Testing Depot",
            AssignedTechnicianID: null
        );

        var vehicleId = await Sender.Send(createVehicleCommand);
        var vehicle = await DbContext.Vehicles.FindAsync(vehicleId);
        vehicle.Should().NotBeNull();

        return vehicle!;
    }
}