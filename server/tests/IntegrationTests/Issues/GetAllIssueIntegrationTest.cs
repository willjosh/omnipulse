using Application.Features.Issues.Command.CreateIssue;
using Application.Features.Issues.Query.GetAllIssue;
using Application.Features.Users.Command.CreateTechnician;
using Application.Features.VehicleGroups.Command.CreateVehicleGroup;
using Application.Features.Vehicles.Command.CreateVehicle;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.Issues;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(Issue))]
public class GetAllIssueIntegrationTest : BaseIntegrationTest
{
    public GetAllIssueIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnPagedIssues_When_QueryIsValid()
    {
        // Arrange - Create dependencies
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        // Create multiple issues
        var issue1Id = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: "Engine Problem",
            description: "Engine is making strange noises",
            priority: PriorityLevelEnum.HIGH,
            category: IssueCategoryEnum.ENGINE
        );

        var issue2Id = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: "Brake Issue",
            description: "Brakes are squeaking",
            priority: PriorityLevelEnum.MEDIUM,
            category: IssueCategoryEnum.BRAKES
        );

        var issue3Id = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: "Tire Replacement",
            description: "Front tire needs replacement",
            priority: PriorityLevelEnum.LOW,
            category: IssueCategoryEnum.TIRES
        );

        var query = new GetAllIssueQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            SortBy = "title",
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

        // Verify the created issues are in the result
        var createdIssues = result.Items.Where(x =>
            x.ID == issue1Id || x.ID == issue2Id || x.ID == issue3Id).ToList();

        createdIssues.Should().HaveCount(3);

        // Verify Engine Problem issue
        var engineIssue = createdIssues.First(x => x.ID == issue1Id);
        engineIssue.Title.Should().Be("Engine Problem");
        engineIssue.Description.Should().Be("Engine is making strange noises");
        engineIssue.PriorityLevel.Should().Be(PriorityLevelEnum.HIGH);
        engineIssue.Category.Should().Be(IssueCategoryEnum.ENGINE);

        // Verify Brake Issue
        var brakeIssue = createdIssues.First(x => x.ID == issue2Id);
        brakeIssue.Title.Should().Be("Brake Issue");
        brakeIssue.Category.Should().Be(IssueCategoryEnum.BRAKES);
    }

    [Fact]
    public async Task Should_ReturnEmptyResult_When_NoIssuesExist()
    {
        // Arrange - Clear existing issues
        var existingIssues = await DbContext.Issues.ToListAsync();
        DbContext.Issues.RemoveRange(existingIssues);
        await DbContext.SaveChangesAsync();

        var query = new GetAllIssueQuery(new PaginationParameters
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
        // Arrange - Create dependencies
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        // Create 8 issues
        var issueIds = new List<int>();
        for (int i = 1; i <= 8; i++)
        {
            var issueId = await CreateIssueAsync(
                vehicleId: vehicleId,
                technicianId: technicianId,
                title: $"Issue {i:D2}",
                description: $"Description for issue {i}",
                priority: PriorityLevelEnum.MEDIUM
            );
            issueIds.Add(issueId);
        }

        // Act - Get first page (3 issues per page)
        var firstPageQuery = new GetAllIssueQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 3,
            SortBy = "title",
            SortDescending = false
        });

        var firstPageResult = await Sender.Send(firstPageQuery);

        // Act - Get second page
        var secondPageQuery = new GetAllIssueQuery(new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 3,
            SortBy = "title",
            SortDescending = false
        });

        var secondPageResult = await Sender.Send(secondPageQuery);

        // Assert First Page
        firstPageResult.Should().NotBeNull();
        firstPageResult.Items.Should().HaveCount(3);
        firstPageResult.PageNumber.Should().Be(1);
        firstPageResult.PageSize.Should().Be(3);
        firstPageResult.TotalCount.Should().BeGreaterThanOrEqualTo(8);
        firstPageResult.HasPreviousPage.Should().BeFalse();
        firstPageResult.HasNextPage.Should().BeTrue();

        // Assert Second Page
        secondPageResult.Should().NotBeNull();
        secondPageResult.Items.Should().HaveCount(3);
        secondPageResult.PageNumber.Should().Be(2);
        secondPageResult.PageSize.Should().Be(3);
        secondPageResult.HasPreviousPage.Should().BeTrue();

        // Verify no duplicate issues between pages
        var firstPageIds = firstPageResult.Items.Select(x => x.ID).ToList();
        var secondPageIds = secondPageResult.Items.Select(x => x.ID).ToList();
        firstPageIds.Should().NotIntersectWith(secondPageIds);
    }

    [Fact]
    public async Task Should_FilterBySearch_When_SearchParameterProvided()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        var engineIssueId = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: "Engine Overheating",
            description: "Engine temperature too high"
        );

        var brakeIssueId = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: "Brake Failure",
            description: "Brake pedal not working"
        );

        // Act - Search for "Engine"
        var engineSearchQuery = new GetAllIssueQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "Engine"
        });

        var engineSearchResult = await Sender.Send(engineSearchQuery);

        // Assert Engine Search
        engineSearchResult.Should().NotBeNull();
        engineSearchResult.Items.Should().Contain(x => x.ID == engineIssueId);
        engineSearchResult.Items.Should().NotContain(x => x.ID == brakeIssueId);
    }

    [Theory]
    [InlineData(PriorityLevelEnum.CRITICAL)]
    [InlineData(PriorityLevelEnum.HIGH)]
    [InlineData(PriorityLevelEnum.MEDIUM)]
    [InlineData(PriorityLevelEnum.LOW)]
    public async Task Should_HandleDifferentPriorityLevels_When_IssuesHaveVariousPriorities(PriorityLevelEnum priority)
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        var issueId = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: $"{priority} Priority Issue",
            priority: priority
        );

        var query = new GetAllIssueQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var testIssue = result.Items.First(x => x.ID == issueId);
        testIssue.PriorityLevel.Should().Be(priority);
        testIssue.Title.Should().Be($"{priority} Priority Issue");
    }

    [Theory]
    [InlineData(IssueCategoryEnum.ENGINE)]
    [InlineData(IssueCategoryEnum.BRAKES)]
    [InlineData(IssueCategoryEnum.TIRES)]
    [InlineData(IssueCategoryEnum.ELECTRICAL)]
    [InlineData(IssueCategoryEnum.BODY)]
    public async Task Should_HandleDifferentCategories_When_IssuesHaveVariousCategories(IssueCategoryEnum category)
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        var issueId = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: $"{category} Issue",
            category: category
        );

        var query = new GetAllIssueQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var testIssue = result.Items.First(x => x.ID == issueId);
        testIssue.Category.Should().Be(category);
        testIssue.Title.Should().Be($"{category} Issue");
    }

    [Fact]
    public async Task Should_IncludeAllRequiredProperties_When_ReturningIssues()
    {
        // Arrange
        var vehicleGroupId = await CreateVehicleGroupAsync();
        var vehicleId = await CreateVehicleAsync(vehicleGroupId);
        var technicianId = await CreateTechnicianAsync();

        var issueId = await CreateIssueAsync(
            vehicleId: vehicleId,
            technicianId: technicianId,
            title: "Complete Properties Test",
            description: "Testing all properties"
        );

        var query = new GetAllIssueQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        var testIssue = result.Items.First(x => x.ID == issueId);

        // Verify all required properties are populated
        testIssue.ID.Should().Be(issueId);
        testIssue.VehicleID.Should().Be(vehicleId);
        testIssue.Title.Should().Be("Complete Properties Test");
        testIssue.Description.Should().Be("Testing all properties");
        testIssue.PriorityLevel.Should().BeDefined();
        testIssue.Category.Should().BeDefined();
        testIssue.Status.Should().BeDefined();
        testIssue.ReportedByUserID.Should().NotBeNullOrEmpty();
        testIssue.ReportedDate.Should().BeAfter(DateTime.MinValue);
    }

    // Helper methods
    private async Task<int> CreateIssueAsync(
        int vehicleId,
        Guid technicianId,
        string? title = null,
        string? description = null,
        PriorityLevelEnum priority = PriorityLevelEnum.MEDIUM,
        IssueCategoryEnum category = IssueCategoryEnum.ENGINE,
        IssueStatusEnum status = IssueStatusEnum.OPEN)
    {
        var createCommand = new CreateIssueCommand(
            VehicleID: vehicleId,
            Title: title ?? $"Test Issue {Faker.Random.AlphaNumeric(5)}",
            Description: description ?? $"Test description {Faker.Random.AlphaNumeric(8)}",
            PriorityLevel: priority,
            Category: category,
            Status: status,
            ReportedByUserID: technicianId.ToString(),
            ReportedDate: DateTime.UtcNow
        );

        return await Sender.Send(createCommand);
    }

    private async Task<int> CreateVehicleAsync(int vehicleGroupId)
    {
        var createCommand = new CreateVehicleCommand(
            Name: $"Test Vehicle {Faker.Random.AlphaNumeric(5)}",
            Make: Faker.Vehicle.Manufacturer(),
            Model: Faker.Vehicle.Model(),
            Year: 2020,
            VIN: Faker.Vehicle.Vin(),
            LicensePlate: Faker.Random.Replace("???-###"),
            LicensePlateExpirationDate: DateTime.Today.AddYears(1),
            VehicleType: VehicleTypeEnum.CAR,
            VehicleGroupID: vehicleGroupId,
            AssignedTechnicianID: null,
            Trim: "Base",
            Mileage: 10000.0,
            FuelCapacity: 50.0,
            FuelType: FuelTypeEnum.PETROL,
            PurchaseDate: DateTime.Today.AddYears(-1),
            PurchasePrice: 25000.00m,
            Status: VehicleStatusEnum.ACTIVE,
            Location: "Test Location"
        );

        return await Sender.Send(createCommand);
    }

    private async Task<int> CreateVehicleGroupAsync()
    {
        var createCommand = new CreateVehicleGroupCommand(
            Name: $"Test Vehicle Group {Faker.Random.AlphaNumeric(5)}",
            Description: $"Test description {Faker.Random.AlphaNumeric(8)}",
            IsActive: true
        );

        return await Sender.Send(createCommand);
    }

    private async Task<Guid> CreateTechnicianAsync()
    {
        var createCommand = new CreateTechnicianCommand(
            Email: $"tech{Faker.Random.AlphaNumeric(5)}@test.com",
            Password: "TestPassword123!",
            FirstName: Faker.Name.FirstName(),
            LastName: Faker.Name.LastName(),
            HireDate: DateTime.UtcNow.AddYears(-1),
            IsActive: true
        );

        return await Sender.Send(createCommand);
    }
}