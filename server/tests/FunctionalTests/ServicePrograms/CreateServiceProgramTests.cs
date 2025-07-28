using System.Net;
using System.Net.Http.Json;

using Application.Features.ServicePrograms.Command.CreateServiceProgram;

using FluentAssertions;

using FunctionalTests.Abstractions;
using FunctionalTests.Extensions;

namespace FunctionalTests.ServicePrograms;

[Trait("Entity", "Service Program")]
[Trait("Route", CreateServiceProgramRoute)]
[Trait("TestCategory", "Functional")]
public class CreateServiceProgramTests : BaseFunctionalTest
{
    public CreateServiceProgramTests(FunctionalTestWebAppFactory factory) : base(factory) { }

    // Constants
    private const string CreateServiceProgramRoute = "/api/serviceprograms";
    private const int NameMaxLength = 200;
    private const int DescriptionMaxLength = 500;

    [Theory]
    [InlineData("Test Service Program", "A test service program for functional testing", true)]
    [InlineData("Service Program Without Description", null, true)]
    [InlineData("Service Program With Empty Description", "", true)]
    public async Task Should_CreateServiceProgram_WithDifferentDescriptionValues(string name, string? description, bool isActive)
    {
        // Arrange
        var request = new CreateServiceProgramCommand(
            Name: name,
            Description: description,
            IsActive: isActive
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var serviceProgramId = await response.Content.ReadFromJsonAsync<int>();
        serviceProgramId.Should().BeGreaterThan(0);

        // Verify the location header points to the created resource
        response.Headers.Location.Should().NotBeNull();
        response.Headers.Location!.ToString().Should().Contain($"/api/ServicePrograms/{serviceProgramId}");
    }

    [Theory]
    [InlineData(true)]
    [InlineData(false)]
    public async Task Should_CreateServiceProgram_WithDifferentIsActiveValues(bool isActive)
    {
        // Arrange
        var request = new CreateServiceProgramCommand(
            Name: $"Service Program IsActive={isActive}",
            Description: "Testing different IsActive values",
            IsActive: isActive
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var serviceProgramId = await response.Content.ReadFromJsonAsync<int>();
        serviceProgramId.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(nameof(CreateServiceProgramCommand.Name), NameMaxLength, "Service Program With Long Description")]
    [InlineData(nameof(CreateServiceProgramCommand.Description), DescriptionMaxLength, "Testing name at maximum length")]
    public async Task Should_CreateServiceProgram_WithFieldAtMaxLength(string field, int length, string otherFieldValue)
    {
        // Arrange
        var value = new string(field == nameof(CreateServiceProgramCommand.Name) ? 'N' : 'D', length);
        var request = new CreateServiceProgramCommand(
            Name: field == nameof(CreateServiceProgramCommand.Name) ? value : otherFieldValue,
            Description: field == nameof(CreateServiceProgramCommand.Description) ? value : otherFieldValue,
            IsActive: true
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var serviceProgramId = await response.Content.ReadFromJsonAsync<int>();
        serviceProgramId.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData(null)]
    public async Task Should_ReturnBadRequest_WhenNameIsNull(string? name)
    {
        // Arrange
        var request = new CreateServiceProgramCommand(
            Name: name!,
            Description: "Service Program Description",
            IsActive: true
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("\t")]
    [InlineData("\n")]
    public async Task Should_ReturnBadRequest_WhenNameIsEmptyOrWhitespace(string? name)
    {
        // Arrange
        var request = new CreateServiceProgramCommand(
            Name: name!,
            Description: "Service Program Description",
            IsActive: true
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Theory]
    [InlineData(nameof(CreateServiceProgramCommand.Name), NameMaxLength + 1, "Valid Service Program Name", "Service Program Description")]
    [InlineData(nameof(CreateServiceProgramCommand.Description), DescriptionMaxLength + 1, "Valid Service Program Name", "Valid Service Program Name")]
    public async Task Should_ReturnBadRequest_WhenFieldTooLong(string field, int length, string name, string description)
    {
        // Arrange
        var value = new string(field == nameof(CreateServiceProgramCommand.Name) ? 'N' : 'D', length);
        var request = new CreateServiceProgramCommand(
            Name: field == nameof(CreateServiceProgramCommand.Name) ? value : name,
            Description: field == nameof(CreateServiceProgramCommand.Description) ? value : description,
            IsActive: true
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task Should_ReturnConflict_WhenServiceProgramWithSameNameExists()
    {
        // Arrange - Create first service program
        var firstRequest = new CreateServiceProgramCommand(
            Name: "Duplicate Name Test",
            Description: "First service program",
            IsActive: true
        );

        // Arrange - Try to create second service program with same name
        var secondRequest = new CreateServiceProgramCommand(
            Name: "Duplicate Name Test",
            Description: "Second service program with same name",
            IsActive: false
        );

        // Act
        var firstResponse = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, firstRequest);
        var secondResponse = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, secondRequest);

        // Assert
        firstResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Conflict);
    }

    [Theory]
    [InlineData("Service Program with Special Chars: !@#$%^&*()_+-=[]{}|;':\",./<>?", "Testing special characters in name")]
    [InlineData("Service Program with Unicode: ˜ˆç˙ø¬åß Âå††˙´∑ Íå∑", "Testing unicode characters in name")]
    public async Task Should_HandleSpecialAndUnicodeCharacters_InName(string name, string description)
    {
        // Arrange
        var request = new CreateServiceProgramCommand(
            Name: name,
            Description: description,
            IsActive: true
        );

        // Act
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var serviceProgramId = await response.Content.ReadFromJsonAsync<int>();
        serviceProgramId.Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task Should_ReturnUnsupportedMediaType_WhenContentTypeIsNotJson()
    {
        // Arrange
        var content = new StringContent("invalid json content", System.Text.Encoding.UTF8, "text/plain");

        // Act
        var response = await HttpClient.PostAsync(CreateServiceProgramRoute, content);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.UnsupportedMediaType);
    }

    [Fact]
    public async Task Should_CompleteRequest_WithinReasonableTime()
    {
        // Arrange
        var request = new CreateServiceProgramCommand(
            Name: "Performance Test Service Program",
            Description: "Testing request completion time",
            IsActive: true
        );

        // Act & Assert
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();
        var response = await HttpClient.PostAsJsonAsync(CreateServiceProgramRoute, request);
        stopwatch.Stop();

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        stopwatch.ElapsedMilliseconds.Should().BeLessThan(5000); // Should complete within 5 seconds
    }
}