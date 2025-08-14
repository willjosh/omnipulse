using Application.Features.InventoryItemLocations.Command;
using Application.Features.InventoryItems.Command.CreateInventoryItem;
using Application.Features.ServicePrograms.Command.AddVehicleToServiceProgram;
using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServiceReminders.Command.SyncServiceReminders;
using Application.Features.ServiceReminders.Query;
using Application.Features.ServiceReminders.Query.GetAllServiceReminders;
using Application.Features.ServiceSchedules.Command.CreateServiceSchedule;
using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.Users.Command.CreateTechnician;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Models.PaginationModels;

using Bogus;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using MediatR;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

using Persistence.DatabaseContext;

namespace IntegrationTests.Abstractions;

/// <summary>
/// Base class for integration tests.
/// </summary>
[Trait("TestCategory", "Integration")]
[Collection("IntegrationTests")]
public abstract class BaseIntegrationTest : IClassFixture<IntegrationTestWebAppFactory>, IDisposable
{
    private readonly IServiceScope _scope;
    protected virtual DateTimeOffset BaselineNow => new(2025, 6, 2, 9, 0, 0, TimeSpan.Zero);

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        Factory = factory;

        // Ensure fake time is not before BaselineNow (FakeTimeProvider can't move backwards)
        DateTimeOffset now = Factory.FakeTimeProvider.GetUtcNow();
        if (now < BaselineNow)
        {
            Factory.FakeTimeProvider.SetUtcNow(BaselineNow);
        }

        _scope = Factory.Services.CreateScope();
        Sender = _scope.ServiceProvider.GetRequiredService<ISender>();
        DbContext = _scope.ServiceProvider.GetRequiredService<OmnipulseDatabaseContext>();

