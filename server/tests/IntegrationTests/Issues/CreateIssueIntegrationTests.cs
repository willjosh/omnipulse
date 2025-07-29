using Application.Features.Issues.Command.CreateIssue;
using Application.Features.Users.Command.CreateTechnician;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.Issues;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(Issue))]
public class CreateIssueIntegrationTests : BaseIntegrationTest
{
    public CreateIssueIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddIssueToDatabase_When_CommandIsValid()
    {
        // Arrange - Dependencies
        var createVehicleGroupCommand = new CreateVehicleGroupCommand(
            Name: $"[Dependency] {nameof(VehicleGroup)} - {nameof(Should_AddIssueToDatabase_When_CommandIsValid)}",
            Description: $"[Dependency] {nameof(VehicleGroup)} - {nameof(Should_AddIssueToDatabase_When_CommandIsValid)} {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );
        int vehicleGroupId = await Sender.Send(createVehicleGroupCommand);

        var createVehicleCommand = new CreateVehicleCommand(
            Name: $"[Dependency] {nameof(Vehicle)} - {nameof(Should_AddIssueToDatabase_When_CommandIsValid)}",
            Make: Faker.Vehicle.Manufacturer(),
            Model: Faker.Vehicle.Model(),
            Year: 2024,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: $"ABC{Faker.Random.AlphaNumeric(3)}",
            LicensePlateExpirationDate: DateTime.UtcNow.AddYears(1),
            VehicleType: VehicleTypeEnum.VAN,
            VehicleGroupID: vehicleGroupId,
            Trim: "Base",
            Mileage: 0,
            EngineHours: 0,
            FuelCapacity: 70.0,
            FuelType: FuelTypeEnum.DIESEL,
            PurchaseDate: DateTime.UtcNow.AddDays(-1),
            PurchasePrice: 45000m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Main Depot",
            AssignedTechnicianID: null
        );
        int vehicleId = await Sender.Send(createVehicleCommand);

        var createTechnicianCommand = new CreateTechnicianCommand(
            Email: $"technician{Faker.Random.AlphaNumeric(5)}@omnipulse.com",
            Password: Faker.Internet.Password(length: 50, regexPattern: @"[a-zA-Z0-9!@#$%^&*()_+=\-]"),
            FirstName: Faker.Name.FirstName(),
            LastName: Faker.Name.LastName(),
            HireDate: DateTime.UtcNow.AddYears(-1),
            IsActive: true
        );
        Guid technicianId = await Sender.Send(createTechnicianCommand);

        // Arrange
        var createCommand = new CreateIssueCommand(
            VehicleID: vehicleId,
            Title: $"Test {nameof(Issue)} Title {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test {nameof(Issue)} Description {Faker.Random.AlphaNumeric(10)}",
            PriorityLevel: PriorityLevelEnum.MEDIUM,
            Category: IssueCategoryEnum.ENGINE,
            Status: IssueStatusEnum.OPEN,
            ReportedByUserID: technicianId.ToString(),
            ReportedDate: DateTime.UtcNow
        );

        // Act
        int createdIssueId = await Sender.Send(createCommand);

        // Assert
        Issue? createdIssueEntity = await DbContext.Issues.FindAsync(createdIssueId);
        createdIssueEntity.Should().NotBeNull();

        createdIssueEntity.VehicleID.Should().Be(createCommand.VehicleID);
        createdIssueEntity.Title.Should().Be(createCommand.Title);
        createdIssueEntity.Description.Should().Be(createCommand.Description);
        createdIssueEntity.PriorityLevel.Should().Be(createCommand.PriorityLevel);
        createdIssueEntity.Category.Should().Be(createCommand.Category);
        createdIssueEntity.Status.Should().Be(createCommand.Status);
        createdIssueEntity.ReportedByUserID.Should().Be(createCommand.ReportedByUserID);
        createdIssueEntity.ReportedDate.Should().Be(createCommand.ReportedDate);
    }
}