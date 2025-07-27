namespace FunctionalTests.Abstractions;

public class BaseFunctionalTest : IClassFixture<FunctionalTestWebAppFactory>
{
    public BaseFunctionalTest(FunctionalTestWebAppFactory factory)
    {
        HttpClient = factory.CreateClient();
    }

    protected HttpClient HttpClient { get; init; }

    [Fact(DisplayName = "GET /swagger returns 200 OK")]
    public async Task Get_SwaggerUI_Returns_Ok()
    {
        var response = await HttpClient.GetAsync("/swagger");
        response.EnsureSuccessStatusCode();
    }
}