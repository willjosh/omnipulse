using Application.Features.ServiceTasks.Command.CreateServiceTask;

using Domain.Entities;
using Domain.Entities.Enums;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.ServiceTasks;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(ServiceTask))]
public class CreateServiceTaskIntegrationTests : BaseIntegrationTest
{
    public CreateServiceTaskIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddServiceTaskToDatabase_When_CommandIsValid()
    {
        // Arrange
        var createCommand = new CreateServiceTaskCommand(
            Name: $"Test {nameof(ServiceTask)} Name",
            Description: $"Test {nameof(ServiceTask)} Description",
            EstimatedLabourHours: 2.5,
            EstimatedCost: 150.00m,
            Category: ServiceTaskCategoryEnum.PREVENTIVE,
            IsActive: true
        );

        // Act
        int createdServiceTaskId = await Sender.Send(createCommand);

        // Assert
        ServiceTask? createdServiceTaskEntity = await DbContext.ServiceTasks.FindAsync(createdServiceTaskId);
        createdServiceTaskEntity.Should().NotBeNull();

        createdServiceTaskEntity.Name.Should().Be(createCommand.Name);
        createdServiceTaskEntity.Description.Should().Be(createCommand.Description);
        createdServiceTaskEntity.EstimatedLabourHours.Should().Be(createCommand.EstimatedLabourHours);
        createdServiceTaskEntity.EstimatedCost.Should().Be(createCommand.EstimatedCost);
        createdServiceTaskEntity.Category.Should().Be(createCommand.Category);
        createdServiceTaskEntity.IsActive.Should().BeTrue();
    }
}