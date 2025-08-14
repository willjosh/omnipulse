using Application.Features.ServicePrograms.Command.CreateServiceProgram;
using Application.Features.ServicePrograms.Query.GetAllServiceProgram;
using Application.Models.PaginationModels;

using Domain.Entities;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ServicePrograms;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceProgram))]
public class GetAllServiceProgramIntegrationTest : BaseIntegrationTest
{
    public GetAllServiceProgramIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnPagedServicePrograms_When_QueryIsValid()
    {
        // Arrange - Create multiple service programs
        var program1Id = await CreateServiceProgramAsync(
            name: "Preventive Maintenance Program",
            description: "Regular maintenance for all vehicles",
            isActive: true
        );

        var program2Id = await CreateServiceProgramAsync(
            name: "Emergency Repair Program",
            description: "Emergency and breakdown repairs",
            isActive: true
        );

        var program3Id = await CreateServiceProgramAsync(
            name: "Seasonal Maintenance Program",
            description: "Seasonal vehicle preparations",
            isActive: false
        );

        var query = new GetAllServiceProgramQuery(new PaginationParameters
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

        // Verify the created programs are in the result
        var createdPrograms = result.Items.Where(x =>
            x.ID == program1Id || x.ID == program2Id || x.ID == program3Id).ToList();

        createdPrograms.Should().HaveCount(3);

        // Verify Preventive Maintenance Program
        var preventiveProgram = createdPrograms.First(x => x.ID == program1Id);
        preventiveProgram.Name.Should().Be("Preventive Maintenance Program");
        preventiveProgram.Description.Should().Be("Regular maintenance for all vehicles");
        preventiveProgram.IsActive.Should().BeTrue();

        // Verify Emergency Repair Program
        var emergencyProgram = createdPrograms.First(x => x.ID == program2Id);
        emergencyProgram.Name.Should().Be("Emergency Repair Program");
        emergencyProgram.Description.Should().Be("Emergency and breakdown repairs");
        emergencyProgram.IsActive.Should().BeTrue();

        // Verify Seasonal Maintenance Program
        var seasonalProgram = createdPrograms.First(x => x.ID == program3Id);
        seasonalProgram.Name.Should().Be("Seasonal Maintenance Program");
        seasonalProgram.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnEmptyResult_When_NoServiceProgramsExist()
    {
        // Arrange - Clear existing service programs
        var existingPrograms = await DbContext.ServicePrograms.ToListAsync();
        DbContext.ServicePrograms.RemoveRange(existingPrograms);
        await DbContext.SaveChangesAsync();

        var query = new GetAllServiceProgramQuery(new PaginationParameters
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
        // Arrange - Create 8 service programs
        var programIds = new List<int>();
        for (int i = 1; i <= 8; i++)
        {
            var programId = await CreateServiceProgramAsync(
                name: $"Service Program {i:D2}",
                description: $"Description for program {i}",
                isActive: i % 2 == 0 // Alternate active/inactive
            );
            programIds.Add(programId);
        }

        // Act - Get first page (3 programs per page)
        var firstPageQuery = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 3,
            SortBy = "name",
            SortDescending = false
        });

        var firstPageResult = await Sender.Send(firstPageQuery);

        // Act - Get second page
        var secondPageQuery = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 2,
            PageSize = 3,
            SortBy = "name",
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

        // Verify no duplicate programs between pages
        var firstPageIds = firstPageResult.Items.Select(x => x.ID).ToList();
        var secondPageIds = secondPageResult.Items.Select(x => x.ID).ToList();
        firstPageIds.Should().NotIntersectWith(secondPageIds);
    }

    [Fact]
    public async Task Should_FilterBySearch_When_SearchParameterProvided()
    {
        // Arrange
        var maintenanceId = await CreateServiceProgramAsync(
            name: "Vehicle Maintenance Program",
            description: "Regular vehicle maintenance and inspections"
        );

        var repairId = await CreateServiceProgramAsync(
            name: "Repair Service Program",
            description: "Vehicle repair and troubleshooting services"
        );

        var inspectionId = await CreateServiceProgramAsync(
            name: "Safety Inspection Program",
            description: "Safety and compliance inspections"
        );

        // Act - Search for "maintenance"
        var maintenanceSearchQuery = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "maintenance"
        });

