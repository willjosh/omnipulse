using Application.Features.VehicleGroups.Query.GetAllVehicleGroup;
using Application.Models.PaginationModels;

using Domain.Entities;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.VehicleGroups;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(VehicleGroup))]
public class GetAllVehicleGroupIntegrationTest : BaseIntegrationTest
{
    public GetAllVehicleGroupIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnPagedVehicleGroups_When_QueryIsValid()
    {
        // Arrange - Create multiple vehicle groups with different properties
        var groupId1 = await CreateVehicleGroupAsync(
            name: "Fleet Group Alpha",
            description: "Main commercial fleet vehicles",
            isActive: true
        );

        var groupId2 = await CreateVehicleGroupAsync(
            name: "Emergency Vehicles",
            description: "Emergency response vehicles",
            isActive: true
        );

        var groupId3 = await CreateVehicleGroupAsync(
            name: "Maintenance Group",
            description: "Vehicles currently under maintenance",
            isActive: false
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
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

        // Verify the created groups are in the result
        var createdGroups = result.Items.Where(x =>
            x.ID == groupId1 || x.ID == groupId2 || x.ID == groupId3).ToList();

        createdGroups.Should().HaveCount(3);

        // Verify Fleet Group Alpha
        var fleetGroup = createdGroups.First(x => x.ID == groupId1);
        fleetGroup.Name.Should().Be("Fleet Group Alpha");
        fleetGroup.Description.Should().Be("Main commercial fleet vehicles");
        fleetGroup.IsActive.Should().BeTrue();

        // Verify Emergency Vehicles
        var emergencyGroup = createdGroups.First(x => x.ID == groupId2);
        emergencyGroup.Name.Should().Be("Emergency Vehicles");
        emergencyGroup.Description.Should().Be("Emergency response vehicles");
        emergencyGroup.IsActive.Should().BeTrue();

        // Verify Maintenance Group
        var maintenanceGroup = createdGroups.First(x => x.ID == groupId3);
        maintenanceGroup.Name.Should().Be("Maintenance Group");
        maintenanceGroup.Description.Should().Be("Vehicles currently under maintenance");
        maintenanceGroup.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnEmptyResult_When_NoVehicleGroupsExist()
    {
        // Arrange - Clear existing vehicle groups
        var existingGroups = await DbContext.VehicleGroups.ToListAsync();
        DbContext.VehicleGroups.RemoveRange(existingGroups);
        await DbContext.SaveChangesAsync();

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
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
        // Arrange - Create 12 vehicle groups
        var groupIds = new List<int>();
        for (int i = 1; i <= 12; i++)
        {
            var groupId = await CreateVehicleGroupAsync(
                name: $"Group {i:D2}",
                description: $"Description for group {i}",
                isActive: i % 2 == 0 // Alternate active/inactive
            );
            groupIds.Add(groupId);
        }

        // Act - Get first page (5 groups per page)
        var firstPageQuery = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 5,
            SortBy = "name",
            SortDescending = false
        });

        var firstPageResult = await Sender.Send(firstPageQuery);

        // Act - Get second page
        var secondPageQuery = new GetAllVehicleGroupQuery(new PaginationParameters
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

        // Verify no duplicate groups between pages
        var firstPageIds = firstPageResult.Items.Select(x => x.ID).ToList();
        var secondPageIds = secondPageResult.Items.Select(x => x.ID).ToList();
        firstPageIds.Should().NotIntersectWith(secondPageIds);
    }

    [Fact]
    public async Task Should_FilterBySearch_When_SearchParameterProvided()
    {
        // Arrange
        var fleetGroupId = await CreateVehicleGroupAsync(
            name: "Fleet Management",
            description: "Main fleet operations"
        );

        var emergencyGroupId = await CreateVehicleGroupAsync(
            name: "Emergency Response",
            description: "Emergency vehicles and equipment"
        );

        var maintenanceGroupId = await CreateVehicleGroupAsync(
            name: "Maintenance Department",
            description: "Vehicle maintenance and repair"
        );

        // Act - Search for "fleet"
        var fleetSearchQuery = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "fleet"
        });

        var fleetSearchResult = await Sender.Send(fleetSearchQuery);

