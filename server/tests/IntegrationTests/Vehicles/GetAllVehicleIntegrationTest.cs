using Application.Features.Vehicles.Query.GetAllVehicle;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Vehicles;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(Vehicle))]
public class GetAllVehicleIntegrationTest : BaseIntegrationTest
{
    public GetAllVehicleIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnPagedVehicles_When_QueryIsValid()
    {
        // Arrange - Create vehicle group dependency
        var vehicleGroupId = await CreateVehicleGroupAsync();

        // Create multiple vehicles with different properties
        var vehicleId1 = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Toyota Camry",
            make: "Toyota",
            model: "Camry",
            year: 2020,
            vehicleType: VehicleTypeEnum.CAR,
            mileage: 15000.0,
            purchasePrice: 25000.00m
        );

        var vehicleId2 = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Ford Transit",
            make: "Ford",
            model: "Transit",
            year: 2021,
            vehicleType: VehicleTypeEnum.VAN,
            mileage: 8000.0,
            purchasePrice: 35000.00m
        );

        var vehicleId3 = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Freightliner Cascadia",
            make: "Freightliner",
            model: "Cascadia",
            year: 2019,
            vehicleType: VehicleTypeEnum.TRUCK,
            mileage: 120000.0,
            purchasePrice: 85000.00m
        );

        var query = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = "name",
            SortDescending = false
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThanOrEqualTo(3);
        result.TotalCount.Should().BeGreaterThanOrEqualTo(3);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);

        // Verify the created vehicles are in the result
        var createdVehicles = result.Items.Where(x =>
            x.ID == vehicleId1 || x.ID == vehicleId2 || x.ID == vehicleId3).ToList();

        createdVehicles.Should().HaveCount(3);

        // Verify Toyota Camry
        var camry = createdVehicles.First(x => x.ID == vehicleId1);
        camry.Name.Should().Be("Toyota Camry");
        camry.Make.Should().Be("Toyota");
        camry.Model.Should().Be("Camry");
        camry.Year.Should().Be(2020);
        camry.VehicleType.Should().Be(VehicleTypeEnum.CAR);
        camry.Mileage.Should().Be(15000.0);
        camry.PurchasePrice.Should().Be(25000.00m);

        // Verify Ford Transit
        var transit = createdVehicles.First(x => x.ID == vehicleId2);
        transit.Name.Should().Be("Ford Transit");
        transit.Make.Should().Be("Ford");
        transit.Model.Should().Be("Transit");
        transit.VehicleType.Should().Be(VehicleTypeEnum.VAN);
    }

    [Fact]
    public async Task Should_ReturnEmptyResult_When_NoVehiclesExist()
    {
        // Arrange - Clear existing vehicles
        var existingVehicles = await DbContext.Vehicles.ToListAsync();
        DbContext.Vehicles.RemoveRange(existingVehicles);
        await DbContext.SaveChangesAsync();

        var query = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().BeEmpty();
        result.TotalCount.Should().Be(0);
        result.PageNumber.Should().Be(1);
        result.PageSize.Should().Be(10);
        result.TotalPages.Should().Be(0);
        result.HasPreviousPage.Should().BeFalse();
        result.HasNextPage.Should().BeFalse();
    }

    [Fact]
    public async Task Should_HandlePagination_When_MultiplePages()
    {
        // Arrange - Create vehicle group
        var vehicleGroupId = await CreateVehicleGroupAsync();

        // Create 12 vehicles
        var vehicleIds = new List<int>();
        for (int i = 1; i <= 12; i++)
        {
            var vehicleId = await CreateVehicleAsync(
                vehicleGroupId: vehicleGroupId,
                name: $"Vehicle {i:D2}",
                make: "Honda",
                model: "Civic",
                year: 2020 + (i % 3),
                mileage: i * 5000.0,
                purchasePrice: 20000.00m + (i * 1000)
            );
            vehicleIds.Add(vehicleId);
        }

        // Act - Get first page (5 vehicles per page)
        var firstPageQuery = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            SortBy = "name",
            SortDescending = false
        });

        var firstPageResult = await Sender.Send(firstPageQuery);

        // Act - Get second page
        var secondPageQuery = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 5,
            SortBy = "name",
            SortDescending = false
        });

        var secondPageResult = await Sender.Send(secondPageQuery);

        // Assert First Page
        firstPageResult.Should().NotBeNull();
        firstPageResult.Items.Should().HaveCount(5);
        firstPageResult.PageNumber.Should().Be(1);
        firstPageResult.PageSize.Should().Be(5);
        firstPageResult.TotalCount.Should().BeGreaterThanOrEqualTo(12);
        firstPageResult.HasPreviousPage.Should().BeFalse();
        firstPageResult.HasNextPage.Should().BeTrue();

        // Assert Second Page
        secondPageResult.Should().NotBeNull();
        secondPageResult.Items.Should().HaveCount(5);
        secondPageResult.PageNumber.Should().Be(2);
        secondPageResult.PageSize.Should().Be(5);
        secondPageResult.HasPreviousPage.Should().BeTrue();

        // Verify no duplicate vehicles between pages
        var firstPageIds = firstPageResult.Items.Select(x => x.ID).ToList();
        var secondPageIds = secondPageResult.Items.Select(x => x.ID).ToList();
        firstPageIds.Should().NotIntersectWith(secondPageIds);
    }

    [Fact]
    public async Task Should_FilterBySearch_When_SearchParameterProvided()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();

        var toyotaId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Toyota Prius",
            make: "Toyota",
            model: "Prius"
        );

        var fordId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Ford F-150",
            make: "Ford",
            model: "F-150"
        );

        var chevroletId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Chevrolet Silverado",
            make: "Chevrolet",
            model: "Silverado"
        );

        // Act - Search for "Toyota"
        var toyotaSearchQuery = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "Toyota"
        });

        var toyotaSearchResult = await Sender.Send(toyotaSearchQuery);

        // Act - Search for "Ford"
        var fordSearchQuery = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "Ford"
        });

        var fordSearchResult = await Sender.Send(fordSearchQuery);

        // Assert Toyota Search
        toyotaSearchResult.Should().NotBeNull();
        toyotaSearchResult.Items.Should().Contain(x => x.ID == toyotaId);
        toyotaSearchResult.Items.Should().NotContain(x => x.ID == fordId || x.ID == chevroletId);

        // Assert Ford Search
        fordSearchResult.Should().NotBeNull();
        fordSearchResult.Items.Should().Contain(x => x.ID == fordId);
        fordSearchResult.Items.Should().NotContain(x => x.ID == toyotaId || x.ID == chevroletId);
    }

    [Theory]
    [InlineData("name", false)]
    [InlineData("name", true)]
    [InlineData("make", false)]
    [InlineData("make", true)]
    [InlineData("year", false)]
    [InlineData("year", true)]
    [InlineData("mileage", false)]
    [InlineData("mileage", true)]
    [InlineData("purchaseprice", false)]
    [InlineData("purchaseprice", true)]
    public async Task Should_SortCorrectly_When_SortParametersProvided(string sortBy, bool sortDescending)
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();

        await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Alpha Vehicle",
            make: "Audi",
            year: 2020,
            mileage: 30000.0,
            purchasePrice: 40000.00m
        );

        await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Beta Vehicle",
            make: "BMW",
            year: 2021,
            mileage: 20000.0,
            purchasePrice: 50000.00m
        );

        await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Charlie Vehicle",
            make: "Cadillac",
            year: 2019,
            mileage: 40000.0,
            purchasePrice: 45000.00m
        );

        var query = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = sortBy,
            SortDescending = sortDescending
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCountGreaterThanOrEqualTo(3);

        // Verify sorting based on the sort field
        var sortedVehicles = result.Items.Where(x =>
            x.Name.Contains("Alpha") || x.Name.Contains("Beta") || x.Name.Contains("Charlie")).ToList();
        sortedVehicles.Should().HaveCount(3);

        switch (sortBy.ToLowerInvariant())
        {
            case "name":
                if (sortDescending)
                {
                    sortedVehicles.Should().BeInDescendingOrder(x => x.Name);
                }
                else
                {
                    sortedVehicles.Should().BeInAscendingOrder(x => x.Name);
                }
                break;

            case "make":
                if (sortDescending)
                {
                    sortedVehicles.Should().BeInDescendingOrder(x => x.Make);
                }
                else
                {
                    sortedVehicles.Should().BeInAscendingOrder(x => x.Make);
                }
                break;

            case "year":
                if (sortDescending)
                {
                    sortedVehicles.Should().BeInDescendingOrder(x => x.Year);
                }
                else
                {
                    sortedVehicles.Should().BeInAscendingOrder(x => x.Year);
                }
                break;

            case "mileage":
                if (sortDescending)
                {
                    sortedVehicles.Should().BeInDescendingOrder(x => x.Mileage);
                }
                else
                {
                    sortedVehicles.Should().BeInAscendingOrder(x => x.Mileage);
                }
                break;

            case "purchaseprice":
                if (sortDescending)
                {
                    sortedVehicles.Should().BeInDescendingOrder(x => x.PurchasePrice);
                }
                else
                {
                    sortedVehicles.Should().BeInAscendingOrder(x => x.PurchasePrice);
                }
                break;
        }
    }

    [Fact]
    public async Task Should_HandleDifferentVehicleTypes_When_VehiclesHaveVariousTypes()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();

        var carId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Car Vehicle",
            vehicleType: VehicleTypeEnum.CAR
        );

        var truckId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Truck Vehicle",
            vehicleType: VehicleTypeEnum.TRUCK
        );

        var vanId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Van Vehicle",
            vehicleType: VehicleTypeEnum.VAN
        );

        var busId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Bus Vehicle",
            vehicleType: VehicleTypeEnum.BUS
        );

        var motorcycleId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Motorcycle Vehicle",
            vehicleType: VehicleTypeEnum.MOTORCYCLE
        );

        var query = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdVehicles = result.Items.Where(x =>
            x.ID == carId || x.ID == truckId || x.ID == vanId ||
            x.ID == busId || x.ID == motorcycleId).ToList();

        createdVehicles.Should().HaveCount(5);

        // Verify each vehicle type exists
        createdVehicles.Should().Contain(x => x.VehicleType == VehicleTypeEnum.CAR);
        createdVehicles.Should().Contain(x => x.VehicleType == VehicleTypeEnum.TRUCK);
        createdVehicles.Should().Contain(x => x.VehicleType == VehicleTypeEnum.VAN);
        createdVehicles.Should().Contain(x => x.VehicleType == VehicleTypeEnum.BUS);
        createdVehicles.Should().Contain(x => x.VehicleType == VehicleTypeEnum.MOTORCYCLE);
    }

    [Fact]
    public async Task Should_HandleDifferentVehicleStatuses_When_VehiclesHaveVariousStatuses()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();

        var activeId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Active Vehicle",
            status: VehicleStatusEnum.ACTIVE
        );

        var maintenanceId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Maintenance Vehicle",
            status: VehicleStatusEnum.MAINTENANCE
        );

        var outOfServiceId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Out of Service Vehicle",
            status: VehicleStatusEnum.OUT_OF_SERVICE
        );

        var query = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdVehicles = result.Items.Where(x =>
            x.ID == activeId || x.ID == maintenanceId || x.ID == outOfServiceId).ToList();

        createdVehicles.Should().HaveCount(3);

        // Verify each status exists
        createdVehicles.Should().Contain(x => x.Status == VehicleStatusEnum.ACTIVE);
        createdVehicles.Should().Contain(x => x.Status == VehicleStatusEnum.MAINTENANCE);
        createdVehicles.Should().Contain(x => x.Status == VehicleStatusEnum.OUT_OF_SERVICE);
    }

    [Fact]
    public async Task Should_HandleDifferentFuelTypes_When_VehiclesHaveVariousFuelTypes()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();

        var petrolId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Petrol Vehicle",
            fuelType: FuelTypeEnum.PETROL
        );

        var dieselId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Diesel Vehicle",
            fuelType: FuelTypeEnum.DIESEL
        );

        var electricId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Electric Vehicle",
            fuelType: FuelTypeEnum.ELECTRIC
        );

        var hybridId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Hybrid Vehicle",
            fuelType: FuelTypeEnum.HYBRID
        );

        var query = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdVehicles = result.Items.Where(x =>
            x.ID == petrolId || x.ID == dieselId ||
            x.ID == electricId || x.ID == hybridId).ToList();

        createdVehicles.Should().HaveCount(4);

        // Verify each fuel type exists
        createdVehicles.Should().Contain(x => x.FuelType == FuelTypeEnum.PETROL);
        createdVehicles.Should().Contain(x => x.FuelType == FuelTypeEnum.DIESEL);
        createdVehicles.Should().Contain(x => x.FuelType == FuelTypeEnum.ELECTRIC);
        createdVehicles.Should().Contain(x => x.FuelType == FuelTypeEnum.HYBRID);
    }

    [Fact]
    public async Task Should_IncludeAllRequiredProperties_When_ReturningVehicles()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();

        var vehicleId = await CreateVehicleAsync(
            vehicleGroupId: vehicleGroupId,
            name: "Complete Vehicle Test",
            make: "Toyota",
            model: "Camry",
            year: 2022
        );

        var query = new GetAllVehicleQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        var testVehicle = result.Items.First(x => x.ID == vehicleId);

        // Verify all required properties are populated
        testVehicle.ID.Should().Be(vehicleId);
        testVehicle.Name.Should().NotBeNullOrEmpty();
        testVehicle.Make.Should().NotBeNullOrEmpty();
        testVehicle.Model.Should().NotBeNullOrEmpty();
        testVehicle.Year.Should().BeGreaterThan(1900);
        testVehicle.VIN.Should().NotBeNullOrEmpty();
        testVehicle.LicensePlate.Should().NotBeNullOrEmpty();
        testVehicle.LicensePlateExpirationDate.Should().BeAfter(DateTime.MinValue);
        testVehicle.VehicleType.Should().BeDefined();
        testVehicle.VehicleGroupID.Should().Be(vehicleGroupId);
        testVehicle.Trim.Should().NotBeNullOrEmpty();
        testVehicle.FuelCapacity.Should().BeGreaterThan(0);
        testVehicle.FuelType.Should().BeDefined();
        testVehicle.PurchaseDate.Should().BeAfter(DateTime.MinValue);
        testVehicle.PurchasePrice.Should().BeGreaterThan(0);
        testVehicle.Status.Should().BeDefined();
        testVehicle.Location.Should().NotBeNullOrEmpty();
    }

    // Helper method to create vehicles with specific properties
    private async Task<int> CreateVehicleAsync(
        int vehicleGroupId,
        string? name = null,
        string? make = null,
        string? model = null,
        int? year = null,
        VehicleTypeEnum? vehicleType = null,
        double? mileage = null,
        decimal? purchasePrice = null,
        VehicleStatusEnum? status = null,
        FuelTypeEnum? fuelType = null)
    {
        var createVehicleCommand = new Application.Features.Vehicles.Command.CreateVehicle.CreateVehicleCommand(
            Name: name ?? $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: make ?? Faker.Vehicle.Manufacturer(),
            Model: model ?? Faker.Vehicle.Model(),
            Year: year ?? 2020,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: vehicleType ?? VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "Base",
            Mileage: mileage ?? 10000.0,
            FuelCapacity: 50.0,
            FuelType: fuelType ?? FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: purchasePrice ?? 25000.00m,
            Status: status ?? VehicleStatusEnum.ACTIVE,
            Location: "Test Location"
        );

        return await Sender.Send(createVehicleCommand);
    }
}