using Application.Features.ServiceTasks.Command.CreateServiceTask;
using Application.Features.ServiceTasks.Query.GetAllServiceTask;
using Application.Models.PaginationModels;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

using Microsoft.EntityFrameworkCore;

namespace IntegrationTests.ServiceTasks;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceTask))]
public class GetAllServiceTaskIntegrationTest : BaseIntegrationTest
{
    public GetAllServiceTaskIntegrationTest(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_ReturnPagedServiceTasks_When_QueryIsValid()
    {
        // Arrange - Create multiple service tasks
        var task1Id = await CreateServiceTaskAsync(
            name: "Oil Change Service",
            description: "Change engine oil and filter",
            estimatedLabourHours: 1.0,
            estimatedCost: 50.00m,
            category: ServiceTaskCategoryEnum.PREVENTIVE,
            isActive: true
        );

        var task2Id = await CreateServiceTaskAsync(
            name: "Brake Inspection",
            description: "Inspect brake pads and rotors",
            estimatedLabourHours: 0.5,
            estimatedCost: 25.00m,
            category: ServiceTaskCategoryEnum.PREVENTIVE,
            isActive: true
        );

        var task3Id = await CreateServiceTaskAsync(
            name: "Engine Repair",
            description: "Major engine repair work",
            estimatedLabourHours: 8.0,
            estimatedCost: 1200.00m,
            category: ServiceTaskCategoryEnum.CORRECTIVE,
            isActive: false
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
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

        // Verify the created tasks are in the result
        var createdTasks = result.Items.Where(x =>
            x.ID == task1Id || x.ID == task2Id || x.ID == task3Id).ToList();

        createdTasks.Should().HaveCount(3);

        // Verify Oil Change Service
        var oilChangeTask = createdTasks.First(x => x.ID == task1Id);
        oilChangeTask.Name.Should().Be("Oil Change Service");
        oilChangeTask.Description.Should().Be("Change engine oil and filter");
        oilChangeTask.EstimatedLabourHours.Should().Be(1.0);
        oilChangeTask.EstimatedCost.Should().Be(50.00m);
        oilChangeTask.Category.Should().Be(ServiceTaskCategoryEnum.PREVENTIVE);
        oilChangeTask.IsActive.Should().BeTrue();

        // Verify Brake Inspection
        var brakeTask = createdTasks.First(x => x.ID == task2Id);
        brakeTask.Name.Should().Be("Brake Inspection");
        brakeTask.Category.Should().Be(ServiceTaskCategoryEnum.PREVENTIVE);

        // Verify Engine Repair
        var engineTask = createdTasks.First(x => x.ID == task3Id);
        engineTask.Name.Should().Be("Engine Repair");
        engineTask.Category.Should().Be(ServiceTaskCategoryEnum.CORRECTIVE);
        engineTask.IsActive.Should().BeFalse();
    }

    [Fact]
    public async Task Should_ReturnEmptyResult_When_NoServiceTasksExist()
    {
        // Arrange - Clear existing service tasks
        var existingTasks = await DbContext.ServiceTasks.ToListAsync();
        DbContext.ServiceTasks.RemoveRange(existingTasks);
        await DbContext.SaveChangesAsync();

        var query = new GetAllServiceTaskQuery(new PaginationParameters
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
        // Arrange - Create 8 service tasks
        var taskIds = new List<int>();
        for (int i = 1; i <= 8; i++)
        {
            var taskId = await CreateServiceTaskAsync(
                name: $"Service Task {i:D2}",
                description: $"Description for task {i}",
                estimatedLabourHours: i * 0.5,
                estimatedCost: i * 25.00m,
                category: i % 2 == 0 ? ServiceTaskCategoryEnum.PREVENTIVE : ServiceTaskCategoryEnum.CORRECTIVE,
                isActive: i % 3 != 0 // Most tasks active, every 3rd inactive
            );
            taskIds.Add(taskId);
        }

        // Act - Get first page (3 tasks per page)
        var firstPageQuery = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 3,
            SortBy = "name",
            SortDescending = false
        });

        var firstPageResult = await Sender.Send(firstPageQuery);

        // Act - Get second page
        var secondPageQuery = new GetAllServiceTaskQuery(new PaginationParameters
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

        // Verify no duplicate tasks between pages
        var firstPageIds = firstPageResult.Items.Select(x => x.ID).ToList();
        var secondPageIds = secondPageResult.Items.Select(x => x.ID).ToList();
        firstPageIds.Should().NotIntersectWith(secondPageIds);
    }

    [Fact]
    public async Task Should_FilterBySearch_When_SearchParameterProvided()
    {
        // Arrange
        var oilChangeId = await CreateServiceTaskAsync(
            name: "Oil Change Service",
            description: "Change engine oil and filter"
        );

        var brakeServiceId = await CreateServiceTaskAsync(
            name: "Brake Service",
            description: "Brake pad replacement and rotor service"
        );

        var tireRotationId = await CreateServiceTaskAsync(
            name: "Tire Rotation",
            description: "Rotate tires for even wear"
        );

        // Act - Search for "oil"
        var oilSearchQuery = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "oil"
        });

        var oilSearchResult = await Sender.Send(oilSearchQuery);

        // Act - Search for "brake"
        var brakeSearchQuery = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10,
            Search = "brake"
        });

        var brakeSearchResult = await Sender.Send(brakeSearchQuery);

        // Assert Oil Search
        oilSearchResult.Should().NotBeNull();
        oilSearchResult.Items.Should().Contain(x => x.ID == oilChangeId);
        oilSearchResult.Items.Should().NotContain(x => x.ID == brakeServiceId || x.ID == tireRotationId);

        // Assert Brake Search
        brakeSearchResult.Should().NotBeNull();
        brakeSearchResult.Items.Should().Contain(x => x.ID == brakeServiceId);
        brakeSearchResult.Items.Should().NotContain(x => x.ID == oilChangeId || x.ID == tireRotationId);
    }

    [Theory]
    [InlineData("name", false)]
    [InlineData("name", true)]
    [InlineData("description", false)]
    [InlineData("description", true)]
    [InlineData("estimatedlabourhours", false)]
    [InlineData("estimatedlabourhours", true)]
    [InlineData("estimatedcost", false)]
    [InlineData("estimatedcost", true)]
    [InlineData("category", false)]
    [InlineData("category", true)]
    [InlineData("isactive", false)]
    [InlineData("isactive", true)]
    public async Task Should_SortCorrectly_When_SortParametersProvided(string sortBy, bool sortDescending)
    {
        // Arrange
        await CreateServiceTaskAsync(
            name: "Alpha Task",
            description: "First task description",
            estimatedLabourHours: 1.0,
            estimatedCost: 100.00m,
            category: ServiceTaskCategoryEnum.PREVENTIVE,
            isActive: true
        );

        await CreateServiceTaskAsync(
            name: "Beta Task",
            description: "Second task description",
            estimatedLabourHours: 2.0,
            estimatedCost: 200.00m,
            category: ServiceTaskCategoryEnum.CORRECTIVE,
            isActive: false
        );

        await CreateServiceTaskAsync(
            name: "Charlie Task",
            description: "Third task description",
            estimatedLabourHours: 0.5,
            estimatedCost: 50.00m,
            category: ServiceTaskCategoryEnum.EMERGENCY,
            isActive: true
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
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
        var sortedTasks = result.Items.Where(x =>
            x.Name.Contains("Alpha") || x.Name.Contains("Beta") || x.Name.Contains("Charlie")).ToList();
        sortedTasks.Should().HaveCount(3);

        switch (sortBy.ToLowerInvariant())
        {
            case "name":
                if (sortDescending)
                {
                    sortedTasks.Should().BeInDescendingOrder(x => x.Name);
                }
                else
                {
                    sortedTasks.Should().BeInAscendingOrder(x => x.Name);
                }
                break;

            case "description":
                if (sortDescending)
                {
                    sortedTasks.Should().BeInDescendingOrder(x => x.Description);
                }
                else
                {
                    sortedTasks.Should().BeInAscendingOrder(x => x.Description);
                }
                break;

            case "estimatedlabourhours":
                if (sortDescending)
                {
                    sortedTasks.Should().BeInDescendingOrder(x => x.EstimatedLabourHours);
                }
                else
                {
                    sortedTasks.Should().BeInAscendingOrder(x => x.EstimatedLabourHours);
                }
                break;

            case "estimatedcost":
                if (sortDescending)
                {
                    sortedTasks.Should().BeInDescendingOrder(x => x.EstimatedCost);
                }
                else
                {
                    sortedTasks.Should().BeInAscendingOrder(x => x.EstimatedCost);
                }
                break;

            case "category":
                if (sortDescending)
                {
                    sortedTasks.Should().BeInDescendingOrder(x => x.Category);
                }
                else
                {
                    sortedTasks.Should().BeInAscendingOrder(x => x.Category);
                }
                break;

            case "isactive":
                if (sortDescending)
                {
                    sortedTasks.Should().BeInDescendingOrder(x => x.IsActive);
                }
                else
                {
                    sortedTasks.Should().BeInAscendingOrder(x => x.IsActive);
                }
                break;
        }
    }

    [Theory]
    [InlineData(ServiceTaskCategoryEnum.PREVENTIVE)]
    [InlineData(ServiceTaskCategoryEnum.CORRECTIVE)]
    [InlineData(ServiceTaskCategoryEnum.EMERGENCY)]
    public async Task Should_HandleDifferentCategories_When_TasksHaveVariousCategories(ServiceTaskCategoryEnum category)
    {
        // Arrange
        var taskId = await CreateServiceTaskAsync(
            name: $"{category} Test Task",
            description: $"Task for {category} category",
            category: category
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var testTask = result.Items.First(x => x.ID == taskId);
        testTask.Category.Should().Be(category);
        testTask.Name.Should().Be($"{category} Test Task");
        testTask.Description.Should().Be($"Task for {category} category");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_HandleActiveAndInactiveTasks_When_IsActiveVaries(bool isActive)
    {
        // Arrange
        var taskId = await CreateServiceTaskAsync(
            name: isActive ? "Active Test Task" : "Inactive Test Task",
            description: isActive ? "This task is active" : "This task is inactive",
            isActive: isActive
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var testTask = result.Items.First(x => x.ID == taskId);
        testTask.IsActive.Should().Be(isActive);
        testTask.Name.Should().Be(isActive ? "Active Test Task" : "Inactive Test Task");
        testTask.Description.Should().Be(isActive ? "This task is active" : "This task is inactive");
    }

    [Fact]
    public async Task Should_HandleMixedCategories_When_TasksHaveVariousCategories()
    {
        // Arrange
        var preventiveTaskId = await CreateServiceTaskAsync(
            name: "Preventive Maintenance Task",
            description: "Regular maintenance task",
            category: ServiceTaskCategoryEnum.PREVENTIVE,
            isActive: true
        );

        var correctiveTaskId = await CreateServiceTaskAsync(
            name: "Corrective Repair Task",
            description: "Repair task for issues",
            category: ServiceTaskCategoryEnum.CORRECTIVE,
            isActive: true
        );

        var emergencyTaskId = await CreateServiceTaskAsync(
            name: "Emergency Response Task",
            description: "Emergency repair task",
            category: ServiceTaskCategoryEnum.EMERGENCY,
            isActive: true
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdTasks = result.Items.Where(x =>
            x.ID == preventiveTaskId || x.ID == correctiveTaskId || x.ID == emergencyTaskId).ToList();

        createdTasks.Should().HaveCount(3);

        var preventiveTask = createdTasks.First(x => x.ID == preventiveTaskId);
        var correctiveTask = createdTasks.First(x => x.ID == correctiveTaskId);
        var emergencyTask = createdTasks.First(x => x.ID == emergencyTaskId);

        preventiveTask.Category.Should().Be(ServiceTaskCategoryEnum.PREVENTIVE);
        preventiveTask.Name.Should().Be("Preventive Maintenance Task");

        correctiveTask.Category.Should().Be(ServiceTaskCategoryEnum.CORRECTIVE);
        correctiveTask.Name.Should().Be("Corrective Repair Task");

        emergencyTask.Category.Should().Be(ServiceTaskCategoryEnum.EMERGENCY);
        emergencyTask.Name.Should().Be("Emergency Response Task");
    }

    [Fact]
    public async Task Should_HandleVariousCosts_When_TasksHaveDifferentCosts()
    {
        // Arrange
        var lowCostTaskId = await CreateServiceTaskAsync(
            name: "Low Cost Task",
            description: "Inexpensive maintenance task",
            estimatedCost: 25.00m,
            estimatedLabourHours: 0.5
        );

        var mediumCostTaskId = await CreateServiceTaskAsync(
            name: "Medium Cost Task",
            description: "Moderate cost repair task",
            estimatedCost: 150.00m,
            estimatedLabourHours: 2.0
        );

        var highCostTaskId = await CreateServiceTaskAsync(
            name: "High Cost Task",
            description: "Expensive overhaul task",
            estimatedCost: 1500.00m,
            estimatedLabourHours: 12.0
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 20,
            SortBy = "estimatedcost",
            SortDescending = false
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        result.Should().NotBeNull();

        var createdTasks = result.Items.Where(x =>
            x.ID == lowCostTaskId || x.ID == mediumCostTaskId || x.ID == highCostTaskId).ToList();

        createdTasks.Should().HaveCount(3);

        var lowCostTask = createdTasks.First(x => x.ID == lowCostTaskId);
        var mediumCostTask = createdTasks.First(x => x.ID == mediumCostTaskId);
        var highCostTask = createdTasks.First(x => x.ID == highCostTaskId);

        lowCostTask.EstimatedCost.Should().Be(25.00m);
        lowCostTask.EstimatedLabourHours.Should().Be(0.5);

        mediumCostTask.EstimatedCost.Should().Be(150.00m);
        mediumCostTask.EstimatedLabourHours.Should().Be(2.0);

        highCostTask.EstimatedCost.Should().Be(1500.00m);
        highCostTask.EstimatedLabourHours.Should().Be(12.0);
    }

    [Fact]
    public async Task Should_IncludeAllRequiredProperties_When_ReturningServiceTasks()
    {
        // Arrange
        var taskId = await CreateServiceTaskAsync(
            name: "Complete Properties Test Task",
            description: "Testing all properties",
            estimatedLabourHours: 3.5,
            estimatedCost: 275.00m,
            category: ServiceTaskCategoryEnum.PREVENTIVE,
            isActive: true
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act
        var result = await Sender.Send(query);

        // Assert
        var testTask = result.Items.First(x => x.ID == taskId);

        // Verify all required properties are populated
        testTask.ID.Should().Be(taskId);
        testTask.Name.Should().Be("Complete Properties Test Task");
        testTask.Description.Should().Be("Testing all properties");
        testTask.EstimatedLabourHours.Should().Be(3.5);
        testTask.EstimatedCost.Should().Be(275.00m);
        testTask.Category.Should().Be(ServiceTaskCategoryEnum.PREVENTIVE);
        testTask.IsActive.Should().BeTrue();
    }

    [Fact]
    public async Task Should_ReturnConsistentData_When_CalledMultipleTimes()
    {
        // Arrange
        var taskId = await CreateServiceTaskAsync(
            name: "Consistency Test Task",
            description: "Testing data consistency",
            estimatedCost: 89.99m,
            isActive: true
        );

        var query = new GetAllServiceTaskQuery(new PaginationParameters
        {
            PageNumber = 1,
            PageSize = 10
        });

        // Act - Call multiple times
        var result1 = await Sender.Send(query);
        var result2 = await Sender.Send(query);
        var result3 = await Sender.Send(query);

        // Assert - All results should be consistent
        var task1 = result1.Items.First(x => x.ID == taskId);
        var task2 = result2.Items.First(x => x.ID == taskId);
        var task3 = result3.Items.First(x => x.ID == taskId);

        task1.Should().BeEquivalentTo(task2);
        task2.Should().BeEquivalentTo(task3);
        task1.Should().BeEquivalentTo(task3);

        // Verify specific properties are consistent
        task1.ID.Should().Be(task2.ID).And.Be(task3.ID);
        task1.Name.Should().Be(task2.Name).And.Be(task3.Name);
        task1.EstimatedCost.Should().Be(task2.EstimatedCost).And.Be(task3.EstimatedCost);
        task1.IsActive.Should().Be(task2.IsActive).And.Be(task3.IsActive);
    }

    // Helper method to create service tasks with specific properties
    private async Task<int> CreateServiceTaskAsync(
        string? name = null,
        string? description = null,
        double estimatedLabourHours = 1.0,
        decimal estimatedCost = 100.00m,
        ServiceTaskCategoryEnum category = ServiceTaskCategoryEnum.PREVENTIVE,
        bool isActive = true)
    {
        var createCommand = new CreateServiceTaskCommand(
            Name: name ?? $"Test Service Task {Faker.Random.AlphaNumeric(5)}",
            Description: description ?? $"Test description {Faker.Random.AlphaNumeric(8)}",
            EstimatedLabourHours: estimatedLabourHours,
            EstimatedCost: estimatedCost,
            Category: category,
            IsActive: isActive
        );

        return await Sender.Send(createCommand);
    }
}