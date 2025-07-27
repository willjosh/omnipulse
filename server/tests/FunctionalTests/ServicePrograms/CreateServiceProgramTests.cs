using System.Net;
using System.Net.Http.Json;

using Application.Features.ServicePrograms.Command.CreateServiceProgram;

using FluentAssertions;

using FunctionalTests.Abstractions;
using FunctionalTests.Extensions;

namespace FunctionalTests.ServicePrograms;

public class CreateServiceProgramTests : BaseFunctionalTest
{
    public CreateServiceProgramTests(FunctionalTestWebAppFactory factory) : base(factory) { }

    // Constants
    private const string CreateServiceProgramRoute = "/api/serviceprograms";

    [Trait("Entity", "Service Program")]
    [Trait("HTTP Method", "POST")]
    [Trait("Route", CreateServiceProgramRoute)]
    [Fact]
    public async Task Return_BadRequest_When_NameIsMissing()
    {
        // Arrange
        var request = new CreateServiceProgramCommand(
            Name: "", // Invalid
            Description: "Service Program Description",
            IsActive: true
        );

        // Act
        HttpResponseMessage response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);

        var problemDetails = await response.GetProblemDetailsAsync();
        problemDetails.ShouldHaveValidationErrorFor(nameof(CreateServiceProgramCommand.Name));
    }
}