        // Act - Search for "emergency"
        var emergencySearchQuery = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "emergency"
        });

        var emergencySearchResult = await Sender.Send(emergencySearchQuery);

        // Assert Fleet Search
        fleetSearchResult.Should().NotBeNull();
        fleetSearchResult.Items.Should().Contain(x => x.ID == fleetGroupId);
        fleetSearchResult.Items.Should().NotContain(x => x.ID == emergencyGroupId || x.ID == maintenanceGroupId);

        // Assert Emergency Search
        emergencySearchResult.Should().NotBeNull();
        emergencySearchResult.Items.Should().Contain(x => x.ID == emergencyGroupId);
        emergencySearchResult.Items.Should().NotContain(x => x.ID == fleetGroupId || x.ID == maintenanceGroupId);
    }

    [Theory]
    [InlineData("name", false)]
    [InlineData("name", true)]
    [InlineData("description", false)]
    [InlineData("description", true)]
    [InlineData("isactive", false)]
    [InlineData("isactive", true)]
    [InlineData("createdat", false)]
    [InlineData("createdat", true)]
    public async Task Should_SortCorrectly_When_SortParametersProvided(string sortBy, bool sortDescending)
    {
        // Arrange
        await CreateVehicleGroupAsync(
            name: "Alpha Group",
            description: "First group description",
            isActive: true
        );

        await CreateVehicleGroupAsync(
            name: "Beta Group",
            description: "Second group description",
            isActive: false
        );

        await CreateVehicleGroupAsync(
            name: "Charlie Group",
            description: "Third group description",
            isActive: true
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
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
        var sortedGroups = result.Items.Where(x =>
            x.Name.Contains("Alpha") || x.Name.Contains("Beta") || x.Name.Contains("Charlie")).ToList();
        sortedGroups.Should().HaveCount(3);

        switch (sortBy.ToLowerInvariant())
        {
            case "name":
                if (sortDescending)
                {
                    sortedGroups.Should().BeInDescendingOrder(x => x.Name);
                }
                else
                {
                    sortedGroups.Should().BeInAscendingOrder(x => x.Name);
                }
                break;

            case "description":
                if (sortDescending)
                {
                    sortedGroups.Should().BeInDescendingOrder(x => x.Description);
                }
                else
                {
                    sortedGroups.Should().BeInAscendingOrder(x => x.Description);
                }
                break;

            case "isactive":
                if (sortDescending)
                {
                    sortedGroups.Should().BeInDescendingOrder(x => x.IsActive);
                }
                else
                {
                    sortedGroups.Should().BeInAscendingOrder(x => x.IsActive);
                }
                break;

        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_HandleActiveAndInactiveGroups_When_IsActiveVaries(bool isActive)
    {
        // Arrange
        var groupId = await CreateVehicleGroupAsync(
            name: isActive ? "Active Test Group" : "Inactive Test Group",
            description: isActive ? "This group is active" : "This group is inactive",
            isActive: isActive
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var testGroup = result.Items.First(x => x.ID == groupId);
        testGroup.IsActive.Should().Be(isActive);
        testGroup.Name.Should().Be(isActive ? "Active Test Group" : "Inactive Test Group");
        testGroup.Description.Should().Be(isActive ? "This group is active" : "This group is inactive");
    }

    [Fact]
    public async Task Should_HandleMixedActiveStatus_When_GroupsHaveVariousActiveStates()
    {
        // Arrange
        var activeGroupId = await CreateVehicleGroupAsync(
            name: "Active Operations",
            description: "Currently active group",
            isActive: true
        );

        var inactiveGroupId = await CreateVehicleGroupAsync(
            name: "Inactive Legacy",
            description: "Legacy inactive group",
            isActive: false
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdGroups = result.Items.Where(x =>
            x.ID == activeGroupId || x.ID == inactiveGroupId).ToList();

        createdGroups.Should().HaveCount(2);

        var activeGroup = createdGroups.First(x => x.ID == activeGroupId);
        var inactiveGroup = createdGroups.First(x => x.ID == inactiveGroupId);

        activeGroup.IsActive.Should().BeTrue();
        activeGroup.Name.Should().Be("Active Operations");

        inactiveGroup.IsActive.Should().BeFalse();
        inactiveGroup.Name.Should().Be("Inactive Legacy");
    }

    [Fact]
    public async Task Should_HandleVariousDescriptionLengths_When_GroupsHaveDifferentDescriptions()
    {
        // Arrange
        var shortDescGroupId = await CreateVehicleGroupAsync(
            name: "Short Desc Group",
            description: "Short",
            isActive: true
        );

        var longDescGroupId = await CreateVehicleGroupAsync(
            name: "Long Desc Group",
            description: "This is a very long description that contains multiple sentences. It provides detailed information about the vehicle group and its purpose. This description is intentionally verbose to test handling of longer text content.",
            isActive: true
        );

        var noDescGroupId = await CreateVehicleGroupAsync(
            name: "No Desc Group",
            description: "",
            isActive: true
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdGroups = result.Items.Where(x =>
            x.ID == shortDescGroupId || x.ID == longDescGroupId || x.ID == noDescGroupId).ToList();

        createdGroups.Should().HaveCount(3);

        var shortDescGroup = createdGroups.First(x => x.ID == shortDescGroupId);
        var longDescGroup = createdGroups.First(x => x.ID == longDescGroupId);
        var noDescGroup = createdGroups.First(x => x.ID == noDescGroupId);

        shortDescGroup.Description.Should().Be("Short");
        longDescGroup.Description.Should().Contain("very long description");
        longDescGroup.Description.Length.Should().BeGreaterThan(100);
        noDescGroup.Description.Should().BeEmpty();
    }

    [Fact]
    public async Task Should_IncludeAllRequiredProperties_When_ReturningVehicleGroups()
    {
        // Arrange
        var groupId = await CreateVehicleGroupAsync(
            name: "Complete Properties Test",
            description: "Testing all properties",
            isActive: true
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        var testGroup = result.Items.First(x => x.ID == groupId);

        // Verify all required properties are populated
        testGroup.ID.Should().Be(groupId);
        testGroup.Name.Should().Be("Complete Properties Test");
        testGroup.Description.Should().Be("Testing all properties");
        testGroup.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Should_HandleSpecialCharactersInNames_When_GroupsHaveUnicodeContent()
    {
        // Arrange
        var specialCharsGroupId = await CreateVehicleGroupAsync(
            name: "Spëcîål Chàrs & Símböls!",
            description: "Group with spëcîål characters, émojis 🚗🚚, and numbers (123)",
            isActive: true
        );

        var numbersGroupId = await CreateVehicleGroupAsync(
            name: "Group-123_TEST",
            description: "Group with numbers and underscores",
            isActive: true
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdGroups = result.Items.Where(x =>
            x.ID == specialCharsGroupId || x.ID == numbersGroupId).ToList();

        createdGroups.Should().HaveCount(2);

        var specialCharsGroup = createdGroups.First(x => x.ID == specialCharsGroupId);
        var numbersGroup = createdGroups.First(x => x.ID == numbersGroupId);

        specialCharsGroup.Name.Should().Be("Spëcîål Chàrs & Símböls!");
        specialCharsGroup.Description.Should().Contain("spëcîål characters");

        numbersGroup.Name.Should().Be("Group-123_TEST");
        numbersGroup.Description.Should().Be("Group with numbers and underscores");
    }

    [Fact]
    public async Task Should_ReturnConsistentData_When_CalledMultipleTimes()
    {
        // Arrange
        var groupId = await CreateVehicleGroupAsync(
            name: "Consistency Test Group",
            description: "Testing data consistency",
            isActive: true
        );

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act - Call multiple times
        var result1 = await Sender.Send(query);
        var result2 = await Sender.Send(query);
        var result3 = await Sender.Send(query);

        // Assert - All results should be consistent
        var group1 = result1.Items.First(x => x.ID == groupId);
        var group2 = result2.Items.First(x => x.ID == groupId);
        var group3 = result3.Items.First(x => x.ID == groupId);

        group1.Should().BeEquivalentTo(group2);
        group2.Should().BeEquivalentTo(group3);
        group1.Should().BeEquivalentTo(group3);

        // Verify specific properties are consistent
        group1.ID.Should().Be(group2.ID).And.Be(group3.ID);
        group1.Name.Should().Be(group2.Name).And.Be(group3.Name);
        group1.IsActive.Should().Be(group2.IsActive).And.Be(group3.IsActive);
    }

    [Fact]
    public async Task Should_HandleLargeDataset_When_ManyGroupsExist()
    {
        // Arrange - Create 50 vehicle groups
        var groupIds = new List<int>();
        for (int i = 1; i <= 50; i++)
        {
            var groupId = await CreateVehicleGroupAsync(
                name: $"Large Dataset Group {i:D3}",
                description: $"Group number {i} in large dataset test",
                isActive: i % 3 != 0 // Most groups active, every 3rd inactive
            );
            groupIds.Add(groupId);
        }

        var query = new GetAllVehicleGroupQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20,
            SortBy = "name",
            SortDescending = false
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();
        result.Items.Should().HaveCount(20); // First page should have 20 items
        result.TotalCount.Should().BeGreaterThanOrEqualTo(50);
        result.TotalPages.Should().BeGreaterThanOrEqualTo(3); // At least 3 pages for 50+ items

        // Verify we got some of our created groups
        var ourGroups = result.Items.Where(x => groupIds.Contains(x.ID)).ToList();
        ourGroups.Should().NotBeEmpty();

        // Verify sorting is working
        var sortedNames = result.Items.Select(x => x.Name).ToList();
        sortedNames.Should().BeInAscendingOrder();
    }

    // Helper method to create vehicle groups with specific properties
    private async Task<int> CreateVehicleGroupAsync(
        string? name = null,
        string? description = null,
        bool isActive = true)
    {
        var createCommand = new Application.Features.VehicleGroups.Command.CreateVehicleGroup.CreateVehicleGroupCommand(
            Name: name ?? $"Test Vehicle Group {Faker.Random.AlphaNumeric(5)}",
            Description: description ?? $"Test description {Faker.Random.AlphaNumeric(8)}",
            IsActive: isActive
        );

        return await Sender.Send(createCommand);
    }
}