        var maintenanceSearchResult = await Sender.Send(maintenanceSearchQuery);

        // Act - Search for "repair"
        var repairSearchQuery = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "repair"
        });

        var repairSearchResult = await Sender.Send(repairSearchQuery);

        // Assert Maintenance Search
        maintenanceSearchResult.Should().NotBeNull();
        maintenanceSearchResult.Items.Should().Contain(x => x.ID == maintenanceId);
        maintenanceSearchResult.Items.Should().NotContain(x => x.ID == repairId || x.ID == inspectionId);

        // Assert Repair Search
        repairSearchResult.Should().NotBeNull();
        repairSearchResult.Items.Should().Contain(x => x.ID == repairId);
        repairSearchResult.Items.Should().NotContain(x => x.ID == maintenanceId || x.ID == inspectionId);
    }

    [Theory]
    [InlineData("name", false)]
    [InlineData("name", true)]
    [InlineData("description", false)]
    [InlineData("description", true)]
    [InlineData("isactive", false)]
    [InlineData("isactive", true)]
    public async Task Should_SortCorrectly_When_SortParametersProvided(string sortBy, bool sortDescending)
    {
        // Arrange
        await CreateServiceProgramAsync(
            name: "Alpha Program",
            description: "First program description",
            isActive: true
        );

        await CreateServiceProgramAsync(
            name: "Beta Program",
            description: "Second program description",
            isActive: false
        );

        await CreateServiceProgramAsync(
            name: "Charlie Program",
            description: "Third program description",
            isActive: true
        );

        var query = new GetAllServiceProgramQuery(new PaginationParameters
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
        var sortedPrograms = result.Items.Where(x =>
            x.Name.Contains("Alpha") || x.Name.Contains("Beta") || x.Name.Contains("Charlie")).ToList();
        sortedPrograms.Should().HaveCount(3);

        switch (sortBy.ToLowerInvariant())
        {
            case "name":
                if (sortDescending)
                {
                    sortedPrograms.Should().BeInDescendingOrder(x => x.Name);
                }
                else
                {
                    sortedPrograms.Should().BeInAscendingOrder(x => x.Name);
                }
                break;

            case "description":
                if (sortDescending)
                {
                    sortedPrograms.Should().BeInDescendingOrder(x => x.Description);
                }
                else
                {
                    sortedPrograms.Should().BeInAscendingOrder(x => x.Description);
                }
                break;

            case "isactive":
                if (sortDescending)
                {
                    sortedPrograms.Should().BeInDescendingOrder(x => x.IsActive);
                }
                else
                {
                    sortedPrograms.Should().BeInAscendingOrder(x => x.IsActive);
                }
                break;
        }
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_HandleActiveAndInactivePrograms_When_IsActiveVaries(bool isActive)
    {
        // Arrange
        var programId = await CreateServiceProgramAsync(
            name: isActive ? "Active Test Program" : "Inactive Test Program",
            description: isActive ? "This program is active" : "This program is inactive",
            isActive: isActive
        );

        var query = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var testProgram = result.Items.First(x => x.ID == programId);
        testProgram.IsActive.Should().Be(isActive);
        testProgram.Name.Should().Be(isActive ? "Active Test Program" : "Inactive Test Program");
        testProgram.Description.Should().Be(isActive ? "This program is active" : "This program is inactive");
    }

    [Fact]
    public async Task Should_HandleMixedActiveStatus_When_ProgramsHaveVariousActiveStates()
    {
        // Arrange
        var activeProgramId = await CreateServiceProgramAsync(
            name: "Active Operations Program",
            description: "Currently active program",
            isActive: true
        );

        var inactiveProgramId = await CreateServiceProgramAsync(
            name: "Inactive Legacy Program",
            description: "Legacy inactive program",
            isActive: false
        );

        var query = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdPrograms = result.Items.Where(x =>
            x.ID == activeProgramId || x.ID == inactiveProgramId).ToList();

        createdPrograms.Should().HaveCount(2);

        var activeProgram = createdPrograms.First(x => x.ID == activeProgramId);
        var inactiveProgram = createdPrograms.First(x => x.ID == inactiveProgramId);

        activeProgram.IsActive.Should().BeTrue();
        activeProgram.Name.Should().Be("Active Operations Program");

        inactiveProgram.IsActive.Should().BeFalse();
        inactiveProgram.Name.Should().Be("Inactive Legacy Program");
    }

    [Fact]
    public async Task Should_IncludeAllRequiredProperties_When_ReturningServicePrograms()
    {
        // Arrange
        var programId = await CreateServiceProgramAsync(
            name: "Complete Properties Test Program",
            description: "Testing all properties",
            isActive: true
        );

        var query = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        var testProgram = result.Items.First(x => x.ID == programId);

        // Verify all required properties are populated
        testProgram.ID.Should().Be(programId);
        testProgram.Name.Should().Be("Complete Properties Test Program");
        testProgram.Description.Should().Be("Testing all properties");
        testProgram.IsActive.Should().BeTrue();
        testProgram.CreatedAt.Should().BeAfter(DateTime.MinValue);
        testProgram.UpdatedAt.Should().BeAfter(DateTime.MinValue);
        testProgram.CreatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
        testProgram.UpdatedAt.Should().BeOnOrBefore(DateTime.UtcNow);
    }

    [Fact]
    public async Task Should_HandleSpecialCharactersInNames_When_ProgramsHaveUnicodeContent()
    {
        // Arrange
        var specialCharsProgramId = await CreateServiceProgramAsync(
            name: "Spëcîål Chàrs & Símböls Program!",
            description: "Program with spëcîål characters, émojis 🚗🔧, and numbers (123)",
            isActive: true
        );

        var numbersProgramId = await CreateServiceProgramAsync(
            name: "Program-123_TEST",
            description: "Program with numbers and underscores",
            isActive: true
        );

        var query = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdPrograms = result.Items.Where(x =>
            x.ID == specialCharsProgramId || x.ID == numbersProgramId).ToList();

        createdPrograms.Should().HaveCount(2);

        var specialCharsProgram = createdPrograms.First(x => x.ID == specialCharsProgramId);
        var numbersProgram = createdPrograms.First(x => x.ID == numbersProgramId);

        specialCharsProgram.Name.Should().Be("Spëcîål Chàrs & Símböls Program!");
        specialCharsProgram.Description.Should().Contain("spëcîål characters");

        numbersProgram.Name.Should().Be("Program-123_TEST");
        numbersProgram.Description.Should().Be("Program with numbers and underscores");
    }

    [Fact]
    public async Task Should_ReturnConsistentData_When_CalledMultipleTimes()
    {
        // Arrange
        var programId = await CreateServiceProgramAsync(
            name: "Consistency Test Program",
            description: "Testing data consistency",
            isActive: true
        );

        var query = new GetAllServiceProgramQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act - Call multiple times
        var result1 = await Sender.Send(query);
        var result2 = await Sender.Send(query);
        var result3 = await Sender.Send(query);

        // Assert - All results should be consistent
        var program1 = result1.Items.First(x => x.ID == programId);
        var program2 = result2.Items.First(x => x.ID == programId);
        var program3 = result3.Items.First(x => x.ID == programId);

        program1.Should().BeEquivalentTo(program2);
        program2.Should().BeEquivalentTo(program3);
        program1.Should().BeEquivalentTo(program3);

        // Verify specific properties are consistent
        program1.ID.Should().Be(program2.ID).And.Be(program3.ID);
        program1.Name.Should().Be(program2.Name).And.Be(program3.Name);
        program1.IsActive.Should().Be(program2.IsActive).And.Be(program3.IsActive);
    }

    // Helper method to create service programs with specific properties
    private async Task<int> CreateServiceProgramAsync(
        string? name = null,
        string? description = null,
        bool isActive = true)
    {
        var createCommand = new CreateServiceProgramCommand(
            Name: name ?? $"Test Service Program {Faker.Random.AlphaNumeric(5)}",
            Description: description ?? $"Test description {Faker.Random.AlphaNumeric(8)}",
            IsActive: isActive
        );

        return await Sender.Send(createCommand);
    }
}