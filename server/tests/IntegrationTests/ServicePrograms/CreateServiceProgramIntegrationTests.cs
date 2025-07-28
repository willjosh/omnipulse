using Application.Features.ServicePrograms.Command.CreateServiceProgram;

using Domain.Entities;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.ServicePrograms;

public class CreateServiceProgramIntegrationTests : BaseIntegrationTest
{
    public CreateServiceProgramIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddServiceProgramToDatabase_When_CommandIsValid()
    {
        // Arrange
        var createCommand = new CreateServiceProgramCommand(
            Name: $"Valid {nameof(ServiceProgram)} Name",
            Description: $"Valid {nameof(ServiceProgram)} Description",
            IsActive: true
        );

        // Act
        int createdServiceProgramId = await Sender.Send(createCommand);

        // Assert
        ServiceProgram? createdServiceProgramEntity = await DbContext.ServicePrograms.FindAsync(createdServiceProgramId);
        createdServiceProgramEntity.Should().NotBeNull();

        createdServiceProgramEntity.Name.Should().Be(createCommand.Name);
        createdServiceProgramEntity.Description.Should().Be(createCommand.Description);
        createdServiceProgramEntity.IsActive.Should().BeTrue();
    }
}