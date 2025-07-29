using Application.Features.Users.Command.CreateTechnician;

using Domain.Entities;

using FluentAssertions;

using IntegrationTests.Abstractions;

namespace IntegrationTests.Users;

[Trait("TestCategory", "Integration")]
[Trait("Entity", nameof(User))]
public class CreateUserIntegrationTests : BaseIntegrationTest
{
    public CreateUserIntegrationTests(IntegrationTestWebAppFactory factory) : base(factory) { }

    [Fact]
    public async Task Should_AddUserToDatabase_When_CommandIsValid()
    {
        // Arrange
        var createCommand = new CreateTechnicianCommand(
            Email: "technician@omnipulse.com",
            Password: Faker.Internet.Password(length: 50, regexPattern: @"[a-zA-Z0-9!@#$%^&*()_+=\-]"),
            FirstName: Faker.Name.FirstName(),
            LastName: Faker.Name.LastName(),
            HireDate: DateTime.UtcNow.AddYears(-1),
            IsActive: true
        );

        // Act
        Guid createdTechnicianGuid = await Sender.Send(createCommand);

        // Assert
        User? createdTechnicianEntity = await DbContext.Users.FindAsync(createdTechnicianGuid.ToString());
        createdTechnicianEntity.Should().NotBeNull();

        createdTechnicianEntity.Email.Should().Be(createCommand.Email);
        createdTechnicianEntity.FirstName.Should().Be(createCommand.FirstName);
        createdTechnicianEntity.LastName.Should().Be(createCommand.LastName);
        createdTechnicianEntity.HireDate.Should().Be(createCommand.HireDate);
        createdTechnicianEntity.IsActive.Should().Be(createCommand.IsActive);
    }
}