        Faker = new Faker();
    }

    protected IntegrationTestWebAppFactory Factory { get; }

    protected ISender Sender { get; }
    protected OmnipulseDatabaseContext DbContext { get; }
    protected Faker Faker { get; }

    protected DateTime GetUtcToday()
        => DateTime.SpecifyKind(Factory.FakeTimeProvider.GetUtcNow().UtcDateTime.Date, DateTimeKind.Utc);

    protected DateTime GetUtcTodayPlusDays(int days)
        => DateTime.SpecifyKind(Factory.FakeTimeProvider.GetUtcNow().UtcDateTime.Date.AddDays(days), DateTimeKind.Utc);

    /// <summary>Advance the fake clock by a duration.</summary>
    /// <param name="by">The amount of time to advance.</param>
    /// <example><c>ClockAdvance(TimeSpan.FromDays(1))</c></example>
    protected void ClockAdvance(TimeSpan by) => Factory.FakeTimeProvider.Advance(by);

    /// <summary>Set the fake clock to an exact instant.</summary>
    /// <remarks>Can throw if you pass a time earlier than current fake time</remarks>
    protected void ClockSetNow(DateTimeOffset now) => Factory.FakeTimeProvider.SetUtcNow(now);

    public void Dispose()
    {
        _scope.Dispose();
        GC.SuppressFinalize(this); // Prevent finalisation if subclass adds a finaliser
    }

    [Fact]
    public void Should_HaveDatabaseContext_When_TestStarts()
    {
        // Assert
        DbContext.Should().NotBeNull();
    }

    [Fact]
    public void Should_HaveSender_When_TestStarts()
    {
        // Assert
        Sender.Should().NotBeNull();
    }

    [Fact]
    public void Should_HaveFaker_When_TestStarts()
    {
        // Assert
        Faker.Should().NotBeNull();
    }

    // ===================
    // ===== HELPERS =====
    // ===================

    /// <summary>Create a technician user.</summary>
    protected async Task<Guid> CreateTechnicianAsync(
        string? email = null,
        bool isActive = true)
        => await Sender.Send(new CreateTechnicianCommand(
            Email: email ?? $"technician_{Faker.Random.AlphaNumeric(6)}@omnipulse.com",
            Password: Faker.Internet.Password(length: 50, regexPattern: @"[a-zA-Z0-9!@#$%^&*()_+=\-]"),
            FirstName: Faker.Name.FirstName(),
            LastName: Faker.Name.LastName(),
            HireDate: DateTime.UtcNow.AddYears(-1),
            IsActive: isActive
        ));

    /// <summary>Create an inventory item.</summary>
    protected async Task<int> CreateInventoryItemAsync(
        string? itemNumber = null,
        string? itemName = null,
        decimal unitCost = 25.50m,
        InventoryItemCategoryEnum category = InventoryItemCategoryEnum.ENGINE,
        InventoryItemUnitCostMeasurementUnitEnum unitCostUnit = InventoryItemUnitCostMeasurementUnitEnum.Unit,
        bool isActive = true)
        => await Sender.Send(new CreateInventoryItemCommand(
            ItemNumber: itemNumber ?? $"ITEM-{Faker.Random.AlphaNumeric(5)}",
            ItemName: itemName ?? $"Test {nameof(InventoryItem)} {Faker.Random.AlphaNumeric(5)}",
            Description: $"{nameof(InventoryItem)} Description {Faker.Random.AlphaNumeric(5)}",
            Category: category,
            Manufacturer: "Test Manufacturer",
            ManufacturerPartNumber: $"MPN{Faker.Random.AlphaNumeric(5)}",
            UniversalProductCode: "123456789012",
            UnitCost: unitCost,
            UnitCostMeasurementUnit: unitCostUnit,
            Supplier: "Test Supplier",
            WeightKG: 1.0,
            IsActive: isActive
        ));

    /// <summary>Create an inventory item location.</summary>
    protected async Task<int> CreateInventoryItemLocationAsync(
        string? locationName = null,
        int capacity = 10000)
        => await Sender.Send(new CreateInventoryItemLocationCommand(
            LocationName: locationName ?? $"{nameof(InventoryItemLocation)} Name {Faker.Random.AlphaNumeric(4)}",
            Address: "123 Test St",
            Longitude: 151.0,
            Latitude: -33.0,
            Capacity: capacity
        ));

    /// <summary>
    /// Ensure there is an inventory row for the given item and location with the specified quantity and unit cost.
    /// Creates the row if missing or updates existing ones.
    /// </summary>
    /// <param name="inventoryItemId">The ID of the inventory item.</param>
    /// <param name="inventoryItemLocationId">The ID of the inventory item location.</param>
    /// <param name="quantityOnHand">The quantity on hand.</param>
    /// <param name="unitCost">The unit cost.</param>
    /// <returns>The inventory ID.</returns>
    protected async Task<int> EnsureInventoryAsync(int inventoryItemId, int inventoryItemLocationId, int quantityOnHand, decimal unitCost = 25.00m)
    {
        var existingForItem = await DbContext.Inventories.Where(i => i.InventoryItemID == inventoryItemId).ToListAsync();
        if (existingForItem.Count != 0)
        {
            foreach (var inv in existingForItem)
            {
                inv.QuantityOnHand = quantityOnHand;
                inv.UnitCost = unitCost;
            }
            await DbContext.SaveChangesAsync();
            return existingForItem.First().ID;
        }

        var invItem = await DbContext.InventoryItems.FirstAsync(ii => ii.ID == inventoryItemId);
        var inventoryEntity = Inventory.CreateDefaultInventory(inventoryItemId, unitCost);
        inventoryEntity.InventoryItemLocationID = inventoryItemLocationId;
        inventoryEntity.QuantityOnHand = quantityOnHand;
        inventoryEntity.MinStockLevel = 0;
        inventoryEntity.MaxStockLevel = 10000;
        inventoryEntity.InventoryItem = invItem;

        DbContext.Inventories.Add(inventoryEntity);
        await DbContext.SaveChangesAsync();
        return inventoryEntity.ID;
    }

    /// <summary>Create a vehicle group.</summary>
    protected async Task<int> CreateVehicleGroupAsync()
        => await Sender.Send(new CreateVehicleGroupCommand(
            Name: $"Vehicle Group Name {Faker.Random.AlphaNumeric(5)}",
            Description: $"Vehicle Group Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true));

    /// <summary>Create a vehicle under the given vehicle group.</summary>
    protected async Task<int> CreateVehicleAsync(int vehicleGroupId, double mileage = 10000.0)
        => await Sender.Send(new CreateVehicleCommand(
            Name: $"Vehicle Name {Faker.Random.AlphaNumeric(5)}",
            Make: Faker.Vehicle.Manufacturer(),
            Model: Faker.Vehicle.Model(),
            Year: 2004,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.BUS,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "Base",
            Mileage: mileage,
            FuelCapacity: 50.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: 20000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Test Yard"));

    /// <summary>Create a service program.</summary>
    protected async Task<int> CreateServiceProgramAsync(bool isActive = true)
        => await Sender.Send(new CreateServiceProgramCommand(
            Name: $"Service Program Name {Faker.Random.AlphaNumeric(5)}",
            Description: $"Service Program Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: isActive));

    /// <summary>Create a service task.</summary>
    protected async Task<int> CreateServiceTaskAsync(
        double estimatedHours = 1.0,
        decimal estimatedCost = 10m,
        ServiceTaskCategoryEnum category = ServiceTaskCategoryEnum.PREVENTIVE,
        bool isActive = true)
        => await Sender.Send(new CreateServiceTaskCommand(
            Name: $"Service Task Name {Faker.Random.AlphaNumeric(5)}",
            Description: $"Service Task Description {Faker.Random.AlphaNumeric(5)}",
            EstimatedLabourHours: estimatedHours,
            EstimatedCost: estimatedCost,
            Category: category,
            IsActive: isActive));

    /// <summary>Add a vehicle to a service program.</summary>
    protected async Task AddVehicleToServiceProgramAsync(int serviceProgramId, int vehicleId)
        => await Sender.Send(new AddVehicleToServiceProgramCommand(ServiceProgramID: serviceProgramId, VehicleID: vehicleId));

    /// <summary>Create a time-based service schedule.</summary>
    protected async Task<int> CreateTimeBasedServiceScheduleAsync(
        int serviceProgramId,
        List<int> serviceTaskIds,
        int intervalValue,
        TimeUnitEnum intervalUnit,
        int bufferValue,
        TimeUnitEnum bufferUnit,
        DateTime? firstServiceDate,
        string? name = null)
        => await Sender.Send(new CreateServiceScheduleCommand(
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
            FirstServiceMileage: null));

    /// <summary>Create a mileage-based service schedule.</summary>
    protected async Task<int> CreateMileageBasedServiceScheduleAsync(
        int serviceProgramId,
        List<int> serviceTaskIds,
        int mileageInterval,
        int mileageBuffer,
        int? firstServiceMileage,
        string? name = null)
        => await Sender.Send(new CreateServiceScheduleCommand(
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
            FirstServiceMileage: firstServiceMileage));

    /// <summary>Trigger reminder sync.</summary>
    protected async Task<SyncServiceRemindersResponse> SyncServiceRemindersAsync()
        => await Sender.Send(new SyncServiceRemindersCommand());

    /// <summary>Get reminders for a specific (vehicle, schedule) pair via query.</summary>
    protected async Task<List<ServiceReminderDTO>> GetRemindersForScheduleVehiclePairAsync(int vehicleId, int scheduleId, int pageSize = 100)
    {
        var page = await Sender.Send(new GetAllServiceRemindersQuery(new PaginationParameters { PageNumber = 1, PageSize = pageSize }));
        return [.. page.Items.Where(r => r.VehicleID == vehicleId && r.ServiceScheduleID == scheduleId)];
    }

    /// <summary>Direct DB lookup of reminders for a schedule (bypassing CQRS).</summary>
    protected async Task<List<ServiceReminder>> GetRemindersInDbByScheduleAsync(int scheduleId)
        => await DbContext.ServiceReminders.Where(r => r.ServiceScheduleID == scheduleId).ToListAsync();
}