using Application.Features.InspectionForms.Command.CreateInspectionForm;

using Domain.Entities;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.InspectionForms;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(InspectionForm))]
public class CreateInspectionFormIntegrationTests : BaseIntegrationTest
{
    public CreateInspectionFormIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddInspectionFormToDatabase_When_CommandIsValid()
    {
        // Arrange
        var createCommand = new CreateInspectionFormCommand(
            Title: $"Valid {nameof(InspectionForm)} Title {Faker.Random.AlphaNumeric(5)}",
            Description: $"Valid {nameof(InspectionForm)} Description {Faker.Random.AlphaNumeric(5)}",
            IsActive: true
        );

        // Act
        int createdInspectionFormId = await Sender.Send(createCommand);

        // Assert
        InspectionForm? createdInspectionFormEntity = await DbContext.InspectionForms.FindAsync(createdInspectionFormId);
        createdInspectionFormEntity.Should().NotBeNull();

        createdInspectionFormEntity.Title.Should().Be(createCommand.Title);
        createdInspectionFormEntity.Description.Should().Be(createCommand.Description);
        createdInspectionFormEntity.IsActive.Should().BeTrue();
    }
}